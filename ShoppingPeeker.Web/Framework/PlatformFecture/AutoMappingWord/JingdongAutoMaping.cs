using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ShoppingPeeker.Utilities;
using ShoppingPeeker.Utilities.Http;
using ShoppingPeeker.Utilities.Logging;

using System.Net;
using System.Collections.Specialized;
using System.Net.Http;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.AutoMappingWord
{
    /// <summary>
    ///京东搜索词自动完成
    ///注意：京东 需要几个关键的请求头信息
    ///需要模拟头+指定一个Cookie 容器 请求下首页 把Cookie 保存
    /// </summary>
    public class JingdongAutoMaping : BaseAutoMappingWord
    {
        /// <summary>
        /// 京东获取关键词地址
        /// </summary>
        private const string templateOfSuggestUrl = "https://dd-search.jd.com/?ver=2&zip=1&key={0}&callback=jQuery{1}";

        // private const string jdMainPage = "https://www.jd.com/";
        /// <summary>
        /// 京东请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly HttpClient jdHttpClient;

        /// <summary>
        /// 京东自营的标记
        /// </summary>
        private const string JD_DIRTY_WORD= "京东自营";

        /// <summary>
        /// 请求的地址
        /// </summary>
        protected override string TargetUrl
        {
            get; set;
        }

        /// <summary>
        /// 请求使用的Cookies
        /// </summary>
        //private static CookieContainer Cookies;
        /// <summary>
        /// 京东请求需要的头信息
        /// </summary>
       // private  NameValueCollection jdRequestHeaders;

        public JingdongAutoMaping(string userAgent)
        {
            HttpServerProxy.ChangeHttpClientUserAgent(jdHttpClient, userAgent);
        }

        /// <summary>
        /// 静态构造函数 进行一次初始化
        /// </summary>
        static JingdongAutoMaping()
        {
            //初始化头信息
           var  jdRequestHeaders = BaseAutoMappingWord.GetCommonHttpRequestHeaders();
            jdRequestHeaders.Add("Referer", "https://www.jd.com/");

            jdHttpClient = new HttpClient();
            jdHttpClient.Timeout= TimeSpan.FromMilliseconds(2000);
            HttpServerProxy.FormatRequestHeader(jdHttpClient.DefaultRequestHeaders, jdRequestHeaders);

        }

        ///// <summary>
        ///// 初始化Cookie信息
        ///// </summary>
        //private void Init()
        //{

        //    //if (null == Cookies)
        //    //{
        //    //    var cookedClient = new CookedWebClient();
        //    //    cookedClient.GetWebRequestAndResponse(new Uri(jdMainPage));

        //    //    Cookies = cookedClient.Cookies;
        //    //}


        //}
        /// <summary>
        /// 查询关键词集合
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public override List<string> QueryWords(string keyWord)
        {
            List<string> dataContaniner = new List<string>();
            if (string.IsNullOrEmpty(keyWord))
            {
                return null;
            }
            //获取一个6位随机码
            string mixCode = StringUtil.GenerateRandomNumberString(6);
            this.TargetUrl = string.Format(templateOfSuggestUrl, keyWord, mixCode);


            /*结果师范
             * jQuery6791318([{"key":"中号垃圾袋","qre":1164},
             * {"key":"中号长尾夹","qre":565},
             * {"key":"中号夹子","qre":7502},{"key":"中号垃圾桶","qre":4314},
             * {"key":"中号燕尾夹","qre":563},{"key":"中号信封","qre":1138},{"key":"中号垃圾袋京东自营","qre":71},{"key":"中号电池","qre":1951},
             * {"key":"中号毛笔","qre":2130},{"key":"中号避孕套","qre":1478},{"version":"T11"}])
             */

            string respText = this.QueryWordsResonseAsync(jdHttpClient).Result;

            if (string.IsNullOrEmpty(respText))
            {
                return null;
            }

            //注意  由于我们只取 键值对的第一个  所以没必要 写成对象的序列化
            List<Dictionary<string, string>> result = null;

            try
            {
                string tokenStartIndex = "[{";
                string tokenEndIndex = ")";
                int startIndex = respText.IndexOf(tokenStartIndex);
                int endIndex = respText.LastIndexOf(tokenEndIndex);
                //取出 内容部分
                string content = respText.Substring(startIndex, respText.Length - startIndex - tokenEndIndex.Length);

                result = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(content);
                if (null != result)
                {
                    foreach (var item in result)
                    {
                        if (null==item||item.Count <= 0)
                        {
                            continue;
                        }
                        if (item.ContainsKey("key"))
                        {
                            string fullWord = item["key"];//取出键值对的 key那个即可
                            if (fullWord.IndexOf(JD_DIRTY_WORD)>=0)
                            {
                                continue;
                            }
                            dataContaniner.Add(fullWord);
                        }
                     
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //throw ex;
            }

            return dataContaniner;
        }
    }


}
