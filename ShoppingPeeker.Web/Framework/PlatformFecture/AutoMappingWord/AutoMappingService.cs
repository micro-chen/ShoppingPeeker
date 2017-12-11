﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Threading.Tasks;
using ShoppingPeeker.Utilities.Logging;
using ShoppingPeeker.Utilities.Caching;

using ShoppingPeeker.Utilities.Http;
using ShoppingPeeker.Utilities.Interface;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.AutoMappingWord
{
    public class AutoMappingService: IPlatformService
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AutoMappingService()
        {

        }
        /// <summary>
        /// 查询这个关键词在3大平台上的关键词集合，
        /// 统计 按照统计进行排序并输出
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public  IEnumerable<string> QueryThisKeywordMappings(string keyWord)
        {
            //创建一个 基于阻塞集合 用以保证线程安全
            var allWords = new BlockingCollection<string>();


            //------------cache begin-----------------------
            string cacheKey = string.Concat("AutoMapping-", keyWord);
            var cacheManager = CacheConfigFactory.GetCacheManager();
            if (cacheManager.IsHasSet(cacheKey))
            {
                var cacheWords = cacheManager.Get<IEnumerable<string>>(cacheKey);
                if (cacheWords != null)
                {
                    return cacheWords;
                    //var cursor= cacheWords.GetEnumerator();//直接返回缓存对象数组
                    //while (cursor.MoveNext())
                    //{
                    //    yield return cursor.Current;
                    //}
                }
            }


            //----------如果未能从缓存获取，那么从网络流中检索-------------------

            string userAgent = WorkContext.HttpContext.Request.Headers[HttpServerProxy.RequestHeaderKeyUserAgent];

            //开辟3个异步任务 并行获取关键词 注意：即使某个平台挂了 不能查询关键词  那么不妨碍其他平台
            var tsk_tmall = Task.Factory.StartNew(() =>
            {
                try
                {
                    var result_words = new TmallAutoMapping(userAgent).QueryWords(keyWord);
                    allWords.AddRange(result_words);
                }
                catch (Exception ex)
                {
                    Logger.Error(new Exception(string.Concat("tmall 获取关键词列表失败：失败原因:", ex.ToString())));
                }

            });
            var tsk_taobao = Task.Factory.StartNew(() =>
            {
                try
                {
                    var result_words = new TaobaoAutoMapping(userAgent).QueryWords(keyWord);
                    allWords.AddRange(result_words);
                }
                catch (Exception ex)
                {
                    Logger.Error(new Exception(string.Concat("taobao 获取关键词列表失败：失败原因:", ex.ToString())));
                }

            });


            var tsk_jingdong = Task.Factory.StartNew(() =>
            {
                try
                {
                    var result_words = new JingdongAutoMaping(userAgent).QueryWords(keyWord);
                    allWords.AddRange(result_words);
                }
                catch (Exception ex)
                {
                    Logger.Error(new Exception(string.Concat("jingdong 获取关键词列表失败：失败原因:", ex.ToString())));
                }
            });
            //等待并行任务完毕
            Task.WaitAll(tsk_tmall, tsk_taobao, tsk_jingdong);


            //分组 排序 并取前十
            var gps = allWords.GroupBy(x => x).OrderByDescending(x => x.Count()).Take(10);

            //-------------------注册到缓存中--------------------
            var resultList = gps.Select(x => x.Key);
            cacheManager.Set(cacheKey, resultList);

            return resultList;
            #region 废弃  yield
            //foreach (var item in gps)
            //{
            //    yield return item.Key;
            //}
            #endregion

        }



    }
}
