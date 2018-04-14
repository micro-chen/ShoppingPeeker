
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;

using ShoppingPeeker.DbManage.Utilities;
using ShoppingPeeker.DbManage.CommandTree;
using ShoppingPeeker.Utilities.TypeFinder;
using ShoppingPeeker.Utilities;

namespace ShoppingPeeker.DbManage
{
    public class MysqlDbContext<TElement> : MySqlOperationCore<TElement>, IDbContext<TElement>, IDisposable
        where TElement : BaseEntity, new()
    {



        #region Construction and fields



        private string _CurrentDBConnectionString;
        // 数据库连接字符串--覆盖基类中的属性
        public override string CurrentDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(this._CurrentDBConnectionString))
                {
                    this._CurrentDBConnectionString = GlobalDBConnection.DBConnectionString;
                }
                return this._CurrentDBConnectionString;
            }
            set
            {
                this._CurrentDBConnectionString = value;
            }
        }

        /// <summary>
        /// 实体的主键名称
        /// </summary>
        private string EntityIdentityFiledName = new TElement().GetIdentity().IdentityKeyName;


        /// <summary>
        /// 继承类-采用默认全局的数据库连接字符串
        /// </summary>
        public MysqlDbContext()
        {
            Check.NotEmpty(GlobalDBConnection.DBConnectionString, "GlobalDBConnection.DBConnectionString");
        }


        public MysqlDbContext(string connString)
        {
            Check.NotEmpty(connString, "User Custom DBConnectionString");
            this._CurrentDBConnectionString = connString;
        }

        #endregion



        #region 对提供映射字段配置的数据，进行ORM对象获取



        #region  Insert操作
        /// <summary>
        /// 插入 实体
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public long Insert(TElement entity)
        {
            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);

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


            MySqlParameter[] parameters = new MySqlParameter[noIdentityParas.Length];

            var settedValueDic = entity.GetSettedValuePropertyDic();
            for (int i = 0; i < noIdentityParas.Length; i++)
            {
                var colName = noIdentityParas[i];
                string key = noIdentityPropertys[i].Name;
                object value = null;//ReflectionHelper.GetPropertyValue(entity, noIdentityPropertys[i]);
                settedValueDic.TryGetValue(key, out value);
                var para = new MySqlParameter(colName, value);

                para.IsNullable = true;

                parameters[i] = para;
            }


            //例子：以上代码  代替下面的代码
            //{
            //        new SqlParameter("@ProvinceCode", SqlDbType.NVarChar,15),
            //        new SqlParameter("@ProvinceName", SqlDbType.NVarChar,50),
            //        new SqlParameter("@Submmary", SqlDbType.Text)};
            //parameters[0].Value = model.ProvinceCode;
            //parameters[1].Value = model.ProvinceName;
            //parameters[2].Value = model.Submmary;

            var sqlCmd = sb_Sql.ToString();


            ///清理掉字符串拼接构造器
            sb_Sql.Clear();
            sb_Sql = null;
            var result = this.ExecuteScalar(sqlCmd, CommandType.Text, parameters);
            if (null != result)
            {
                return long.Parse(result.ToString());
            }
            return Error_Opeation_Result;
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
            ResolveEntity(entities.First(), out tableInDbName, out propertys, out filelds, out paras);

            using (var bcp = new MysqlBuckCopy<TElement>(this.CurrentDBConnectionString))
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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return -1;
                throw new Exception("未指定除主键后其他字段！");
            }

            StringBuilder sb_FiledParaPairs = new StringBuilder("");

            //---废弃全更新，改造为设定值的更新，防止擦除字段---------
            //for (int i = 0; i < filelds.Length; i++)
            //{
            //    if (filelds[i] != EntityIdentityFiledName)
            //    {
            //        sb_FiledParaPairs.AppendFormat("{0}=@{0},", filelds[i]);
            //    }
            //}
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

            //sb_Sql.Append("ProvinceCode=@ProvinceCode,");
            //sb_Sql.Append("ProvinceName=@ProvinceName,");
            //sb_Sql.Append("Submmary=@Submmary");

            sb_Sql.AppendFormat(" where {0}=@{0}", EntityIdentityFiledName);//主键

            //设定参数值--------(字段一一映射)
            MySqlParameter[] parameters = new MySqlParameter[settedValueDic.Count];
            int counter = 0;
            foreach (var item in settedValueDic)
            {
                var keyProperty = item.Key;
                var value = item.Value;
                var paraName = string.Format("@{0}", keyProperty);
                var Parameter = new MySqlParameter(paraName, value);
                Parameter.IsNullable = true;
                parameters[counter] = Parameter;


                counter++;
            }



            var sqlCmd = sb_Sql.ToString();
            ///清理掉字符串拼接构造器
            sb_FiledParaPairs.Clear();
            sb_FiledParaPairs = null;
            sb_Sql.Clear();
            sb_Sql = null;
            return ExecuteNonQuery(sqlCmd, CommandType.Text, parameters);
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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
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




            MySqlParameter[] parameters = new MySqlParameter[settedValueDic.Count];
            int counter = 0;
            foreach (var item in settedValueDic)
            {
                var keyProperty = item.Key;
                var value = item.Value;
                var paraName = string.Format("@{0}", keyProperty);
                var Parameter = new MySqlParameter(paraName, value);
                Parameter.IsNullable = true;
                parameters[counter] = Parameter;


                counter++;
            }


            var sqlCmd = sb_Sql.ToString();

            ///清理字符串构建
            sb_FiledParaPairs.Clear();
            sb_FiledParaPairs = null;
            sb_Sql.Clear();
            sb_Sql = null;


            return ExecuteNonQuery(sqlCmd, CommandType.Text, parameters);
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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
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
            sb_Sql.AppendFormat(" where {0}=@{0};", EntityIdentityFiledName);
            MySqlParameter[] parameters = {
                    new MySqlParameter()
            };
            parameters[0].ParameterName = string.Format("@{0)", EntityIdentityFiledName);
            parameters[0].Value = id;

            var sqlCmd = sb_Sql.ToString();

            sb_Sql.Clear();
            sb_Sql = null;

            System.Data.Common.DbDataReader reader = null;

            try
            {
                reader = ExecuteReader(sqlCmd, CommandType.Text, parameters);
                reader.Read();
                entity = reader.ConvertDataReaderToEntity<TElement>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //释放读取器
                if (null != reader)
                {
                    reader.Close();
                    reader.Dispose();
                }

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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
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

            System.Data.Common.DbDataReader reader = null;
            //解析DataReader  获取数据集合
            List<TElement> dataLst = new List<TElement>();

            try
            {
                reader = ExecuteReader(sqlCmd, CommandType.Text, null);
                if (null == reader)
                {
                    return null;
                }

                while (reader.Read())
                {
                    var model = reader.ConvertDataReaderToEntity<TElement>();
                    dataLst.Add(model);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (null != reader)
                {
                    reader.Close();
                    reader.Dispose();
                }
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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
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



            //调用分页存储过程--- 注意：7.0.7版本的驱动有bug 不能成功调用存储过程
            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.Append(Contanst.PageSql_Call_Name);


            MySqlParameter[] parameters = {
                    new MySqlParameter("@PageIndex",MySqlDbType.Int32,4 ),//页索引
                    new MySqlParameter("@PageSize", MySqlDbType.Int32,4),//页大小
                    new MySqlParameter("@TableName", MySqlDbType.VarChar,3000),//表名称
                    new MySqlParameter("@SelectFields", MySqlDbType.Text),//查询的字段
                    new MySqlParameter("@PrimaryKey", MySqlDbType.VarChar,50),//查询的表的主键
                     new MySqlParameter("@ConditionWhere", MySqlDbType.Text), //查询条件      
                    new MySqlParameter("@SortField", MySqlDbType.VarChar,50),//排序字段
                    new MySqlParameter("@IsDesc", MySqlDbType.Int32,4),//倒排序 正排序
                    new MySqlParameter("@TotalRecords", MySqlDbType.Int32,4),//总记录数（可选参数）
                    new MySqlParameter("@TotalPageCount", MySqlDbType.Int32,4)//总页数（输出参数）
                                        };
            parameters[0].Value = pageIndex;
            parameters[1].Value = pageSize;
            parameters[2].Value = tableInDbName;
            parameters[3].Value = fieldSplitString;
            parameters[4].Value = EntityIdentityFiledName;
            parameters[5].Value = whereStr;
            parameters[6].Value = sortField;// String.Join(",", sortField.ContainerFileds.ToArray());
            parameters[7].Value = (int)rule;//== OrderRule.ASC @ 0 : 1;
            // parameters[8].Value = TotalRecords;
            parameters[8].Direction = ParameterDirection.Output;
            // parameters[9].Value = TotalPageCount;
            parameters[9].Direction = ParameterDirection.Output;
            var sqlCmd = sb_Sql.ToString();


            try
            {

                dataLst = this.ExecuteStoredProcedureList(sqlCmd, parameters);
                //查询完毕后 根据输出参数 返回总记录数 总页数
                totalRecords = Convert.ToInt32(parameters[8].Value);
                totalPages = Convert.ToInt32(parameters[9].Value);


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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
            if (filelds.Length <= 1)
            {
                //除主键后 没有其他字段
                return -1;
                throw new Exception("未指定除主键后其他字段！");
            }


            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("delete from {0} ", tableInDbName);
            sb_Sql.AppendFormat(" where {0}=@{0};", EntityIdentityFiledName);
            MySqlParameter[] parameters = {
                    new MySqlParameter()
            };
            parameters[0].ParameterName = string.Format("@{0)", EntityIdentityFiledName);
            parameters[0].Value = ReflectionHelper.GetPropertyValue(entity, EntityIdentityFiledName);

            var sqlCmd = sb_Sql.ToString();

            //清理构建器
            sb_Sql.Clear();
            sb_Sql = null;

            try
            {
                return Convert.ToInt32(ExecuteNonQuery(sqlCmd, CommandType.Text, parameters));

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
            ResolveEntity(entity, out tableInDbName, out propertys, out filelds, out paras);
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
                return Convert.ToInt32(ExecuteNonQuery(sqlCmd, CommandType.Text));

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

            //置空连接字符串
            this.CurrentDBConnectionString = null;
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
