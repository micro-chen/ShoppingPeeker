
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ShoppingPeeker.Utilities.Http;
using ShoppingPeeker.Utilities;
namespace ShoppingPeeker.Web.Framework.PlatformFecture
{
    public abstract class BaseRequest
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        protected abstract string TargetUrl { get; set; }

        /// <summary>
        /// 通用的请求头信息 
        /// 静态公共头信息  由于是静态头信息，所以不要添加键值对
        /// 直接使用键 访问修改值即可
        /// </summary>
        public static NameValueCollection GetCommonHttpRequestHeaders()
        {

            var commonRequestHeaders = new NameValueCollection();
            commonRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            commonRequestHeaders.Add("Cache-Control", "no-cache");
            commonRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
            commonRequestHeaders.Add("Connection", "Keep-Alive");
            commonRequestHeaders.Add("User-Agent", ConfigHelper.HostingConfiguration.GetConfig("UserAgent")); //"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

            return commonRequestHeaders;
        }


        /// <summary>
        /// 发送异步请求
        /// </summary>
        /// <returns></returns>
        protected Task<string> SendHttpRequesntAsync(HttpClient client, CookieContainer cookies = null)
        {
            var clientProxy = new HttpServerProxy() { Client = client, KeepAlive = true, Cookies = cookies };
            var resp = clientProxy.GetResponseTransferAsync(TargetUrl, null).Result;
            if (null != resp && resp.StatusCode != HttpStatusCode.InternalServerError)
            {
                return resp.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }


        }


    }
}
