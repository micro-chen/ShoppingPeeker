using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingPeeker.Utilities.DataStructure
{
    /// <summary>
    /// 配置文件中的数据库连接字符串区域 ，集合
    /// </summary>
    public class ConnectionStringSection:List<ConnectionStringNode>
    {
        /// <summary>
        /// 配置节点名称
        /// </summary>
        public const string SectionName = "ConnectionStringSection";

    }
    /// <summary>
    /// 连接
    /// </summary>
    public class ConnectionStringNode
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }

        public bool SqlOutPut { get; set; }
    }
}
