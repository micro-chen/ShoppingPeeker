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
        ResolvedSearchUrlWithParas ResolveSearchUrl( BaseFetchWebPageArgument webArgs);
        // <summary>
        /// 执行内容解析
        /// </summary>
        ///<param name="isNeedHeadFilter">是否要解析头部筛选</param> 
        /// <param name="content">要解析的内容</param>
        /// <returns></returns>
        SearchProductViewModel ResolvePageContent(bool isNeedHeadFilter, string pageContent);
       
    }
}
