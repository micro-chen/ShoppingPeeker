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
    /// 天猫自动完成搜索词类
    /// </summary>
    public class TmallAutoMapping : BaseAutoMappingWord
    {
        private const string templateOfSuggestUrl = "https://suggest.taobao.com/sug?code=utf-8&q={0}&callback=jsonp{1}&_ksTS=1498375027126_16071&area=b2c&code=utf-8&k=1&bucketid=9&src=tmall_pc&isg=AjMz-Y-YbpUSXyI6Z0X2N0dgwjedwF8rLGCF4eXUutKJ5FKGQjknesoy6gnV";
        /// <summary>
        /// 天猫请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly HttpClient tmallHttpClient;

        /// <summary>
        /// 天猫超市的标记
        /// </summary>
        private const string TMALL_DIRTY_WORD = "天猫超市";

        protected override string TargetUrl
        {
            get;set;
        }



        

        public TmallAutoMapping(string userAgent)
        {
            HttpServerProxy.ChangeHttpClientUserAgent(tmallHttpClient, userAgent);
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static TmallAutoMapping()
        {
            //初始化头信息
            var tmallRequestHeaders = BaseAutoMappingWord.GetCommonHttpRequestHeaders();
            tmallRequestHeaders.Add("Referer", "https://www.tmall.com/");
            tmallHttpClient = new HttpClient();
            tmallHttpClient.Timeout = TimeSpan.FromMilliseconds(2000);
            HttpServerProxy.FormatRequestHeader(tmallHttpClient.DefaultRequestHeaders, tmallRequestHeaders);

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
            //获取一个5位随机码
            string mixCode = StringUtil.GenerateRandomNumberString(5);
            this.TargetUrl = string.Format(templateOfSuggestUrl, keyWord,mixCode);


            /*结果师范
              jsonp13131({ "result":[["小米","48413","3567519482","0.61"],["小米6","1996","358038546","0.61"],
              ["小白鞋女","61435","1856829008-1856946250-3654449291-503519311-503636553","0.21"],
              ["小龙虾","1766","2171432531","0.54"],["小冰箱","6898","3569546137","0.48"],
              ["小米手环2","270","2952167576","0.76"],["小白鞋","87347","2973182695-4117220011-4117337253","0.31"],
              ["小风扇","5168","3568539911","0.53"],["小米6手机壳","2246","2427275086","0.50"],["小米5","5914","358006921","0.61"]]})*/

            string respText = this.QueryWordsResonseAsync(tmallHttpClient).Result;

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
                int endIndex = respText.LastIndexOf(tokenEndIndex) - startIndex + tokenEndIndex.Length;//

                //jsonp16163({"result":[]}) 这种内容表示空白 忽略掉
                if (startIndex<0)
                {
                    return null;
                }

                //取出 内容部分
                string content = respText.Substring(startIndex, endIndex);//respText.Length - startIndex - tokenEndIndex.Length
               content = this.FormatAndFilterString(content);
                result = JsonConvert.DeserializeObject<List<string[]>>(content);
                if (null!=result)
                {
                    foreach (var item in result)
                    {
                        if (item.Length<=0)
                        {
                            continue;
                        }
                        string fullWord = item[0];
                        if (fullWord.IndexOf(TMALL_DIRTY_WORD)>=0)
                        {
                            continue;
                        }
                        dataContaniner.Add(fullWord);
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
