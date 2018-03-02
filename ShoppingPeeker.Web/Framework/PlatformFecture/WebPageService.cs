using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using NTCPMessage;
using NTCPMessage.Client;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingPeeker.Utilities.Interface;
using ShoppingPeeker.Web.ViewModels;
using ShoppingPeeker.Utilities;
using ShoppingPeeker.Utilities.Logging;
using ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers;

namespace ShoppingPeeker.Web.Framework.PlatformFecture.WebPageService
{
    /// <summary>
    /// web 页面请求服务类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebPageService : IPlatformService
    {

        public static WebPageService CreateNew()
        {
            return new WebPageService();
        }
        /// <summary>
        /// 根据关键词 ,筛选条件， 请求对应平台上的返回结果
        /// 结果是各自平台上的 商品Item列表
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public SearchProductViewModel QueryProductsByKeyWords(BaseFetchWebPageArgument webArgs)
        {
            SearchProductViewModel dataModel = new SearchProductViewModel();
            if (webArgs.IsValid() == false)
            {
                return dataModel;
            }

            try
            {
                //注册搜索词到热词服务
                HotWordService.AddWord(webArgs.KeyWord);

                //是否开启内容缓存，如果开启，那么从缓存中加载内容
                if (true == WorkContext.IsFetchPageCacheaAble)
                {
                    dataModel = WorkContext.GetFetchPageResultFromCache(webArgs);
                    if (null != dataModel)
                    {
                        return dataModel;
                    }
                }

                //工厂模式 获取指定平台的内容解析器
                var resolver = ResolverFactory.GetSearchProductResolver(webArgs.Platform);
                //尝试解析页面参数的检索地址
                var searchUrl = resolver.ResolveSearchUrl(webArgs);
                if (null != searchUrl)
                {
                    webArgs.ResolvedUrl = searchUrl;
                }
                string pageContent = string.Empty;

                using (var connMgr = new WebCrawlerConnConfigManager())
                {

                    var connStrConfig = connMgr.Connection;
                    //;//ConfigHelper.WebCrawlerSection.ConnectionStringCollection["Crawler-Server1"];
                    webArgs.SystemAttachParas["SoapTcpConnectionString"] = connStrConfig;//register to attach paras

                    if (searchUrl.IsNeedPreRequest == true)
                    {
                        ////1 打开tcp 链接 
                        ////2 发送参数
                        ////3 解析结果

                        using (var conn = new SoapTcpConnection(connStrConfig))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }

                            //发送soap
                            var soapCmd = new SoapMessage() { Head = CommandConstants.CMD_FetchPage };
                            soapCmd.Body = webArgs.ToJson();
                            var dataContainer = conn.SendSoapMessage(soapCmd);
                            if (null != dataContainer && dataContainer.Status == 1)
                            {
                                pageContent = dataContainer.Result;
                            }
                            else
                            {
                                StringBuilder errMsg = new StringBuilder("抓取网页请求失败！参数：");
                                errMsg.Append(soapCmd.Body);
                                if (null != dataContainer && !string.IsNullOrEmpty(dataContainer.ErrorMsg))
                                {
                                    errMsg.Append("；服务端错误消息：")
                                        .Append(dataContainer.ErrorMsg);
                                }
                                throw new Exception(errMsg.ToString());
                            }
                        }
                    }
                }


                //开始解析内容字符串
                //*******注意：针对可以直接进行内容解析的连接，交给内容解析函数进行地址的内容请求和解析*********
                if (!string.IsNullOrEmpty(pageContent) || !searchUrl.IsNeedPreRequest)
                {

                    dataModel = resolver.ResolvePageContent(webArgs, pageContent);
                    if (null != dataModel)
                    {
                        dataModel.KeyWord = webArgs.KeyWord;
                        dataModel.IsNeedResolveHeaderTags = webArgs.IsNeedResolveHeaderTags;
                    }
                }



            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            //如果开启缓存页面结果
            if (true == WorkContext.IsFetchPageCacheaAble
                && null != dataModel
                && dataModel.Products.IsNotEmpty())
            {
                int cacheTime = ConfigHelper.AppSettingsConfiguration.GetConfigInt("FetchPageCacheTime");
                if (cacheTime <= 0)
                {
                    cacheTime = 60;//默认缓存页面结果60秒
                }
                WorkContext.SetFetchPageResultFromCache(webArgs, dataModel, cacheTime);
            }
            return dataModel;
        }

        ///// <summary>
        ///// 格式化 字符串 并且过滤
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //protected string FormatAndFilterString(string input)
        //{
        //    return input;
        //        //.Replace("<b>", "")
        //        //.Replace("</b>", "")
        //        //.Replace("<\\/b>", "");
        //}

        ///// <summary>
        ///// 查询关键词对应的网页内容异步支持
        ///// 返回的是 -响应的文本
        ///// </summary>
        ///// <returns></returns>
        //public Task<string> QuerySearchContantResonseAsync(HttpClient client, CookieContainer cookies = null)
        //{
        //    return this.SendRequesntAsync(client, cookies);
        //}


    }
}
