
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;
using Dapper;
using MySql.Data.MySqlClient;

using ShoppingPeeker.DbManage.Utilities;
using ShoppingPeeker.DbManage.CommandTree;
using ShoppingPeeker.Utilities.TypeFinder;
using ShoppingPeeker.Utilities;

namespace ShoppingPeeker.DbManage
{
    public class MySqlDbContext<TElement> : BaseSqlOperation<TElement>, IDbContext<TElement>, IDisposable
       where TElement : BaseEntity, new()
    {



        #region Construction and fields




        /// <summary>
        /// 实体的主键名称
        /// </summary>
        private string EntityIdentityFiledName = new TElement().GetIdentity().IdentityKeyName;

        /// <summary>
        /// 覆盖基类字段包裹
        /// </summary>
        public override string FieldWrapperChar { get; set; }


        public MySqlDbContext()
        {
            this.FieldWrapperChar = "`";
        }
        /// <summary>
        /// 数据上下文构造函数
        /// </summary>
        /// <param name="dbConfig"></param>
        public MySqlDbContext(DbConnConfig dbConfig):this()
        {
            this.DbConfig = dbConfig;
        }


        #endregion



        #region 对提供映射字段配置的数据，进行ORM对象获取



        #region  Insert操作
        /// <summary>
        /// 插入 实体
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="transaction">db事务</param>
        /// <returns></returns>
        public int Insert(TElement entity, IDbTransaction transaction = null)
        {
            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, true, out tableInDbName, out propertys, out filelds, out paras);

            ///不含主键的属性
            var noIdentityPropertys = propertys.Remove(x => x.Name == EntityIdentityFiledName);
            var noIdentityFileds = filelds.Remove(x => x == EntityIdentityFiledName);
            var noIdentityParas = paras.Remove(x => x == string.Format("@{0}", EntityIdentityFiledName));

            string splitor = string.Format("{0},{0}", this.FieldWrapperChar);
            var fieldSplitString = string.Concat(this.FieldWrapperChar, string.Join(splitor, noIdentityFileds), this.FieldWrapperChar);//返回逗号分隔的字符串 例如：`ProvinceCode`,`ProvinceName`
            var parasSplitString = string.Join(",", noIdentityParas);//参数   数组 的逗号分隔


            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.Append(string.Format("insert into {0}(", tableInDbName));
            sb_Sql.Append(string.Format("{0})", fieldSplitString));
            sb_Sql.Append(" values (");
            sb_Sql.Append(string.Format("{0})", parasSplitString));
            sb_Sql.Append(";select @@IDENTITY;");

            #region 无用代码 ---已经被Dapper 替代


            //MySqlParameter[] parameters = new MySqlParameter[noIdentityParas.Length];

            //var settedValueDic = entity.GetSettedValuePropertyDic();
            //for (int i = 0; i < noIdentityParas.Length; i++)
            //{
            //    var colName = noIdentityParas[i];
            //    string key = noIdentityPropertys[i].Name;
            //    object value = null;//ReflectionHelper.GetPropertyValue(entity, noIdentityPropertys[i]);
            //    settedValueDic.TryGetValue(key, out value);
            //    var para = new MySqlParameter(colName, value);

            //    para.IsNullable = true;

            //    parameters[i] = para;
            //}


            //例子：以上代码  代替下面的代码
            //{
            //        new SqlParameter("@ProvinceCode", SqlDbType.NVarChar,15),
            //        new SqlParameter("@ProvinceName", SqlDbType.NVarChar,50),
            //        new SqlParameter("@Submmary", SqlDbType.Text)};
            //parameters[0].Value = model.ProvinceCode;
            //parameters[1].Value = model.ProvinceName;
            //parameters[2].Value = model.Submmary;

            #endregion

            var sqlCmd = sb_Sql.ToString();


            ///清理掉字符串拼接构造器
            sb_Sql.Clear();
            sb_Sql = null;
            this.SqlOutPutToLogAsync(sqlCmd, entity);

            using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
            {
                var result = conn.ExecuteScalar<int>(sqlCmd, entity, transaction);
                return result;
            }
             
        }


        /// <summary>
        /// 单次批量多次插入多个实体,并返回执行的记录数目---参数化的形式
        /// 第二种使用db事务  ，使用代码事务 using(var tran=new TranstionScope()){ your  code.......}
        /// </summary>
        /// <param name="entities">实体集合</param>
        public bool InsertMulitiEntities(IEnumerable<TElement> entities, IDbTransaction transaction = null)
        {

            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entities.First(), true, out tableInDbName, out propertys, out filelds, out paras);

            this.SqlOutPutToLogAsync("InsertMulitiEntities", entities);

            using (var bcp = new MysqlBuckCopy<TElement>(this.DbConfig.ConnString))
            {
                return bcp.WriteToServer(entities, tableInDbName, transaction);
            }


        }


        #endregion


        #region Update 更新操作

        /// <summary>
        /// 更新单个模型
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Update(TElement entity, IDbTransaction transaction = null)
        {
            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, true, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return -1;
                throw new Exception("未指定除主键后其他字段！");
            }

            StringBuilder sb_FiledParaPairs = new StringBuilder("");
            var settedValueDic = entity.GetSettedValuePropertyDic();

            foreach (var item in settedValueDic)
            {
                var keyProperty = item.Key;
                //var value = item.Value;
                if (keyProperty != EntityIdentityFiledName)
                {
                    sb_FiledParaPairs.AppendFormat("{1}{0}{1}=@{0},", keyProperty,this.FieldWrapperChar);
                }
            }

            //移除最后一个逗号
            var str_FiledParaPairs = sb_FiledParaPairs.ToString();
            str_FiledParaPairs = str_FiledParaPairs.Remove(str_FiledParaPairs.Length - 1);

            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.Append(string.Format("update {0} set ", tableInDbName));//Set Table
            sb_Sql.Append(str_FiledParaPairs);//参数对

         

            sb_Sql.AppendFormat(" where {0}=@{0}", EntityIdentityFiledName);//主键


            var sqlCmd = sb_Sql.ToString();
            ///清理掉字符串拼接构造器
            sb_FiledParaPairs.Clear();
            sb_FiledParaPairs = null;
            sb_Sql.Clear();
            sb_Sql = null;
            this.SqlOutPutToLogAsync(sqlCmd, entity);
            using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
            {
                var result = conn.Execute(sqlCmd, entity, transaction);
                return result;
            }
         
        }




        /// <summary>
        /// 更新元素 通过  符合条件的
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int UpdateByCondition(TElement entity, Expression<Func<TElement, bool>> predicate, IDbTransaction transaction = null)
        {
            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, true, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return -1;
                throw new Exception("未指定除主键后其他字段！");
            }


            StringBuilder sb_FiledParaPairs = new StringBuilder("");
            ///解析要更新的列
            var settedValueDic = entity.GetSettedValuePropertyDic();

            foreach (var item in settedValueDic)
            {
                var keyProperty = item.Key;
                //var value = item.Value;
                if (keyProperty != EntityIdentityFiledName)
                {
                    sb_FiledParaPairs.AppendFormat("{1}{0}{1}=@{0},", keyProperty,this.FieldWrapperChar);
                }
            }
            //移除最后一个逗号
            var str_FiledParaPairs = sb_FiledParaPairs.ToString();
            str_FiledParaPairs = str_FiledParaPairs.Remove(str_FiledParaPairs.Length - 1);

            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.Append(string.Format("update {0} set ", tableInDbName));//Set Table
            sb_Sql.Append(str_FiledParaPairs);//参数对



            if (null != predicate)
            {
                string where = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate, wrapperChar:this.FieldWrapperChar);
                sb_Sql.Append(" where ");//解析条件
                sb_Sql.Append(where);//条件中带有参数=值的  拼接字符串
            }


            var sqlCmd = sb_Sql.ToString();

            ///清理字符串构建
            sb_FiledParaPairs.Clear();
            sb_FiledParaPairs = null;
            sb_Sql.Clear();
            sb_Sql = null;

            this.SqlOutPutToLogAsync(sqlCmd, entity);

            using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
            {
                var result = conn.Execute(sqlCmd, entity, transaction);
                return result;
            }
        }


        #endregion

        #region Select   查询操作

        /// <summary>
        /// 通过主键获取单个元素
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public TElement GetElementById(long id)
        {


            TElement entity = new TElement();

            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, false,out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return null;
                throw new Exception("未指定除主键后其他字段！");
            }

            string splitor = string.Format("{0},{0}", this.FieldWrapperChar);
            var fieldSplitString = string.Concat(this.FieldWrapperChar, string.Join(splitor, filelds), this.FieldWrapperChar);//返回逗号分隔的字符串 例如：`ProvinceCode`,`ProvinceName`

            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("select  {0} ", fieldSplitString);
            sb_Sql.AppendFormat(" from {0} ", tableInDbName);
            sb_Sql.AppendFormat(" where {2}{0}{2}={1};", EntityIdentityFiledName,id,this.FieldWrapperChar);

            var sqlCmd = sb_Sql.ToString();

            sb_Sql.Clear();
            sb_Sql = null;

            this.SqlOutPutToLogAsync(sqlCmd, entity);

            try
            {
                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    entity = conn.QueryFirstOrDefault<TElement>(sqlCmd);
                }

                #region ADO.NET 的方式，性能比较
                //////reader = ExecuteReader(sqlCmd, parameters, CommandType.Text);
                //////reader.Read();
                //////entity = reader.ConvertDataReaderToEntity<TElement>();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
            return entity;


        }



        /// <summary>
        /// 通过特定的条件查询出元素集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TElement> GetElementsByCondition(Expression<Func<TElement, bool>> predicate)
        {
            TElement entity = new TElement();

            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, false,out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return null;
                throw new Exception("未指定除主键后其他字段！");
            }
            //获取字段
            string splitor = string.Format("{0},{0}", this.FieldWrapperChar);
            var fieldSplitString = string.Concat(this.FieldWrapperChar, string.Join(splitor, filelds), this.FieldWrapperChar);//返回逗号分隔的字符串 例如：`ProvinceCode`,`ProvinceName`
            //解析查询条件
            string whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate, wrapperChar: this.FieldWrapperChar);
            }



            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("select  {0} ", fieldSplitString);
            sb_Sql.AppendFormat(" from {0} ", tableInDbName);
            sb_Sql.AppendFormat(" where {0};", whereStr);


            var sqlCmd = sb_Sql.ToString();

            sb_Sql.Clear();
            sb_Sql = null;

            List<TElement> dataLst = null;
            try
            {
                this.SqlOutPutToLogAsync(sqlCmd);

                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    dataLst = conn.Query<TElement>(sqlCmd).AsList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
 
            return dataLst;
        }


        /// <summary>
        /// 分页获取元素集合-ORM
        /// 注意：mysql 的存储过程 ，调用，不能输出参数。BUG.即使关闭了datareader 也不能关闭 依然isclosed =false
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="rule">排序规则</param>
        /// <returns></returns>
        public List<TElement> GetElementsByPagerAndCondition(int pageIndex, int pageSize, out int totalRecords, out int totalPages, Expression<Func<TElement, bool>> predicate, string sortField, OrderRule rule = OrderRule.ASC)
        {
            List<TElement> dataLst = new List<TElement>();
            TElement entity = new TElement();

            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity,false, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                totalRecords = -1;
                totalPages = -1;
                //除主键后 没有其他字段
                return null;
                throw new Exception("未指定除主键后其他字段！");
            }
            //获取字段
            string splitor = string.Format("{0},{0}", this.FieldWrapperChar);
            var fieldSplitString = string.Concat(this.FieldWrapperChar, string.Join(splitor, filelds), this.FieldWrapperChar);//返回逗号分隔的字符串 例如：`ProvinceCode`,`ProvinceName`
            //解析查询条件
            var whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate, wrapperChar: this.FieldWrapperChar);
            }



            //调用分页存储过程
            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.Append(Contanst.PageSql_Call_Name);

            var sqlCmd = sb_Sql.ToString();

            var sqlParas =   new DynamicParameters();
            sqlParas.Add("@PageIndex", pageIndex);//页索引
            sqlParas.Add("@PageSize", pageSize);//页大小
            sqlParas.Add("@TableName", tableInDbName);//表名称
            sqlParas.Add("@SelectFields", fieldSplitString);//查询的字段
            sqlParas.Add("@PrimaryKey", EntityIdentityFiledName);//查询的表的主键
            sqlParas.Add("@ConditionWhere", whereStr);//查询条件      
            sqlParas.Add("@SortField", sortField);//排序字段
            sqlParas.Add("@IsDesc", (int)rule);//倒排序 正排序
            sqlParas.Add("@TotalRecords", DbType.Int32, direction: ParameterDirection.Output);//总记录数（可选参数）
            sqlParas.Add("@TotalPageCount", DbType.Int32, direction: ParameterDirection.Output);//总页数（输出参数


            try
            {
                this.SqlOutPutToLogAsync(sqlCmd, sqlParas);

                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    dataLst = conn.Query<TElement>(sqlCmd,sqlParas,commandType: CommandType.StoredProcedure).AsList();
                }

                //查询完毕后 根据输出参数 返回总记录数 总页数
                totalRecords = sqlParas.Get<int>("@TotalRecords");
                totalPages = sqlParas.Get<int>("@TotalPageCount");


            }
            catch (Exception ex)
            {
                //抛出Native 异常信息
                throw ex;
            }
            finally
            {
                //清理字符串构建
                sb_Sql.Clear();
                sb_Sql = null;
                propertys = null;
                filelds = null;
                paras = null;
            }


            return dataLst;
        }



        #endregion

        #region Delete 删除操作


        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Delete(TElement entity, IDbTransaction transaction = null)
        {

            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity,true, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return -1;
                throw new Exception("未指定除主键后其他字段！");
            }

            var primaryValue = ReflectionHelper.GetPropertyValue(entity, EntityIdentityFiledName); 

            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("delete from {0} ", tableInDbName);
            sb_Sql.AppendFormat(" where {0}={1};", EntityIdentityFiledName, primaryValue);
            

            var sqlCmd = sb_Sql.ToString();

            //清理构建器
            sb_Sql.Clear();
            sb_Sql = null;


            try
            {
                this.SqlOutPutToLogAsync(sqlCmd, entity);

                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    var result = conn.Execute(sqlCmd,null,transaction);
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        /// <summary>
        /// 删除符合条件的实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int DeleteByCondition(Expression<Func<TElement, bool>> predicate, IDbTransaction transaction = null)
        {
            TElement entity = new TElement();


            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, true, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return -1;
                throw new Exception("未指定除主键后其他字段！");
            }

            //解析查询条件
            var whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate, wrapperChar: this.FieldWrapperChar);
            }

            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("delete from {0} ", tableInDbName);
            if (null != predicate)
            {
                sb_Sql.AppendFormat("where  {0}  ", whereStr);
            }

            var sqlCmd = sb_Sql.ToString();
            try
            {
                this.SqlOutPutToLogAsync(sqlCmd);

                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    var result = conn.Execute(sqlCmd,null, transaction);
                    return result;
                }
              
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        #endregion


        #endregion


        #region Other Methods


        #endregion



        #region Disposable



        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {

            Dispose(true);
            // This class has no unmanaged resources but it is possible that somebody could add some in a subclass.
            GC.SuppressFinalize(this);

        }
        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放那些实现IDisposable接口的托管对象

            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }



        #endregion



    }

}
