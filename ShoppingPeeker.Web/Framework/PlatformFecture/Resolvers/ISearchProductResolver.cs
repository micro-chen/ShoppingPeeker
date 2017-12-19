using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;

using ShoppingPeeker.Web.ViewModels;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    /// <summary>
    /// 搜索商品的结果解析器接口
    /// </summary>
    public interface ISearchProductResolver
    {
        /// <summary>
        /// 尝试解析 来自web 参数
        /// 解析为具体的平台的搜索地址：附带参数
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        string ResolveSearchUrl( BaseFetchWebPageArgument webArgs);
        /// <summary>
        /// 解析页面内容，返回商品视图模型
        /// </summary>
        /// <param name="pageContent"></param>
        /// <returns></returns>
        SearchProductViewModel ResolvePageContent(string pageContent);
       
    }
}
