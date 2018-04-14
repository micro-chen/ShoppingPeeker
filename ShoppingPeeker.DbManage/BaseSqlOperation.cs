using ShoppingPeeker.DbManage.CommandTree;
using ShoppingPeeker.DbManage.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPeeker.DbManage
{
   public abstract class BaseSqlOperation<TElement> where TElement : BaseEntity, new()
    {


        public BaseSqlOperation()
        {
      
        }

        /// <summary>
        /// 数据库连接字符串-虚属性
        /// </summary>
        public virtual string CurrentDBConnectionString { get; set; }

        /// <summary>
        /// 错误的数据操作结果标志
        /// </summary>
        public const long Error_Opeation_Result = -1;


        public abstract DbDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters);

        public abstract object ExecuteScalar(string cmdText, CommandType cmdType, params DbParameter[] commandParameters);
        public abstract int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters);



        #region  聚合函数实现  SUM COUNT  MAX  MIN

        /// <summary>
        /// 使用指定的条件 汇总列
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="specialColumn"></param>
        /// <returns></returns>
        public int Sum(Expression<Func<TElement, bool>> predicate, Fields<TElement> specialColumn)
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
                throw new Exception("未指定除主键后其他字段！");
            }

            //解析查询条件
            string whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
            }



            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("SELECT SUM({0}) FROM  {1} ", specialColumn.Container_Fileds.FirstOrDefault(), tableInDbName);
            if (!string.IsNullOrEmpty(whereStr))
            {
                sb_Sql.AppendFormat("WHERE {0} ", whereStr);
            }


            var sqlCmd = sb_Sql.ToString();

            //清理字符串构建
            sb_Sql.Clear();
            sb_Sql = null;
            try
            {
                return Convert.ToInt32(ExecuteScalar(sqlCmd, CommandType.Text));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 统计 符合条件的行数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Count(Expression<Func<TElement, bool>> predicate)
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
                throw new Exception("未指定除主键后其他字段！");
            }

            //解析查询条件
            string whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
            }


            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("SELECT COUNT(*) FROM  {0} ", tableInDbName);
            if (!string.IsNullOrEmpty(whereStr))
            {
                sb_Sql.AppendFormat("WHERE {0} ", whereStr);
            }

            var sqlCmd = sb_Sql.ToString();

            //清理字符串构建
            sb_Sql.Clear();
            sb_Sql = null;
            try
            {
                return Convert.ToInt32(ExecuteScalar(sqlCmd, CommandType.Text));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 符合条件的行的 指定列的最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public int Max(Expression<Func<TElement, bool>> predicate, Fields<TElement> specialColumn)
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
                throw new Exception("未指定除主键后其他字段！");
            }

            //解析查询条件
            string whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
            }


            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("SELECT MAX({0}) FROM  {1} ", specialColumn.Container_Fileds.FirstOrDefault(), tableInDbName);
            if (!string.IsNullOrEmpty(whereStr))
            {
                sb_Sql.AppendFormat("WHERE {0} ", whereStr);
            }

            var sqlCmd = sb_Sql.ToString();

            //清理字符串构建
            sb_Sql.Clear();
            sb_Sql = null;
            try
            {
                return Convert.ToInt32(ExecuteScalar(sqlCmd, CommandType.Text));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 符合条件的行的 指定列的最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="specialColumn"></param>
        /// <returns></returns>
        public int Min(Expression<Func<TElement, bool>> predicate, Fields<TElement> specialColumn)
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
                throw new Exception("未指定除主键后其他字段！");
            }

            //解析查询条件
            string whereStr = "1=1";
            if (null != predicate)
            {
                whereStr = ResolveLambdaTreeToCondition.ConvertLambdaToCondition<TElement>(predicate);
            }


            StringBuilder sb_Sql = new StringBuilder();
            sb_Sql.AppendFormat("SELECT MIN({0}) FROM  {1} ", specialColumn.Container_Fileds.FirstOrDefault(), tableInDbName);
            if (!string.IsNullOrEmpty(whereStr))
            {
                sb_Sql.AppendFormat("WHERE {0} ", whereStr);
            }

            var sqlCmd = sb_Sql.ToString();

            //清理字符串构建
            sb_Sql.Clear();
            sb_Sql = null;
            try
            {
                return Convert.ToInt32(ExecuteScalar(sqlCmd, CommandType.Text));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


        /// <summary>
        ///  执行一个SQL查询语句，进行参数化查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<TElement> SqlQuery(string commandText, CommandType commandType, params DbParameter[] parameters)
        {

            var dataLst = new List<TElement>();

            //获取返回的结果 作为DataTable  解析其中的Row 到特定的实体
            DbDataReader reader = null;
            try
            {
                reader = this.ExecuteReader(commandText, commandType, parameters);
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
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        public List<TElement> ExecuteStoredProcedureList(string commandText, params DbParameter[] parameters)
        {
            //获取返回的结果 作为DataTable  解析其中的Row 到特定的实体
            var dataList = this.SqlQuery(commandText, CommandType.StoredProcedure, parameters);
            return dataList;
        }



        /// <summary>
        /// 检测是否是参数化查询SQL
        /// </summary>
        /// <param name="conn">基于的数据库连接</param>
        /// <param name="inputSql">执行的SQL  语句</param>
        ///<param name = "cmdParms" > 执行的sql 参数/param>
        /// <returns></returns>
        private bool IsValidParamedSqlQuery(DbConnection conn, string inputSql, IEnumerable<DbParameter> cmdParms)
        {
            var result = false;
        //    MySqlConnectionStringBuilder cb = new MySqlConnectionStringBuilder(
        //conn.ConnectionString);
        //    bool sqlServerMode = cb.SqlServerMode;

            SqlStatementTokenizer statementTokenizer = new SqlStatementTokenizer(inputSql);
            statementTokenizer.ReturnComments = true;
            statementTokenizer.SqlServerMode = true;

            var isParamSql = statementTokenizer.IsParamedSql();
            if (isParamSql == false)
            {
                //非参数话的查询 直接返回true
                return true;
            }


            //如果是参数话的查询 那么检测参数
            //基于参数的查询，匹配是否有参数

            if (cmdParms == null || cmdParms.Count() <= 0)
            {
                throw new Exception("基于参数化查询的SQL命令，请必须提供参数！");
            }


            //通过了检测 
            result = true;
            statementTokenizer = null;

            return result;
        }



        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">DbCommand object</param>
        /// <param name="conn">DbConnection object</param>
        /// <param name="trans">DbTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">DbParameter to use in the command</param>
        protected void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType, string cmdText, IEnumerable<DbParameter> cmdParms)
        {




            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;


            //检测本次查询的有效性，如果是参数查询，但是不提供参数，那么抛出异常信息
            if (!IsValidParamedSqlQuery(conn, cmdText, cmdParms))
            {
                throw new ArgumentException("非法的参数化查询！请检测SQL的正确性！");
            }

            if (cmdParms != null)
            {
                foreach (var parm in cmdParms)
                {
                    if (parm.Value == null)
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }


            }
        }


        /// <summary>
        /// 解析实体   解析其中的关联的表+字段+字段参数
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tableInDbName"></param>
        /// <param name="propertys"></param>
        /// <param name="filelds"></param>
        /// <param name="paras"></param>
        protected  void ResolveEntity(TElement entity, out string tableInDbName, out System.Reflection.PropertyInfo[] propertys, out string[] filelds, out string[] paras)
        {
            tableInDbName = "";
            var targetAttributes = entity.GetType().GetCustomAttributes(typeof(TableAttribute), false);
            if (null == targetAttributes)
            {
                throw new Exception("the model class has not mapping table!");
            }
            tableInDbName = (targetAttributes[0] as TableAttribute).Name;

            //获取所有字段
            propertys = entity.GetCurrentEntityProperties();//entity.CurrentModelPropertys;// entity.GetType().GetProperties();
            filelds = new string[propertys.Length];
            for (int i = 0; i < propertys.Length; i++)
            {
                filelds[i] = propertys[i].Name;
            }
            //参数字段
            paras = filelds.Clone() as string[];
            for (int i = 0; i < paras.Length; i++)
            {
                paras[i] = "@" + paras[i];
            }
        }
    }
}
