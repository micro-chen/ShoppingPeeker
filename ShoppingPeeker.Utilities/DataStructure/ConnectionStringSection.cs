using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingPeeker.Utilities.DataStructure
{
    /// <summary>
    /// 配置文件中的数据库连接字符串区域
    /// </summary>
    public class ConnectionStringSection
    {
        /// <summary>
        /// 配置节点名称
        /// </summary>
        public const string SectionName = "ConnectionStringSection";

        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
    }
}
