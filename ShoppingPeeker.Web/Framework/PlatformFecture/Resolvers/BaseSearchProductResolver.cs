using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingPeeker.Web.ViewModels;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    public abstract class BaseSearchProductResolver : ISearchProductResolver
    {
       

        public virtual string ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
            return string.Empty;
        }

        public abstract SearchProductViewModel ResolvePageContent(string pageContent);
        
    }
}
