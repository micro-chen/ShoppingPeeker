using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ShoppingPeeker.Utilities;
using ShoppingPeeker.Utilities.Logging;
using System.Collections.Specialized;
using System.Net.Http;
using ShoppingPeeker.Utilities.Http;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.AutoMappingWord
{

    /// <summary>
    /// 淘宝关键词自动完成
    /// </summary>
    public class TaobaoAutoMapping : BaseAutoMappingWord
    {
        /// <summary>
        /// 淘宝关键词获取地址
        /// </summary>
        private const string templateOfSuggestUrl = "https://suggest.taobao.com/sug?code=utf-8&q={0}&callback=jsonp{1}&k=1&area=c2c&bucketid=2";

        /// <summary>
        /// 淘宝请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly HttpClient taobaoHttpClient;

        protected override string TargetUrl
        {
            get; set;
        }


        public TaobaoAutoMapping(string userAgent)
        {
            HttpServerProxy.ChangeHttpClientUserAgent(taobaoHttpClient, userAgent);
        }


        /// <summary>
        /// 静态构造函数
        /// </summary>
        static TaobaoAutoMapping()
        {
            //初始化头信息
            var taobaoRequestHeaders = BaseAutoMappingWord.GetCommonHttpRequestHeaders();
            taobaoRequestHeaders.Add("Referer", "https://www.taobao.com/");
            taobaoHttpClient = new HttpClient();
            taobaoHttpClient.Timeout = TimeSpan.FromMilliseconds(2000);
            HttpServerProxy.FormatRequestHeader(taobaoHttpClient.DefaultRequestHeaders, taobaoRequestHeaders);

        }

 

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
            //获取一个4位随机码
            string mixCode = StringUtil.GenerateRandomNumberString(4);
            this.TargetUrl = string.Format(templateOfSuggestUrl, keyWord, mixCode);


            /*结果师范
              jsonp8662({"result":[["小0蓝牙音箱","1652"],["小0件","50204"],["小0食","1960"],["小0音响","1719"],["小0pp0手机套","658"],
              ["小0音箱","1515"],["小0冰箱","335"],["小0家","15808"],["小0公仔","526"],["小0钱包","1206"]]})*/

            string respText = this.QueryWordsResonseAsync(taobaoHttpClient).Result;

            if (string.IsNullOrEmpty(respText))
            {
                return null;
            }

            List<string[]> result = null;

            try
            {
                string tokenStartIndex = "[[";
                string tokenEndIndex = "]]";
                int startIndex = respText.IndexOf(tokenStartIndex);
                int endIndex = respText.IndexOf(tokenEndIndex)+ tokenEndIndex.Length;

                if (startIndex < 0)
                {
                    return null;
                }

                //取出 内容部分
                string content = respText.SubStringSlice(startIndex, endIndex);
                content = this.FormatAndFilterString(content);
                result = JsonConvert.DeserializeObject<List<string[]>>(content);
                if (null != result)
                {
                    foreach (var item in result)
                    {
                        if (item.Length <= 0)
                        {
                            continue;
                        }
                        //System.Net.WebUtility.HtmlDecode(
                        string fullWord = item[0];

                        dataContaniner.Add(fullWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                // throw ex;
            }

            return dataContaniner;
        }
    }
}
