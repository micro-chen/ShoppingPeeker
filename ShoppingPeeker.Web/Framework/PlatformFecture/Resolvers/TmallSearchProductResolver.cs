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
        protected override string NeedPluginName
        {
            get
            {
                return "Plugin.Tmall.Extension";
            }
        }

        public TmallSearchProductResolver()
        {

        }



    }
}
