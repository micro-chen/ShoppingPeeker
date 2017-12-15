using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingPeeker.Utilities.Http;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.AutoMappingWord
{
    /// <summary>
    /// 自动完成关键词检索基类
    /// </summary>
    public abstract class BaseAutoMappingWord: BaseRequest
    {
 
             
        /// <summary>
        /// 各自平台上的查询关键词
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public abstract List<string> QueryWords(string keyWord);

        /// <summary>
        /// 格式化 字符串 并且过滤
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string FormatAndFilterString(string input)
        {
            return input
                .Replace("<b>", "")
                .Replace("</b>", "")
                .Replace("<\\/b>","");
        }

        /// <summary>
        /// 查询关键词异步支持
        /// 返回的是 -响应的文本，需要
        /// </summary>
        /// <returns></returns>
        public Task<string> QueryWordsResonseAsync(HttpClient client, CookieContainer cookies = null)
        {
            return this.SendHttpRequesntAsync(client, cookies);

        }
    }
}
