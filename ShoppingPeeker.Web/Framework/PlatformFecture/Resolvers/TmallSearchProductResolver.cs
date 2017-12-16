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
    public class TmallSearchProductResolver : ISearchProductResolver
    {
        /// <summary>
        /// 需要的插件名称
        /// </summary>
        const string NeedPluginName = "Plugin.Tmall.Extension";
         public TmallSearchProductResolver()
        {
         }
        public SearchProductViewModel Resolve(string pageContent)
        {
            SearchProductViewModel dataModel = new SearchProductViewModel();

            /// 尝试加载所需的插件，使用插件进行内容解析
            IPlugin tmallPlugin = PluginManager.Load(NeedPluginName);
            if (null==tmallPlugin)
            {
                throw new Exception("未能加载插件："+ NeedPluginName);
            }

            var resultBag = tmallPlugin.Execute(pageContent) as Dictionary<string,object>;
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
