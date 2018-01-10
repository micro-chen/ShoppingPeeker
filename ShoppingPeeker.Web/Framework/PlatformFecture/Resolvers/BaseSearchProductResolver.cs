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
        /// get depended plugin
        /// </summary>
        /// <returns></returns>
        protected IPlugin GetNeedPluginInstance()
        {
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

            return pluginInstance;
        }
        /// <summary>
        /// 尝试解析 来自web 参数
        /// 解析为具体的平台的搜索地址：附带参数
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public virtual ResolvedSearchUrlWithParas ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
            ResolvedSearchUrlWithParas searchUrl = null;

            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin pluginInstance = this.GetNeedPluginInstance();

            searchUrl = pluginInstance.ResolveSearchUrl(webArgs);

            return searchUrl;
        }

        // <summary>
        /// 执行内容解析
        /// </summary>
        ///<param name="webArgs">来自web 参数</param> 
        /// <param name="content">要解析的内容</param>
        /// <returns></returns>
        public virtual SearchProductViewModel ResolvePageContent(BaseFetchWebPageArgument webArgs, string pageContent)
        {

            SearchProductViewModel dataModel = new SearchProductViewModel();

            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin pluginInstance = this.GetNeedPluginInstance();


            var resultBag = pluginInstance.ResolveSearchPageContent(webArgs, pageContent) as Dictionary<string, object>;
            if (null == resultBag)
            {
                throw new Exception("插件：" + NeedPluginName + " ;未能正确解析内容：" + pageContent);
            }
            if (webArgs.IsNeedResolveHeaderTags == true)
            {
                if (resultBag.ContainsKey("Brands"))
                {
                    dataModel.Brands = resultBag["Brands"] as List<BrandTag>;
                }
                if (resultBag.ContainsKey("Tags"))
                {
                    dataModel.Tags = resultBag["Tags"] as List<KeyWordTag>;
                }

            }

            dataModel.Products = resultBag["Products"] as ProductBaseCollection;
            return dataModel;
        }

    }
}
