
using System;
using System.Collections.Generic;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;
namespace ShoppingPeeker.Plugins
{
    public interface IPlugin
    {

        /// <summary>
        /// 插件描述清单
        /// </summary>
        PluginMeta MetaManifest { get; }

        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        IPlugin CreateNew();

        /// <summary>
        /// 尝试解析 来自web 参数
        /// 解析为具体的平台的搜索地址：附带参数
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        ResolvedSearchUrlWithParas ResolveSearchUrl( BaseFetchWebPageArgument webArgs);

        /// <summary>
        ///  解析搜索列表内容方法
        /// </summary>
        /// <param name="webArgs"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Dictionary<string, object> ResolveSearchPageContent(BaseFetchWebPageArgument webArgs, string content);
    }
}
