using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;


using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ShoppingPeeker.Utilities.TypeFinder;
using ShoppingPeeker.Utilities;
using ShoppingPeeker.DbManage.Utilities;
using ShoppingPeeker.DbManage.CommandTree;




namespace ShoppingPeeker.DbManage
{


    /// <summary>
    /// 连接数据库的  上下文  用来执行与数据库进行交互
    /// </summary>
    public class SqlDbContext<TElement> :BaseSqlOperation<TElement>, IDbContext<TElement>, IDisposable
        where TElement : BaseEntity, new()
    {
        #region Construction and fields




        /// <summary>
        /// 实体的主键名称
        /// </summary>
        private string EntityIdentityFiledName = new TElement().GetIdentity().IdentityKeyName;



        /// <summary>
        /// 数据上下文 构造函数
        /// </summary>
        /// <param name="dbConfig"></param>
        public SqlDbContext(DbConnConfig dbConfig)
        {
            this.DbConfig = dbConfig;
        }



        #endregion


        #region Context methods


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


            SqlParameter[] parameters = new SqlParameter[noIdentityParas.Length];
            var settedValueDic = entity.GetSettedValuePropertyDic();
            for (int i = 0; i < noIdentityParas.Length; i++)
            {
                var colName = noIdentityParas[i];
                string key = noIdentityPropertys[i].Name;
                object value = null;//ReflectionHelper.GetPropertyValue(entity, noIdentityPropertys[i]);
                settedValueDic.TryGetValue(key, out value);
                var para = new SqlParameter(colName, value);
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
            var result = this.ExecuteScalar(sqlCmd, parameters);
            if (null != result)
            {
                return int.Parse(result.ToString());
            }
            return Error_Opeation_Result;
        }


        /// <summary>
        /// 单次批量多次插入多个实体
        /// (注意：sqlbuck插入，高效率sqlbuck方式插入)
        /// </summary>
        /// <param name="entities"></param>
        public bool InsertMulitiEntities(IEnumerable<TElement> entities)
        {
            var result = -1;


            var count_entities = entities.Count();
            if (count_entities <= 0)
            {
                return false;
            }


            string tableInDbName;
            System.Reflection.PropertyInfo[] propertys;
            string[] filelds;
            string[] paras;
            ResolveEntity(entities.First(), out tableInDbName, out propertys, out filelds, out paras);

            try
            {

                ///不含主键的属性
                var noIdentityPropertys = propertys.Remove(x => x.Name == EntityIdentityFiledName);


                using (var bulk = new SqlBulkCopy(this.DbConfig.ConnString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    bulk.BulkCopyTimeout = 60;//命令超时时间
                    //bulk.BatchSize = 1000;
                    //指定写入的目标表
                    bulk.DestinationTableName = tableInDbName;
                    //数据源中的列名与目标表的属性的映射关系
                    //bulk.ColumnMappings.Add("ip", "ip");
                    //bulk.ColumnMappings.Add("port", "port");
                    //bulk.ColumnMappings.Add("proto_name", "proto_name");
                    //bulk.ColumnMappings.Add("strategy_id", "strategy_id");
                    //init mapping
                    foreach (var pi in noIdentityPropertys)
                    {
                        bulk.ColumnMappings.Add(pi.Name, pi.Name);
                    }

                    DataTable dt = SqlDataTableExtensions.ConvertListToDataTable<TElement>(entities, ref noIdentityPropertys);//数据源数据

                    DbDataReader reader = dt.CreateDataReader();
                    bulk.WriteToServer(dt);
                }


                result = 1;

            }
            catch (Exception ex)
            {

                //抛出Native 异常信息
                throw ex;
            }


            var isSuccess = result > 0 ? true : false;


            return isSuccess;


        }

        #endregion


        #region Update 更新操作
        /// <summary>
        /// 更新单个模型
        /// （更新机制为，模型载体设置的值的字段会被更新掉，不设置值 不更新）
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
            SqlParameter[] parameters = new SqlParameter[settedValueDic.Count];
            int counter = 0;
            foreach (var item in settedValueDic)
            {
                var keyProperty = item.Key;
                var value = item.Value;
                var paraName = string.Format("@{0}", keyProperty);
                var Parameter = new SqlParameter(paraName, value);
                Parameter.IsNullable = true;
                parameters[counter] = Parameter;


                counter++;
            }

            #region 废弃代码


            //for (int i = 0; i < paras.Length; i++)
            //{
            //    var Parameter = new SqlParameter(paras[i], DbTypeAndCLRType.ConvertClrTypeToDbType(propertys[i].GetType()));
            //    Parameter.Value = propertys[i].GetValue(entity, null);
            //    Parameter.IsNullable = true;
            //    parameters[i] = Parameter;
            //}

            //SqlParameter[] parameters = {
            //        new SqlParameter("@ProvinceCode", SqlDbType.NVarChar,15),
            //        new SqlParameter("@ProvinceName", SqlDbType.NVarChar,50),
            //        new SqlParameter("@Submmary", SqlDbType.Text),
            //        new SqlParameter("@ID", SqlDbType.Int,4)};
            //parameters[0].Value = model.ProvinceCode;
            //parameters[1].Value = model.ProvinceName;
            //parameters[2].Value = model.Submmary;
            //parameters[3].Value = model.ID;
            #endregion

            var sqlCmd = sb_Sql.ToString();
            ///清理掉字符串拼接构造器
            sb_FiledParaPairs.Clear();
            sb_FiledParaPairs = null;
            sb_Sql.Clear();
            sb_Sql = null;
            return ExecuteNonQuery(sqlCmd, parameters);
        }

        /// <summary>
        /// 更新元素 通过  符合条件的
        /// （更新机制为，模型载体设置的值的字段会被更新掉，不设置值 不更新）
        /// </summary>
        /// <param name="entity"></param>
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




            SqlParameter[] parameters = new SqlParameter[settedValueDic.Count];
            int counter = 0;
            foreach (var item in settedValueDic)
            {
                var keyProperty = item.Key;
                var value = item.Value;
                var paraName = string.Format("@{0}", keyProperty);
                var Parameter = new SqlParameter(paraName, value);
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


            return ExecuteNonQuery(sqlCmd, parameters);
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
            sb_Sql.AppendFormat("select {0} ", fieldSplitString);
            sb_Sql.AppendFormat(" from {0} ", tableInDbName);//WITH (NOLOCK) 由于不锁定表执行的事务锁-会有数据脏读
            sb_Sql.AppendFormat(" where {0}=@{0};", EntityIdentityFiledName);
            SqlParameter[] parameters = {
                        new SqlParameter()
            };
            parameters[0].ParameterName = string.Format("@{0)", EntityIdentityFiledName);
            parameters[0].Value = id;

            var sqlCmd = sb_Sql.ToString();

            sb_Sql.Clear();
            sb_Sql = null;
            System.Data.Common.DbDataReader reader = null;
            try
            {
                reader = ExecuteReader(sqlCmd, parameters);
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
                reader = ExecuteReader(sqlCmd, null);
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
        /// 分页获取元素集合
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortField">排序字段（如果不指定排序字段 那么默认按照id 排序）</param>
        /// <param name="rule">排序规则</param>
        /// <returns></returns>
        public List<TElement> GetElementsByPagerAndCondition(int pageIndex, int pageSize, out int totalRecords, out int totalPages, Expression<Func<TElement, bool>> predicate, string sortField = null, OrderRule rule = OrderRule.ASC)
        {
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



            StringBuilder sb_Sql = new StringBuilder("EXEC ");
            sb_Sql.Append(Contanst.PageSql_Call_Name);

            SqlParameter[] parameters = {
                    new SqlParameter("@PageIndex",pageIndex),//页索引
                    new SqlParameter("@PageSize", pageSize),//页大小
                    new SqlParameter("@TableName", tableInDbName),//表名称
                    new SqlParameter("@SelectFields", fieldSplitString),//查询的字段
                     new SqlParameter("@ConditionWhere", whereStr), //查询条件      
                    new SqlParameter("@SortField", sortField?? EntityIdentityFiledName),//排序字段
                    new SqlParameter("@IsDesc",rule == OrderRule.ASC ? 0 : 1),//倒排序 正排序
                    new SqlParameter("@TotalRecords",0),//总记录数（可选参数）
                    new SqlParameter("@TotalPageCount",0)//总页数（输出参数）
                                        };
            //parameters[0].Value = pageIndex;
            //parameters[1].Value = pageSize;
            //parameters[2].Value = tableInDbName;
            //parameters[3].Value = fieldSplitString;
            //parameters[4].Value = whereStr;
            //parameters[5].Value = sortField?? EntityIdentityFiledName;
            //parameters[6].Value = rule == OrderRule.ASC ? 0 : 1;
            parameters[7].Direction = ParameterDirection.Output;
            parameters[8].Direction = ParameterDirection.Output;
            var sql = sb_Sql.ToString();
            //清理字符串构建
            sb_Sql.Clear();
            sb_Sql = null;

            try
            {
                var lst_PagedData = ExecuteStoredProcedureList(sql, parameters);
                //查询完毕后 根据输出参数 返回总记录数 总页数
                totalRecords = Convert.ToInt32(parameters[7].Value);
                totalPages = Convert.ToInt32(parameters[8].Value);

                return lst_PagedData;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        #endregion


        #region Delete 删除操作

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="?"></param>
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
            sb_Sql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", entity.GetIdentityValue())
            };

            var sqlCmd = sb_Sql.ToString();
            //清理构建器
            sb_Sql.Clear();
            sb_Sql = null;

            try
            {
                return Convert.ToInt32(ExecuteNonQuery(sqlCmd, parameters));

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
                return Convert.ToInt32(ExecuteNonQuery(sqlCmd, null));

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }



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


        #endregion

    }
}
