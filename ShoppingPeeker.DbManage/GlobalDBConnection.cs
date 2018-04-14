using System;
using ShoppingPeeker.DbManage.Utilities;
using System.Threading.Tasks;

namespace ShoppingPeeker.DbManage
{
    /// <summary>
    /// 使用此全局数据库连接字符串定义与数据库进行交互 的数据连接，
    /// 在整个应用程序生命周期都使用此字符串
    /// </summary>
    public static class GlobalDBConnection
    {


        public static DbState DataBaseState { get; set; }

        /// <summary>
        /// 当前选择的数据库
        /// </summary>
        public static SupportDbType CurrentDbType { get; set; }


        private static string _DBConnectionString;
        /// <summary>
        /// 全局数据库连接字符串
        /// </summary>
        public static string DBConnectionString
        {
            get
            {
                return _DBConnectionString;
            }
            set
            {

                _DBConnectionString = value;


                //设置好连接字符串后  初始化数据库  
                InitDataBase();
            }
        }

        private static void InitDataBase()
        {


            //异步初始化
            Task.Factory.StartNew(() =>
            {
                try
                {

                    switch (GlobalDBConnection.CurrentDbType)
                    {
                        case SupportDbType.Sqlserver:
                            //1 创建必须的分页存储过程等全局操作
                            PagerSQLProcedure.CheckAndCreatePagerSQLProcedure();
                            break;
                        case SupportDbType.Mysql:
                            //1 创建必须的分页存储过程等全局操作
                            MysqlPagerSQLProcedure.CheckAndCreatePagerSQLProcedure();
                            break;
                        case SupportDbType.PostgreSQL:

                            break;
                        case SupportDbType.Oracle:

                            break;
                        default:
                            throw new NotImplementedException();

                    }

                    DataBaseState = DbState.Opened;
                    //2 Other Need Initial Operations....

                }
                catch (Exception ex)
                {
                    DataBaseState = DbState.Closed;
                    throw ex;
                }


            }).ConfigureAwait(false).GetAwaiter();

          


        }
    }
}
