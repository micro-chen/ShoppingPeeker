using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ShoppingPeeker.DbManage
{
    /// <summary>
    /// SQLSERVER 核心交互方法
    /// </summary>
    public class SQLOperationCore<TElement> : BaseSqlOperation<TElement> where TElement : BaseEntity, new()
    {
        public SQLOperationCore()
        {

        }

        #region  SQL辅助相关


        /// <summary>
        /// 执行命令 并返回影响行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters)
        {

            using (SqlConnection conn = new SqlConnection(this.CurrentDBConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (SqlCommand cmd = new SqlCommand())
                {

                    int val = -1;
                    try
                    {
                        PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                        val = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return val;
                }

            }


        }



        /// <summary>
        /// 执行命令  并返回一个可读的Reader
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="trans"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public override DbDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters)
        {

            SqlConnection conn = new SqlConnection(this.CurrentDBConnectionString);

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                return rdr;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }



        /// <summary>
        ///执行命令 并返回首列结果
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="trans"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public override object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters)
        {
            object val = null;

            using (SqlConnection conn = new SqlConnection(this.CurrentDBConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SqlCommand cmd = new SqlCommand();

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                try
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
            }

            return val;
        }


        #endregion



    }
}
