using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using ShoppingPeeker.Web.ViewModels;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    /// <summary>
    /// 搜索商品的结果解析器接口
    /// </summary>
    public interface ISearchProductResolver
    {
        /// <summary>
        /// 解析页面内容，返回商品视图模型
        /// </summary>
        /// <param name="pageContent"></param>
        /// <returns></returns>
        SearchProductViewModel Resolve(string pageContent);
    }
}
