using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;

using NTCPMessage.EntityPackage.Products;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;
using ShoppingPeeker.Plugins;
using NTCPMessage.Client;
using NTCPMessage;

namespace Plugin.Taobao.Extension
{
    public class TaobaoPlugin : PluginBase<TaobaoPlugin>
    {


        /// <summary>
        /// 自我创建新实例
        /// --比较复杂，未完成
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new TaobaoPlugin();
            return instance;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Initialize()
        {
        }

        public override string PluginDirectory
        {
            get
            {
                var dir = Assembly.GetExecutingAssembly().GetDirectoryPath();
                return dir;
            }
        }


        /// <summary>
        /// 解析搜索地址
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public override ResolvedSearchUrlWithParas ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
            ResolvedSearchUrlWithParas resultUrl = new ResolvedSearchUrlWithParas();
            try
            {


                StringBuilder sbSearchUrl = new StringBuilder("https://s.taobao.com/search?q=@###@&imgfile=");

                string filerValueString = "";
                #region 品牌
                if (null != webArgs.Brands && webArgs.Brands.Count > 0)
                {
                    //1 当前平台的品牌
                    var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Taobao);
                    if (currentPlatformBrands.Any())
                    {
                        //多个品牌用 , 号分割
                        string brandIds = string.Join(";", currentPlatformBrands.Select(x => x.BrandId));
                        filerValueString += brandIds;
                    }

                    //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                    var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Taobao);
                    if (null != otherPlatformBrands)
                    {
                        webArgs.KeyWord += " " + otherPlatformBrands.BrandName;
                    }
                }
                #endregion

                #region  属性标签
                if (null != webArgs.TagGroup)
                {
                    //1 当前平台的
                    var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Taobao);
                    if (null != currentPlatformTag)
                    {
                        //1 分类 cat
                        var catFilter = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "cat");
                        if (null != catFilter)
                        {
                            sbSearchUrl.Append("&cat=").Append(catFilter.Value);
                        }

                        // 2 其他的ppath标签
                        var ppathFilter = currentPlatformTag.Where(x => x.FilterFiled == "ppath");
                        if (ppathFilter.Any())
                        {
                            string ppathIds = string.Join(";", ppathFilter.Select(x => x.Value));
                            filerValueString += ";";
                            filerValueString += ppathIds;

                        }

                    }
                    //2 其他平台的tag 作为关键词的一部分
                    var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Taobao);
                    if (null != otherPlatformTag)
                    {
                        webArgs.KeyWord += " " + otherPlatformTag.TagName;
                    }
                }
                //-----追加过滤字段特性--------
                if (!string.IsNullOrEmpty(filerValueString))
                {
                    sbSearchUrl.Append("&ppath=").Append(filerValueString);
                }


                #endregion

                #region 关键词
                sbSearchUrl.Replace("@###@", webArgs.KeyWord);//将关键词的占位符 进行替换
                #endregion

                #region  排序
                if (null != webArgs.OrderFiled)
                {
                    sbSearchUrl.Append("&sort=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
                }
                #endregion

                #region  筛选-价格区间
                #endregion

                #region  页码

                var pageNumber = webArgs.PageIndex + 1;
                if (pageNumber > 0)
                {
                    //sbSearchUrl.Append("&data-key=s&data-value=").Append(pageNumber * 44);//淘宝的分页是基于页索引*44
                    sbSearchUrl.Append("&s=").Append(webArgs.PageIndex * 44);
                }
                #endregion

                #region 杂项
                //string timeToken = JavascriptContext.getUnixTimestamp();
                //sbSearchUrl.AppendFormat("&_ksTS={0}_897", timeToken);
                //sbSearchUrl.Append("&commend=all");
                //sbSearchUrl.Append("&ssid=s5-e");
                //sbSearchUrl.Append("&search_type=item");
                //sbSearchUrl.Append("&sourceId=tb.index");
                //sbSearchUrl.Append("&spm=a21bo.50862.201856-taobao-item.1");
                sbSearchUrl.Append("&ie=utf8");
                //sbSearchUrl.Append("&ajax=true");
                sbSearchUrl.Append("&js=1");
                //sbSearchUrl.Append("&style=grid");
                sbSearchUrl.Append("&stats_click=search_radio_all%3A1");
                sbSearchUrl.Append("&bcoffset=4");
                sbSearchUrl.Append("&ntoffset=4");
                sbSearchUrl.Append("&p4ppushleft=1%2C48");

                sbSearchUrl.AppendFormat("&initiative_id=staobaoz_{0}", DateTime.Now.ToString("yyyyMMdd"));
                #endregion
                resultUrl.Url = sbSearchUrl.ToString();

            }
            catch (Exception ex)
            {

                PluginContext.Logger.Error(ex);
            }
            return resultUrl;
        }


        /// <summary>
        /// 解析搜索首页的剩余的jsonp 获取商品的地址
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        private string ResolveSlicedSearchPageSilcedUrl(BaseFetchWebPageArgument webArgs)
        {
            ResolvedSearchUrlWithParas resultUrl = new ResolvedSearchUrlWithParas();
            StringBuilder sbSearchUrl = new StringBuilder("https://s.taobao.com/api?q=@###@&imgfile=");

            try
            {



                string filerValueString = "";
                #region 品牌
                if (null != webArgs.Brands && webArgs.Brands.Count > 0)
                {
                    //1 当前平台的品牌
                    var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Taobao);
                    if (currentPlatformBrands.Any())
                    {
                        //多个品牌用 , 号分割
                        string brandIds = string.Join(";", currentPlatformBrands.Select(x => x.BrandId));
                        filerValueString += brandIds;
                    }

                    //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                    var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Taobao);
                    if (null != otherPlatformBrands)
                    {
                        webArgs.KeyWord += " " + otherPlatformBrands.BrandName;
                    }
                }
                #endregion

                #region  属性标签
                if (null != webArgs.TagGroup)
                {
                    //1 当前平台的
                    var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Taobao);
                    if (null != currentPlatformTag)
                    {
                        //1 分类 cat
                        var catFilter = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "cat");
                        if (null != catFilter)
                        {
                            sbSearchUrl.Append("&cat=").Append(catFilter.Value);
                        }

                        // 2 其他的ppath标签
                        var ppathFilter = currentPlatformTag.Where(x => x.FilterFiled == "ppath");
                        if (ppathFilter.Any())
                        {
                            string ppathIds = string.Join(";", ppathFilter.Select(x => x.Value));
                            filerValueString += ";";
                            filerValueString += ppathIds;

                        }

                    }
                    //2 其他平台的tag 作为关键词的一部分
                    var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Taobao);
                    if (null != otherPlatformTag)
                    {
                        webArgs.KeyWord += " " + otherPlatformTag.TagName;
                    }
                }
                //-----追加过滤字段特性--------
                if (!string.IsNullOrEmpty(filerValueString))
                {
                    sbSearchUrl.Append("&ppath=").Append(filerValueString);
                }


                #endregion

                #region 关键词
                sbSearchUrl.Replace("@###@", webArgs.KeyWord);//将关键词的占位符 进行替换
                #endregion

                #region  排序
                if (null != webArgs.OrderFiled)
                {
                    sbSearchUrl.Append("&sort=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
                }
                #endregion

                #region  筛选-价格区间
                #endregion

                #region  页码
                sbSearchUrl.Append("&s=36");//this must be a constant value  36 !!!!!!
                #endregion

                #region 杂项
                string timeToken = JavascriptContext.getUnixTimestamp();
                sbSearchUrl.AppendFormat("&_ksTS={0}_897", timeToken);

                sbSearchUrl.Append("&callback=jsonp2822");
                sbSearchUrl.Append("&m=customized");
                sbSearchUrl.Append("&ps=1");

                sbSearchUrl.Append("&ie=utf8");
                sbSearchUrl.Append("&ajax=true");
                sbSearchUrl.Append("&js=1");
                sbSearchUrl.Append("&p4ppushleft=1,48");
                sbSearchUrl.Append("&stats_click=search_radio_all:1");
                sbSearchUrl.Append("&bcoffset=0");
                sbSearchUrl.Append("&ntoffset=4");
                sbSearchUrl.Append("&rn=ee5b33aee4d18bf96ab0ad083eadc7f0");
                sbSearchUrl.AppendFormat("&initiative_id=staobaoz_{0}", DateTime.Now.ToString("yyyyMMdd"));
                #endregion

            }
            catch (Exception ex)
            {

                PluginContext.Logger.Error(ex);
            }
            return sbSearchUrl.ToString();

        }
        /// <summary>
        /// 执行内容解析
        /// </summary>
        ///<param name="webArgs"></param> 
        /// <param name="content">要解析的内容</param>
        /// <returns>返回需要的字段对应的字典</returns>
        public override Dictionary<string, object> ResolveSearchPageContent(BaseFetchWebPageArgument webArgs, string content)
        {


            var resultBag = new Dictionary<string, object>();

            try
            {


                string jsonData = string.Empty;

                if (content.IndexOf("g_page_config") < 0)
                {
                    return null;//无效的页面结果数据
                }


                //send request for load other data of first search page
                Task<string> tskSilcedJsonpContent = null;
                if (webArgs.PageIndex == 0)
                {
                    tskSilcedJsonpContent = Task.Factory.StartNew(() =>
                    {

                        string jsonpContent = "";
                    ////1 打开tcp 链接 
                    ////2 发送参数
                    ////3 解析结果
                    if (!webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
                        {
                            return jsonpContent;
                        }
                        var connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as ShoppingWebCrawlerSection.ConnectionStringConfig;
                        if (null == connStrConfig)
                        {
                            return jsonpContent;
                        }
                    //重写解析地址-首页的分片jsonp地址
                    string urlOfSlicedJsonp = this.ResolveSlicedSearchPageSilcedUrl(webArgs);
                        webArgs.ResolvedUrl = new ResolvedSearchUrlWithParas { Url = urlOfSlicedJsonp };
                        using (var conn = new SoapTcpConnection(connStrConfig))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }

                        //发送soap
                        var soapCmd = new SoapMessage() { Head = CommandConstants.CMD_FetchPage };
                            soapCmd.Body = JsonConvert.SerializeObject(webArgs);
                            var dataContainer = conn.SendSoapMessage(soapCmd);
                            if (null != dataContainer && dataContainer.Status == 1)
                            {
                                jsonpContent = dataContainer.Result;
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
                                PluginContext.Logger.Error(errMsg.ToString());
                            }
                        }

                        return jsonpContent;

                    });

                }


                int startPos = content.IndexOf("g_page_config");
                int endPos = content.IndexOf("g_srp_loadCss") - startPos;
                var secondContent = content.Substring(startPos, endPos);
                int secStartPos = secondContent.IndexOf('{');
                int secEndPos = secondContent.IndexOf("};") - secStartPos + 1;
                jsonData = secondContent.Substring(secStartPos, secEndPos);



                TaobaoPageJsonResut pageJsonObj = JsonConvert.DeserializeObject<TaobaoPageJsonResut>(jsonData);
                if (null == pageJsonObj)
                {
                    return null;
                }

                if (webArgs.IsNeedResolveHeaderTags == true)
                {

                    var navNode = pageJsonObj.mods.nav;
                    if (null != navNode && null != navNode.data)
                    {

                        var commonNode = navNode.data.common;
                        var advNode = navNode.data.adv;

                        //解析common节点
                        if (null != commonNode && commonNode.Any())
                        {
                            //1 检测是否有品牌，有的话 解析品牌
                            #region 品牌解析


                            var brandNode = commonNode.FirstOrDefault(x => x.text == "品牌" && x.sub != null);
                            if (null != brandNode && brandNode.sub != null)
                            {
                                var lstBrands = new List<BrandTag>();
                                foreach (var subItem in brandNode.sub)
                                {
                                    var model = new BrandTag();
                                    model.Platform = SupportPlatformEnum.Taobao;
                                    model.FilterField = "ppath";//使用的过滤字段参数

                                    model.BrandId = subItem.value;
                                    model.BrandName = subItem.text;
                                    model.CharIndex = PinYin.GetFirstLetter(model.BrandName);
                                    lstBrands.Add(model);
                                }
                                //解析完毕品牌
                                resultBag.Add("Brands", lstBrands);
                            }

                            #endregion

                        }


                        //2其他筛选节点的分析

                        #region tags 解析


                        var lstTags = new List<KeyWordTag>();

                        var otherFilterNode1 = commonNode.Where(x => x.text != "品牌" && x.sub != null);
                        foreach (var itemNode in otherFilterNode1)
                        {
                            //找到归属的组
                            string groupName = itemNode.text;
                            ProcessTags(lstTags, itemNode.sub, groupName);
                        }

                        //advNode 的解析
                        foreach (var itemNode in advNode)
                        {
                            //找到归属的组
                            string groupName = itemNode.text;
                            ProcessTags(lstTags, itemNode.sub, groupName);
                        }
                        resultBag.Add("Tags", lstTags);

                        #endregion
                    }



                }

                #region products  解析  
                var lstProducts = new ProductBaseCollection();
                resultBag.Add("Products", lstProducts);

                var itemListNode = pageJsonObj.mods.itemlist;
                if (null != itemListNode && itemListNode.data != null && null != itemListNode.data.auctions)
                {

                    foreach (var itemProduct in itemListNode.data.auctions)
                    {
                        TaobaoProduct modelProduct = this.ResolverProductDom(itemProduct);

                        if (null != modelProduct)
                        {
                            lstProducts.Add(modelProduct);
                        }

                    }

                }

                //淘宝的搜索列表 - 第一页的数据是进行了分片的，在加载html ；36条数据， 后续会进行一次jsonp的请求；加载12条数据
                if (webArgs.PageIndex == 0 && null != tskSilcedJsonpContent)
                {
                    string jsonpContent = tskSilcedJsonpContent.Result;
                    if (!string.IsNullOrEmpty(jsonpContent) && jsonpContent.Contains("API.CustomizedApi"))
                    {
                        int startIdx = jsonpContent.IndexOf(':') + 1;
                        int endIdx = jsonpContent.Length - startIdx - 3;
                        string pureJsonContent = jsonpContent.Substring(startIdx, endIdx);
                        var slicedJsonpResut = JsonConvert.DeserializeObject<TaobaoSlicedJsonpResut>(pureJsonContent);


                        if (null != slicedJsonpResut)
                        {

                            var itemList = slicedJsonpResut.itemlist;
                            if (null != itemList && itemList.auctions != null)
                            {

                                foreach (var itemProduct in itemList.auctions)
                                {
                                    TaobaoProduct modelProduct = this.ResolverProductDom(itemProduct);

                                    if (null != modelProduct)
                                    {
                                        lstProducts.Add(modelProduct);
                                    }

                                }
                            }

                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {

                PluginContext.Logger.Error(ex);
            }
            return resultBag;// string.Concat("has process input :" + content);
        }


        private static void ProcessTags(List<KeyWordTag> lstTags, IEnumerable<TaobaoPageJsonResut.sub> lstSub, string groupName)
        {
            if (null == lstTags)
            {
                lstTags = new List<KeyWordTag>();
            }


            foreach (var itemSub in lstSub)
            {
                var modelTag = new KeyWordTag();
                modelTag.Platform = SupportPlatformEnum.Taobao;
                modelTag.TagName = itemSub.text;
                modelTag.GroupShowName = groupName;
                modelTag.FilterFiled = itemSub.key;
                modelTag.Value = itemSub.value;

                lstTags.Add(modelTag);
            }

        }

        /// <summary>
        /// 解析商品节点
        /// </summary>
        /// <param name="modelProduct"></param>
        /// <param name="productDom"></param>
        private TaobaoProduct ResolverProductDom(TaobaoPageJsonResut.Auctions productDom)
        {
            TaobaoProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new TaobaoProduct();
            try
            {
                //id
                string itemId = productDom.nid;
                if (string.IsNullOrEmpty(itemId))
                {
                    return modelProduct;//凡是没有id 的商品，要么是广告 要么是其他非正常的商品
                }
                long.TryParse(itemId, out long _ItemId);
                modelProduct.ItemId = _ItemId;

                //title
                modelProduct.Title = productDom.title;
                modelProduct.ItemUrl = productDom.detail_url.GetHttpsUrl();

                //price
                var priceDom = productDom.view_price;
                if (null != priceDom)
                {
                    decimal.TryParse(priceDom, out decimal _price);
                    modelProduct.Price = _price;
                }
                //pic
                modelProduct.PicUrl = productDom.pic_url.GetHttpsUrl();
                //shop
                string shopId = productDom.user_id;
                long.TryParse(shopId, out long _shopId);
                //modelProduct.ShopId = _shopId;//天猫店铺id 在搜索列表未出现
                modelProduct.SellerId = _shopId;
                modelProduct.ShopUrl = string.Format("https://store.taobao.com/shop/view_shop.htm?user_number_id={0}", shopId);
                modelProduct.ShopName = productDom.nick;


                //status
                //成交量

                modelProduct.Biz30Day = productDom.view_sales;

                //评论量
                modelProduct.TotalBizRemarkCount = productDom.comment_count;
                modelProduct.RemarkUrl = productDom.comment_url;

                //卖家地址
                modelProduct.SellerAddress = productDom.item_loc;

                //是否天猫
                if (null != productDom.shopcard)
                {
                    modelProduct.IsTmall = productDom.shopcard.isTmall;
                }
                //是否金牌卖家 运费险等
                if (productDom.icon != null && productDom.icon.Any())
                {
                    foreach (var item in productDom.icon)
                    {
                        if (item.icon_key == "icon-service-jinpaimaijia")
                        {
                            modelProduct.IsGold = true;
                        }
                        else if (item.icon_key == "icon-service-baoxian")
                        {
                            modelProduct.IsHasYunfeiXian = true;
                        }
                        else if (item.icon_key == "icon-service-xinpin")
                        {
                            modelProduct.IsXinPin = true;
                        }
                        else if (item.icon_key == "icon-fest-ifashion")
                        {
                            modelProduct.IsFashion = true;
                        }

                    }


                }



            }
            catch (Exception ex)
            {
                PluginContext.Logger.Error(ex);
            }
            return modelProduct;
        }




    }
}
