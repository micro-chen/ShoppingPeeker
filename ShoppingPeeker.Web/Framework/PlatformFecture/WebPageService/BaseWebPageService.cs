using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingPeeker.Utilities.Interface;
using ShoppingPeeker.Utilities.Http;
using ShoppingPeeker.Web.ViewModels;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.WebPageService
{
    /// <summary>
    /// web 页面请求服务的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseWebPageService: BaseRequest,IPlatformService 
    {
        /// <summary>
        /// 根据关键词 ,筛选条件， 请求对应平台上的返回结果
        /// 结果是各自平台上的 商品Item列表
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public abstract SearchProductViewModel QueryProductsByWords(BaseFetchWebPageArgument webArgs);

        /// <summary>
        /// 格式化 字符串 并且过滤
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string FormatAndFilterString(string input)
        {
            return input;
                //.Replace("<b>", "")
                //.Replace("</b>", "")
                //.Replace("<\\/b>", "");
        }

        /// <summary>
        /// 查询关键词对应的网页内容异步支持
        /// 返回的是 -响应的文本
        /// </summary>
        /// <returns></returns>
        public Task<string> QuerySearchContantResonseAsync(HttpClient client, CookieContainer cookies = null)
        {
            return this.SendRequesntAsync(client, cookies);

        }
    }
}
