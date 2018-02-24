using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingPeeker.Utilities.Caching;
using ShoppingPeeker.Utilities.Logging;

namespace ShoppingPeeker.Web.Framework.PlatformFecture
{

    /// <summary>
    /// 热搜词服务
    /// </summary>
    public class HotWordService
    {
        private static readonly string _cacheKeyForHotWords = "HotWords:_cacheKeyForHotWords";
        private static readonly string _cacheKeyForHotWords_topConunt_Prefix = "HotWords:_cacheKeyForHotWords_topConunt_";

        /// <summary>
        /// 取出热词列表
        /// </summary>
        /// <param name="topConut"></param>
        /// <returns></returns>
        public static List<string> GetHotWords(int topConut = 10)
        {
            var dataList = new List<string>();

            //从预先分好的排名中读取缓存
            string miniCacheKey = string.Concat(_cacheKeyForHotWords_topConunt_Prefix, topConut);
            if (WorkContext.RedisClient.IsHasSet(miniCacheKey))
            {
                dataList = WorkContext.RedisClient.Get<List<string>>(miniCacheKey);
                return dataList;
            }


            try
            {

                var dataCache = WorkContext.RedisClient.Get<Dictionary<string, HotWord>>(_cacheKeyForHotWords);

                //--防止溢出，过大的字典！！--超过100个词 自动置空---
                if (null == dataCache || dataCache.Count <= 0)
                {
                    return dataList;
                }

                //倒序 取值
                dataList = dataCache.OrderByDescending(x => x.Value.Count)
                    .Take(topConut)
                    .Select(x => x.Value.Word)
                    .ToList();

                //设置关键词排名
                WorkContext.RedisClient.Set(miniCacheKey, dataList, 10);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return dataList;

        }
        /// <summary>
        /// 添加/增加关键词的计数
        /// </summary>
        /// <param name="word"></param>
        public static void AddWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            try
            {


                var dataCache = WorkContext.RedisClient.Get<Dictionary<string, HotWord>>(_cacheKeyForHotWords);

                //--防止溢出，过大的字典！！--超过100个词 自动置空---
                if (null != dataCache && dataCache.Count > 100)
                {
                    WorkContext.RedisClient.Remove(_cacheKeyForHotWords);
                    dataCache = null;
                }

                if (null == dataCache)
                {

                    dataCache = new Dictionary<string, HotWord>();
                    dataCache.Add(word, new HotWord { Word = word, Count = 1, AddTime = DateTime.Now });
                    WorkContext.RedisClient.Set(_cacheKeyForHotWords, dataCache, 60 * 5);//注册缓存5min

                }
                else
                {
                    var leftTime = (int)WorkContext.RedisClient.GetLeftTime(_cacheKeyForHotWords).TotalSeconds;
                    if (dataCache.ContainsKey(word))
                    {
                        var wordObj = dataCache[word];
                        wordObj.Count += 1;
                    }
                    else
                    {
                        dataCache.Add(word, new HotWord { Word = word, Count = 1, AddTime = DateTime.Now });
                    }

                    WorkContext.RedisClient.Set(_cacheKeyForHotWords, dataCache, leftTime);//注册缓存--剩余时间
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }


        private class HotWord
        {
            public int Count { get; set; }
            public string Word { get; set; }
            public DateTime AddTime { get; set; }
        }
    }
}
