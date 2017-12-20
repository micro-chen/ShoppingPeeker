using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingPeeker.Web.ViewModels;
using ShoppingPeeker.Utilities.Plugins;
using ShoppingPeeker.Plugins;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    public abstract class BaseSearchProductResolver : ISearchProductResolver
    {
        /// <summary>
        /// 解析器需要的插件
        /// </summary>
       protected abstract string NeedPluginName { get; }

        /// <summary>
        /// 尝试解析 来自web 参数
        /// 解析为具体的平台的搜索地址：附带参数
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public virtual string ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
            string searchUrl = string.Empty;
            if (string.IsNullOrEmpty(this.NeedPluginName))
            {
                throw new Exception("必须制定依赖的插件名称！");
            }

            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin pluginInstance = PluginManager.Load(NeedPluginName);
            if (null == pluginInstance)
            {
                throw new Exception("未能加载插件：" + NeedPluginName);
            }

            searchUrl = pluginInstance.ResolveSearchUrl(webArgs);

            return searchUrl;
        }

        /// <summary>
        /// 解析搜索页面列表的内容
        /// </summary>
        /// <param name="pageContent"></param>
        /// <returns></returns>
        public virtual SearchProductViewModel ResolvePageContent(string pageContent)
        {
            SearchProductViewModel dataModel = new SearchProductViewModel();
            if (string.IsNullOrEmpty(this.NeedPluginName))
            {
                throw new Exception("必须制定依赖的插件名称！");
            }
            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin pluginInstance = PluginManager.Load(NeedPluginName);
            if (null == pluginInstance)
            {
                throw new Exception("未能加载插件：" + NeedPluginName);
            }

            var resultBag = pluginInstance.ResolveSearchPageContent(pageContent) as Dictionary<string, object>;
            if (null == resultBag)
            {
                throw new Exception("插件：" + NeedPluginName + " ;未能正确解析内容：" + pageContent);
            }
            dataModel.Brands = resultBag["Brands"] as List<BrandTag>;
            dataModel.Tags = resultBag["Tags"] as List<KeyWordTag>;
            dataModel.Products = resultBag["Products"] as ProductBaseCollection;
            return dataModel;
        }
        
    }
}
