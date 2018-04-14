using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace ShoppingPeeker.DbManage
{
    internal static class DatabaseFactory
    {
        #region Db 交互实例 工厂


        /// <summary>
        /// 获取数据连接
        /// </summary>
        /// <returns></returns>
        public static DbConnection GetDbConnection(DbConnConfig dbConnConfig)
        {
            DbConnection conn = null;
            if (null == dbConnConfig)
            {
                throw new Exception("置数据库连接配置不能为空！");

            }

            switch (dbConnConfig.DbType)
            {
                case SupportDbType.Sqlserver:
                    conn = new SqlConnection(dbConnConfig.ConnString);
                    break;
                case SupportDbType.Mysql:
                    conn = new MySqlConnection(dbConnConfig.ConnString);
                    break;
                case SupportDbType.PostgreSQL:
                case SupportDbType.Oracle:
                default: throw new NotImplementedException();
            }


            return conn;

        }

        /// <summary>
        /// 获取 DbCommand
        /// </summary>
        /// <returns></returns>
        public static DbCommand GetDbDbCommand(DbConnConfig dbConnConfig)
        {
            DbCommand cmd = null;
            if (null == dbConnConfig)
            {
                throw new Exception("置数据库连接配置不能为空！");

            }

            switch (dbConnConfig.DbType)
            {
                case SupportDbType.Sqlserver:

                    cmd = new SqlCommand();
                    break;
                case SupportDbType.Mysql:
                    cmd = new MySqlCommand();
                    break;
                case SupportDbType.PostgreSQL:
                case SupportDbType.Oracle:
                default: throw new NotImplementedException();


            }



            return cmd;
        }
        /// <summary>
        /// 获取一个 DataAdapter
        /// </summary>
        /// <returns></returns>
        public static DbDataAdapter GetDbDataAdapter(DbCommand selectCommand, DbConnConfig dbConnConfig)
        {
            DbDataAdapter dataAdapter = null;
            if (null == dbConnConfig)
            {
                throw new Exception("置数据库连接配置不能为空！");

            }

            switch (dbConnConfig.DbType)
            {
                case SupportDbType.Sqlserver:
                    dataAdapter = new SqlDataAdapter((SqlCommand)selectCommand);
                    break;
                case SupportDbType.Mysql:
                    dataAdapter = new MySqlDataAdapter((MySqlCommand)selectCommand);
                    break;
                case SupportDbType.PostgreSQL:
                case SupportDbType.Oracle:
                default: throw new NotImplementedException();

            }




            return dataAdapter;

        }



        #endregion

    }
}
