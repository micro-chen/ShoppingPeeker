using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;
using ShoppingPeeker.Web.ViewModels;
using ShoppingPeeker.Plugins;
using ShoppingPeeker.Utilities.Plugins;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    public class VipSearchProductResolver : BaseSearchProductResolver
    {

        /// <summary>
        /// 需要的插件名称
        /// </summary>
        protected override string NeedPluginName
        {
            get
            {
                return "Plugin.Vip.Extension";
            }
        }

    }
}