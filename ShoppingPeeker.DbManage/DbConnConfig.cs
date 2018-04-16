using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPeeker.DbManage
{


    public class DbConnConfig
    {

        /// <summary>
        /// 连接字符串的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnString { get; set; }

        private string _ProviderName;
        /// <summary>
        /// 默认是 Sqlserver
        /// 支持 Sqlserver /Mysql
        /// </summary>
        public string ProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(_ProviderName))
                {
                    _ProviderName = SupportDbType.SQLSERVER.ToString();
                }
                return _ProviderName;
            }

            set
            {
                _ProviderName = value;
            }
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

       

        /// <summary>
        /// 数据库类型
        /// </summary>
        public SupportDbType DbType { get
            {
                if (string.IsNullOrEmpty(ProviderName))
                {
                    throw new Exception("请为连接字符串添加 providerName 配置。sqlserver or mysql?");
                }
                return (SupportDbType)Enum.Parse(typeof(SupportDbType), ProviderName.ToUpper());
            }
        }

        
    }
}
