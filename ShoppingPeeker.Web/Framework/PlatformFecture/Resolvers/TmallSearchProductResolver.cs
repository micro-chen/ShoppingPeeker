using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage.Products;

using ShoppingPeeker.Web.ViewModels;
using ShoppingPeeker.Plugins;
using ShoppingPeeker.Utilities.Plugins;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    /// <summary>
    /// 天猫内容解析
    /// </summary>
    public class TmallSearchProductResolver : BaseSearchProductResolver
    {
        /// <summary>
        /// 需要的插件名称
        /// </summary>
        const string NeedPluginName = "Plugin.Tmall.Extension";

         public TmallSearchProductResolver()
        {
         }

        /// <summary>
        /// 尝试解析 来自web 参数
        /// 解析为具体的平台的搜索地址：附带参数
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public override string ResolveSearchUrl( BaseFetchWebPageArgument webArgs)
        {
            string searchUrl = string.Empty;
            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin tmallPlugin = PluginManager.Load(NeedPluginName);
            if (null == tmallPlugin)
            {
                throw new Exception("未能加载插件：" + NeedPluginName);
            }

            searchUrl = tmallPlugin.ResolveSearchUrl(webArgs);

            return searchUrl;
        }

        public override SearchProductViewModel ResolvePageContent(string pageContent)
        {
            SearchProductViewModel dataModel = new SearchProductViewModel();

            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin tmallPlugin = PluginManager.Load(NeedPluginName);
            if (null==tmallPlugin)
            {
                throw new Exception("未能加载插件："+ NeedPluginName);
            }

            var resultBag = tmallPlugin.ResolveSearchPageContent(pageContent) as Dictionary<string,object>;
            if (null==resultBag)
            {
                throw new Exception("插件：" + NeedPluginName+ " ;未能正确解析内容："+pageContent);
            }
            dataModel.Tags = resultBag["Tags"] as List<KeyWordTag>;
            dataModel.Products= resultBag["Products"] as ProductBaseCollection;
            return dataModel;
        }
    }
}
