using System;
using System.Collections.Generic;
using System.Text;
using NTCPMessage.EntityPackage;

namespace ShoppingPeeker.Plugins
{
    /// <summary>
    /// 商品可排序对象
    /// </summary>
    /// <typeparam name="ProductBase"></typeparam>
    public class ProductOrdered<T>where T: ProductBase
    {
        /// <summary>
        /// 唯一键
        /// </summary>
        public string UniqKey { get; set; }
        /// <summary>
        /// 排序位置
        /// </summary>
        public int IndexOrder { get; set; }
        /// <summary>
        /// 商品对象
        /// </summary>
        public T Product { get; set; }
    }
}
