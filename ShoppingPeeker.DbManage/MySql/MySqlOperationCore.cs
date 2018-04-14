using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using ShoppingPeeker.DbManage.Utilities;
using System.Data.Common;

namespace ShoppingPeeker.DbManage
{
    /// <summary>
    /// MYSQL 核心交互方法
    /// </summary>
    public class MySqlOperationCore<TElement> : BaseSqlOperation<TElement> where TElement : BaseEntity, new()
    {
        public MySqlOperationCore()
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

            using (MySqlConnection conn = new MySqlConnection(this.CurrentDBConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (MySqlCommand cmd = new MySqlCommand())
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

            MySqlConnection conn = new MySqlConnection(this.CurrentDBConnectionString);

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);//注意 使用reader 游标的形式 必须使用完毕后关闭连接

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

            using (MySqlConnection conn = new MySqlConnection(this.CurrentDBConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                MySqlCommand cmd = new MySqlCommand();

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
