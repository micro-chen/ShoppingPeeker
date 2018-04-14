
using System;
using System.Configuration;
using System.Threading.Tasks;


namespace ShoppingPeeker.DbManage
{
    /// <summary>
    /// 设置数据库连接
    /// </summary>
    public class InitDatabase
    {
        public static void SetDatabaseConnection(string connStr, string providerName)
        {
            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException("连接字符串设置不能为空！");
            }

            //默认为SqlServer数据库
            if (string.IsNullOrEmpty(providerName))
            {
                providerName = SupportDbType.Sqlserver.ToString();
            }

            GlobalDBConnection.CurrentDbType = (SupportDbType)Enum.Parse(typeof(SupportDbType), providerName);

            GlobalDBConnection.DBConnectionString = connStr;


        }
    }
}