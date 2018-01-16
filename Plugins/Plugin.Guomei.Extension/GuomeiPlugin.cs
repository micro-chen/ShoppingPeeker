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
using NTCPMessage;

using NTCPMessage.Client;
using NTCPMessage.EntityPackage.Products;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;
using ShoppingPeeker.Plugins;


namespace Plugin.Guomei.Extension
{
    public class GuomeiPlugin : PluginBase<GuomeiPlugin>
    {

        public GuomeiPlugin()
        {
         
        }

        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new GuomeiPlugin();
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
            if (webArgs.IsNeedResolveHeaderTags == false)
            {
                resultUrl.IsNeedPreRequest = false;//国美的搜索页面和数据列表是分离的，如果需要加载标签，那么设置为true;否则直接请求json
            }

            StringBuilder sbSearchUrl = new StringBuilder("https://search.gome.com.cn/search?question=@###@");



            #region 品牌
            string facetsString = string.Empty;
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Guomei);
                if (currentPlatformBrands.Any())
                {
                    //sbSearchUrl.Append("&pzpq=0");
                    //sbSearchUrl.Append("&pzin=v4");

                    //多个品牌直接将id拼接为字符串，国美家的 是4位加密码进行的拼接组
                    string brandIds = string.Join("", currentPlatformBrands.Select(x => x.BrandId));
                    //sbSearchUrl.Append("&facets=").Append(brandIds);
                    facetsString += brandIds;

                    //有品牌参数的时候，国美前端有个附加参数 intcmp 没什么用，直接固定
                    sbSearchUrl.Append("&intcmp=search-9000001100-1");
                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Guomei);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Guomei);
                if (currentPlatformTag.Any())
                {

                    #region 分类
                    var catIdTag = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "catId");
                    if (null != catIdTag)
                    {
                        sbSearchUrl.Append("&catId=").Append(catIdTag.Value);
                    }
                    #endregion

                    string attrIds = string.Join("", currentPlatformTag.Select(x => x.Value));//facetsid 的组合
                    facetsString += attrIds;

                }

                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Guomei);
                if (null != otherPlatformTag)
                {
                    webArgs.KeyWord += " " + otherPlatformTag.TagName;
                }
            }

            if (!string.IsNullOrEmpty(facetsString))
            {
                sbSearchUrl.Append("&facets=").Append(facetsString);//国美是把所有的属性作为4字符串值作为参数解析的
            }
            #endregion

            #region 关键词
            sbSearchUrl.Replace("@###@", webArgs.KeyWord);//将关键词的占位符 进行替换
            #endregion

            #region  排序
            if (null == webArgs.OrderFiled)
            {
                sbSearchUrl.Append("&sort=00");//默认综合排序
            }
            else
            {
                sbSearchUrl.Append("&sort=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码

            sbSearchUrl.Append("&page=").Append(webArgs.PageIndex + 1);

            #endregion
            # region 杂项
            sbSearchUrl.Append("&deliv=0");
            sbSearchUrl.Append("&market=10");
            sbSearchUrl.Append("&instock=1");//仅显示有货
            sbSearchUrl.Append("&pzpq=0");
            sbSearchUrl.Append("&pzin=v4");
            #endregion
            resultUrl.Url = sbSearchUrl.ToString();
            return resultUrl;
        }


        /// <summary>
        /// 解析搜索页的Json获取商品的地址
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        private string ResolveSlicedSearchPageSilcedUrl(string htmlRequestString)
        {
            return string.Concat(htmlRequestString, "&bws=0&type=json");
        }


        /// <summary>
        /// 根据参数 查询国美的单品价格
        /// 国美的价格是跟商品查询拆分的，批量调用http 查询，速度极慢
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public GuomeiPriceJsonResult QuerySingleProductPrice(BaseFetchWebPageArgument webArgs)
        {

            GuomeiPriceJsonResult jsonResult = null;
            //产生价格查询任务   
            if (null==webArgs.AttachParas
                ||!webArgs.AttachParas.ContainsKey("pid")
                 || !webArgs.AttachParas.ContainsKey("skuid")
                )
            {
                throw new Exception("缺少必须的 pId 或者 skuId 参数！");
            }

            //从附加参数中获取值
            string pId = webArgs.AttachParas["pid"].ToString();
            string skuId = webArgs.AttachParas["skuid"].ToString();

            string unixTimeToken = JavascriptContext.getUnixTimestamp();

            if (!webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
            {
                throw new Exception("参数缺少连接字符串设置！");
            }
            var connStrConfig =  webArgs.SystemAttachParas["SoapTcpConnectionString"] as ShoppingWebCrawlerSection.ConnectionStringConfig;
            //json地址
            string urlOfPriceJson = string.Format("https://ss.gome.com.cn/search/v1/price/single/{0}/{1}/11010000/flag/item/fn1?callback=fn1&_={2}",
                pId,
                skuId,
                unixTimeToken);

            //创建新的页面查询参数，防止多线程并发导致参数被混淆
            var queryArgs = new GuomeiFetchWebPageArgument { KeyWord = webArgs.KeyWord };
            queryArgs.ResolvedUrl = new ResolvedSearchUrlWithParas { Url = urlOfPriceJson };

            using (var conn = new SoapTcpConnection(connStrConfig))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                //发送soap
                var soapCmd = new SoapMessage() { Head = CommandConstants.CMD_FetchPage };
                soapCmd.Body = JsonConvert.SerializeObject(queryArgs);
                var dataContainer = conn.SendSoapMessage(soapCmd);
                if (null != dataContainer && dataContainer.Status == 1)
                {
                    string jsonpDataPrice = dataContainer.Result;
                    if (!string.IsNullOrEmpty(jsonpDataPrice) && jsonpDataPrice.Contains("fn1("))
                    {
                        //GuomeiPriceJsonResult
                        int startPos = "fn1(".Length;
                        int endPos = jsonpDataPrice.Length - startPos - 1;
                         string jsonPrice= jsonpDataPrice.Substring(startPos, endPos);
                         jsonResult = JsonConvert.DeserializeObject<GuomeiPriceJsonResult>(jsonPrice);

                    }
                }
                else
                {
                    StringBuilder errMsg = new StringBuilder("抓取国美价格请求失败！参数：");
                    errMsg.Append(soapCmd.Body);
                    if (null != dataContainer && !string.IsNullOrEmpty(dataContainer.ErrorMsg))
                    {
                        errMsg.Append("；服务端错误消息：")
                            .Append(dataContainer.ErrorMsg);
                    }
                    PluginContext.Logger.Error(errMsg.ToString());
                }


            }

            return jsonResult;

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

            ShoppingWebCrawlerSection.ConnectionStringConfig connStrConfig = null;

            try
            {
                //开启查询头部筛选

                if (webArgs.IsNeedResolveHeaderTags == true)
                {

                    //创建html 文档对象
                    HtmlParser htmlParser = new HtmlParser();

                    string searchHtmlContent = "";
                    ////1 打开tcp 链接 
                    ////2 发送参数
                    ////3 解析结果
                    if (webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
                    {
                        connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as ShoppingWebCrawlerSection.ConnectionStringConfig;

                        //请求搜索页面的html
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
                                searchHtmlContent = dataContainer.Result;
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

                    }


                    if (!string.IsNullOrEmpty(searchHtmlContent) && !searchHtmlContent.Contains("没有找到您想要"))
                    {
                        //创建html 文档
                        var htmlDoc = htmlParser.Parse(searchHtmlContent);
                        var div_filterDoms = htmlDoc.QuerySelector("div#module-facet");

                        if (null != div_filterDoms)
                        {


                            var div_BrandsDom = div_filterDoms.QuerySelector("div#facets-category-brand");

                            #region 品牌解析
                            var lstBrands = new List<BrandTag>();
                            if (null != div_BrandsDom)
                            {
                                //从属性区域解析dom-品牌内容
                                var brandULDom = div_BrandsDom.QuerySelector("ul.category-brand");

                                if (null != brandULDom)
                                {
                                    var li_DomArray = brandULDom.QuerySelectorAll("li");
                                    foreach (var itemLiDom in li_DomArray)
                                    {
                                        var itemADom = itemLiDom.FirstElementChild;//<li>元素下的<a>

                                        var model = new BrandTag();
                                        model.Platform = SupportPlatformEnum.Guomei;
                                        model.FilterField = "facets";//使用的过滤字段参数
                                                                     //var urlBrand = itemADom.GetAttribute("href");

                                        model.BrandId = itemADom.GetAttribute("facetsid");

                                        model.BrandName = itemADom.TextContent.Replace("\n", "").Replace("\t", "");
                                        model.CharIndex = itemLiDom.GetAttribute("brand-value");//定位字符

                                        lstBrands.Add(model);
                                    }
                                }
                            }
                            resultBag.Add("Brands", lstBrands);

                            #endregion


                            //category 解析  
                            //普通分类
                            var div_AttrsDom_Category_CommonList = div_filterDoms.QuerySelectorAll("div.facets-category.facets-category-common");
                            //高级分类
                            var div_AttrsDom_AdvancedList = div_filterDoms.QuerySelectorAll("div.facets-category.facets-category-syn");

                            var lstTags = new List<KeyWordTag>();
                            var blockList = new BlockingCollection<KeyWordTag>();
                            var taskArray = new List<Task>();

                            //普通分类tag 解析
                            if (null != div_AttrsDom_Category_CommonList)
                            {

                                // PLINQ 的操作 
                                //div_AttrsDom_CategoryList.AsParallel().ForAll((x) => { })

                                for (int i = 0; i < div_AttrsDom_Category_CommonList.Length; i++)
                                {

                                    var itemCategory = div_AttrsDom_Category_CommonList[i];
                                    var taskResolveAEmelems = Task.Factory.StartNew(() =>
                                    {


                                            //找到归属的组
                                            string groupName = itemCategory.QuerySelector("span.fc-key").TextContent;

                                        var childLiADomArray = itemCategory.QuerySelectorAll("div.category-normal>ul>li>a");
                                        foreach (var itemADom in childLiADomArray)
                                        {
                                            var modelTag = new KeyWordTag();
                                            modelTag.Platform = SupportPlatformEnum.Guomei;
                                            modelTag.TagName = itemADom.TextContent;//标签名称
                                                modelTag.GroupShowName = groupName;

                                            modelTag.FilterFiled = "facets";
                                            modelTag.Value = itemADom.GetAttribute("facetsid");
                                                //----解析 a标签完毕-------
                                                blockList.Add(modelTag);

                                        }

                                    });
                                    //将并行任务放到数组
                                    taskArray.Add(taskResolveAEmelems);

                                }

                            }

                            //高级筛选 的解析
                            if (null != div_AttrsDom_AdvancedList)
                            {
                                for (int i = 0; i < div_AttrsDom_AdvancedList.Length; i++)
                                {

                                    var itemSline = div_AttrsDom_AdvancedList[i];
                                    var taskResolveAEmelems = Task.Factory.StartNew(() =>
                                    {


                                            //找到归属的组
                                            string groupName = itemSline.QuerySelector("span.fc-key").TextContent;

                                        var childLiADomArray = itemSline.QuerySelectorAll("ul.category-syn-list>li>a");
                                        foreach (var itemADom in childLiADomArray)
                                        {
                                            var modelTag = new KeyWordTag();
                                            modelTag.Platform = SupportPlatformEnum.Guomei;
                                            modelTag.TagName = itemADom.TextContent;//标签名称
                                                modelTag.GroupShowName = groupName;

                                            modelTag.FilterFiled = "facets";
                                            modelTag.Value = itemADom.GetAttribute("facetsid");


                                                //----解析 a标签完毕-------
                                                blockList.Add(modelTag);

                                        }

                                    });
                                    //将并行任务放到数组
                                    taskArray.Add(taskResolveAEmelems);

                                }
                            }


                            var safeTaskArray = taskArray.Where(x => null != x).ToArray();
                            Task.WaitAll(safeTaskArray);
                            lstTags = blockList.ToList();
                            resultBag.Add("Tags", lstTags);


                        }
                    }

                }



                #region products  解析
                //解析国美的json 列表地址
                string jsonData = "";
                ////1 打开tcp 链接 
                ////2 发送参数
                ////3 解析结果

                if (webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
                {


                    connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as ShoppingWebCrawlerSection.ConnectionStringConfig;

                    //json地址
                    string urlOfSlicedJson = this.ResolveSlicedSearchPageSilcedUrl(webArgs.ResolvedUrl.Url);

                    webArgs.ResolvedUrl = new ResolvedSearchUrlWithParas { Url = urlOfSlicedJson };
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
                            jsonData = dataContainer.Result;
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

                }



                var lstProducts = new ProductBaseCollection();

                GuomeiPageJsonResut pageJsonObj = JsonConvert.DeserializeObject<GuomeiPageJsonResut>(jsonData);
                //判断结果的有效性
                if (null == pageJsonObj
                    || null == pageJsonObj.content
                    || null == pageJsonObj.content.prodInfo
                    || null == pageJsonObj.content.prodInfo.products
                    )
                {
                    return null;
                }

                //多任务并行解析商品
                ConcurrentDictionary<string, decimal> blockingList_ProductPrices = new ConcurrentDictionary<string, decimal>();
                List<Task> lstQueryPriceTask = new List<Task>();

                foreach (var itemProduct in pageJsonObj.content.prodInfo.products)
                {




                    GuomeiProduct modelProduct = this.ResolverProductDom(itemProduct);

                    if (null != modelProduct)
                    {
                        lstProducts.Add(modelProduct);
                    }
                }


                //foreach (var itemProduct in lstProducts)
                //{
                //    string key = string.Concat(itemProduct.ItemId, "-", itemProduct.SkuList.First().SkuId);
                //    if (blockingList_ProductPrices.ContainsKey(key))
                //    {
                //        decimal priceInRemote = blockingList_ProductPrices[key];
                //        itemProduct.Price = priceInRemote;
                //    }

                //}
                resultBag.Add("Products", lstProducts);

                #endregion


            }
            catch (Exception ex)
            {

                PluginContext.Logger.Error(ex);
            }

            return resultBag;// string.Concat("has process input :" + content);

        }

        /// <summary>
        /// 解析商品节点
        /// </summary>
        /// <param name="modelProduct"></param>
        /// <param name="productDom"></param>
        private GuomeiProduct ResolverProductDom(GuomeiPageJsonResut.ProductItem productDom)
        {
            GuomeiProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new GuomeiProduct();
            try
            {
                //id
                string itemId = productDom.pId;
                if (string.IsNullOrEmpty(itemId))
                {
                    return modelProduct;//凡是没有id 的商品，要么是广告 要么是其他非正常的商品
                }
                long.TryParse(itemId, out long _ItemId);
                modelProduct.ItemId = _ItemId;

                //title
                modelProduct.Title = productDom.alt;
                modelProduct.ItemUrl = string.Concat(productDom.sUrl.GetHttpsUrl(), "?intcmp=search-9000000700-1_1_2");

                //price
                //var priceDom = productDom.view_price;
                //if (null != priceDom)
                //{
                //    decimal.TryParse(priceDom, out decimal _price);
                //    modelProduct.Price = _price;
                //}
                //pic
                modelProduct.PicUrl = productDom.sImg.GetHttpsUrl();
                //shop
                string shopId = productDom.shopId;
                long.TryParse(shopId, out long _shopId);
                modelProduct.SellerId = _shopId;
                modelProduct.ShopUrl = productDom.mUrl.GetHttpsUrl();
                modelProduct.ShopName = productDom.sName;


                //status
                //成交量

                //modelProduct.Biz30Day = productDom.evaluateCount.ToString();

                //评论量
                modelProduct.TotalBizRemarkCount = productDom.evaluateCount.ToString();
                modelProduct.RemarkUrl = string.Concat(modelProduct.ItemUrl, "#gm-other-info");


                //卖家地址
                //modelProduct.SellerAddress = productDom.item_loc;

                //是否自营
                if (productDom.thirdProduct == false)
                {
                    modelProduct.IsSelfSale = true;
                }

                //规格
                modelProduct.SkuList = new List<SkuItem>();
                modelProduct.SkuList.Add(new SkuItem { SkuId = productDom.skuId });


            }
            catch (Exception ex)
            {
                PluginContext.Logger.Error(ex);
            }
            return modelProduct;
        }

    }
}
