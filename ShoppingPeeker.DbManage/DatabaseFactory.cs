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
                case SupportDbType.SQLSERVER:
                    conn = new SqlConnection(dbConnConfig.ConnString);
                    break;
                case SupportDbType.MYSQL:
                    conn = new MySqlConnection(dbConnConfig.ConnString);
                    break;
                case SupportDbType.POSTGRESQL:
                case SupportDbType.ORACLE:
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
                case SupportDbType.SQLSERVER:

                    cmd = new SqlCommand();
                    break;
                case SupportDbType.MYSQL:
                    cmd = new MySqlCommand();
                    break;
                case SupportDbType.POSTGRESQL:
                case SupportDbType.ORACLE:
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
                case SupportDbType.SQLSERVER:
                    dataAdapter = new SqlDataAdapter((SqlCommand)selectCommand);
                    break;
                case SupportDbType.MYSQL:
                    dataAdapter = new MySqlDataAdapter((MySqlCommand)selectCommand);
                    break;
                case SupportDbType.POSTGRESQL:
                case SupportDbType.ORACLE:
                default: throw new NotImplementedException();

            }




            return dataAdapter;

        }



        #endregion

    }
}
