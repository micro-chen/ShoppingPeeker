
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
        /// 数据上下文构造函数
        /// </summary>
        /// <param name="dbConfig"></param>
        public MySqlDbContext(DbConnConfig dbConfig)
        {
            this.DbConfig = dbConfig;
        }


        //public MysqlDbContext(string connString)
        //{
        //    Check.NotEmpty(connString, "User Custom DBConnectionString");
        //    this._CurrentDBConnectionString = connString;
        //}

        #endregion



        #region 对提供映射字段配置的数据，进行ORM对象获取



        #region  Insert操作
        /// <summary>
        /// 插入 实体
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(TElement entity)
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

            var fieldSplitString = String.Join(",", noIdentityFileds);//返回逗号分隔的字符串 例如：ProvinceCode,ProvinceName,Submmary
            var parasSplitString = String.Join(",", noIdentityParas);//参数   数组 的逗号分隔


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

            using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
            {
                var result = conn.ExecuteScalar<int>(sqlCmd, entity);
                return result;
            }
             
        }


        /// <summary>
        /// 单次批量多次插入多个实体,并返回执行的记录数目---参数化的形式
        /// 不提供db事务  请使用代码事务 using(var tran=new TranstionScope()){ your  code.......}
        /// </summary>
        /// <param name="entities">实体集合</param>
        public bool InsertMulitiEntities(IEnumerable<TElement> entities)
        {

            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entities.First(), true, out tableInDbName, out propertys, out filelds, out paras);

            using (var bcp = new MysqlBuckCopy<TElement>(this.DbConfig.ConnString))
            {
                return bcp.WriteToServer(entities, tableInDbName);
            }


        }


        #endregion


        #region Update 更新操作

        /// <summary>
        /// 更新单个模型
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(TElement entity)
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
                    sb_FiledParaPairs.AppendFormat("{0}=@{0},", keyProperty);
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
            using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
            {
                var result = conn.Execute(sqlCmd, entity);
                return result;
            }
         
        }




        /// <summary>
        /// 更新元素 通过  符合条件的
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="entity"></param>
        /// <param name="hopeUpdateColumns"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int UpdateByCondition(TElement entity, Expression<Func<TElement, bool>> predicate)
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
                    sb_FiledParaPairs.AppendFormat("{0}=@{0},", keyProperty);
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
                string where = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
                sb_Sql.Append(" where ");//解析条件
                sb_Sql.Append(where);//条件中带有参数=值的  拼接字符串
            }


            var sqlCmd = sb_Sql.ToString();

            ///清理字符串构建
            sb_FiledParaPairs.Clear();
            sb_FiledParaPairs = null;
            sb_Sql.Clear();
            sb_Sql = null;


            using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
            {
                var result = conn.Execute(sqlCmd, entity);
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
            var fieldSplitString = String.Join(",", filelds);//返回逗号分隔的字符串 例如：ProvinceCode,ProvinceName,Submmary

            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("select  {0} ", fieldSplitString);
            sb_Sql.AppendFormat(" from {0} ", tableInDbName);
            sb_Sql.AppendFormat(" where {0}={1};", EntityIdentityFiledName,id);

            var sqlCmd = sb_Sql.ToString();

            sb_Sql.Clear();
            sb_Sql = null;

            try
            {
                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    entity = conn.QueryFirstOrDefault<TElement>(sqlCmd);
                }
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
            var fieldSplitString = String.Join(",", filelds);//返回逗号分隔的字符串 例如：ProvinceCode,ProvinceName,Submmary
            //解析查询条件
            string whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
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
            var fieldSplitString = String.Join(",", filelds);//返回逗号分隔的字符串 例如：ProvinceCode,ProvinceName,Submmary
            //解析查询条件
            var whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
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
        /// <returns></returns>
        public int Delete(TElement entity)
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
                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    var result = conn.Execute(sqlCmd);
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
        public int DeleteByCondition(Expression<Func<TElement, bool>> predicate)
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
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
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
                using (var conn = DatabaseFactory.GetDbConnection(this.DbConfig))
                {
                    var result = conn.Execute(sqlCmd);
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
