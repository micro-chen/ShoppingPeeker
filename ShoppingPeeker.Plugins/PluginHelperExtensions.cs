using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class PluginHelperExtensions
    {
        private const string HTTPS_PREFIX = "https:";
        public static string GetHttpsUrl(this string url)
        {

            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            if (url.IndexOf(HTTPS_PREFIX)!=0)
            {
                // http 开头的 or 不是http 开头的
                if (url.IndexOf("http:")==0)
                {
                    return url.Replace("http:", HTTPS_PREFIX);
                }
                else if(url.IndexOf("//")==0)
                {
                    return string.Concat(HTTPS_PREFIX, url);
                }
                else
                {
                    return string.Concat(HTTPS_PREFIX, "//", url);
                }
            }


            return url;
        }
    }
}
