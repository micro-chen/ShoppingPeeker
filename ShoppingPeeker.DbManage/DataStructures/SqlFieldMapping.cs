using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Reflection;
 
namespace ShoppingPeeker.DbManage
{
    /// <summary>
    /// 管理本地sql 字段属性的映射
    /// </summary>
    internal class SqlFieldMappingManager
    {
        static SqlFieldMappingManager()
        {
            Mappings = new ConcurrentDictionary<string, SqlFieldMapping>();
        }

        public static ConcurrentDictionary<string, SqlFieldMapping> Mappings;
    }

    /// <summary>
    /// POCO 映射模型
    /// </summary>
    internal class SqlFieldMapping
    {


        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 实体属性集合
        /// </summary>
        public PropertyInfo[] Propertys { get; set; }
        /// <summary>
        /// 字段集合
        /// </summary>
        public string[] Filelds  { get; set; }

        /// <summary>
        /// sql 参数集合
        /// </summary>
        public string[] SqlParas  { get; set; }
    }
}
