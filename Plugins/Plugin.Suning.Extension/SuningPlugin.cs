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
using AngleSharp.Dom.Html;

namespace Plugin.Suning.Extension
{
    public class SuningPlugin : PluginBase<SuningPlugin>
    {


        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new SuningPlugin();
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

            resultUrl.IsNeedPreRequest = false;//苏宁的搜索页面和数据列表是分离的，直接在解析中进行内容请求，不需要预先请求


            StringBuilder sbSearchUrl = new StringBuilder("https://search.suning.com/@###@/");



            #region 品牌

            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Suning);
                if (currentPlatformBrands.Any())
                {


                    //多个品牌直接将id拼接为字符串，国美家的 是4位加密码进行的拼接组
                    string brandNames = string.Join(";", currentPlatformBrands.Select(x => x.BrandName));


                    sbSearchUrl.Append("&hf=brand_Name_FacetAll:").Append(brandNames);

                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Suning);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Suning);
                if (currentPlatformTag.Any())
                {

                    #region 分类
                    var catIdTag = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "ci");
                    if (null != catIdTag)
                    {
                        sbSearchUrl.Append("&ci=").Append(catIdTag.Value);
                    }
                    #endregion

                    //https://search.suning.com/羽绒服/&iy=0&sc=0&hf=solr_13696_attrId:收腰型;常规&st=0#search-path-box
                    string attrIds = string.Join(";", currentPlatformTag.Select(x => x.Value));
                    sbSearchUrl.Append("&cf=")
                        .Append(currentPlatformTag.First().FilterFiled)
                        .Append("_attrId:")
                        .Append(attrIds);

                }

                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Suning);
                if (null != otherPlatformTag)
                {
                    webArgs.KeyWord += " " + otherPlatformTag.TagName;
                }
            }


            #endregion

            #region 关键词
            sbSearchUrl.Replace("@###@", webArgs.KeyWord);//将关键词的占位符 进行替换
            #endregion

            #region  排序
            if (null == webArgs.OrderFiled)
            {
                sbSearchUrl.Append("&st=0");//默认综合排序
            }
            else
            {
                sbSearchUrl.Append("&st=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码

            sbSearchUrl.Append("&cp=").Append(webArgs.PageIndex);

            #endregion
            # region 杂项
            sbSearchUrl.Append("&iy=0");
            sbSearchUrl.Append("&sc=0");

            sbSearchUrl.Append("#second-filter");

            #endregion
            resultUrl.Url = sbSearchUrl.ToString();
            return resultUrl;
        }

        /// <summary>
        /// 解析搜索页的Json获取商品的地址
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        private string ResolveSlicedSearchPageSilcedUrl(BaseFetchWebPageArgument webArgs)
        {
            StringBuilder sb_ajaxUrl = new StringBuilder("https://search.suning.com/emall/searchProductList.do?keyword=");
            //关键词
            sb_ajaxUrl.Append(webArgs.KeyWord);

            var urlParas = HttpUtility.ParseQueryString(webArgs.ResolvedUrl.Url);

            //类目
            string ciValue = urlParas["ci"];
            if (!string.IsNullOrEmpty(ciValue))
            {
                sb_ajaxUrl.Append("&ci=").Append(ciValue);
            }

            sb_ajaxUrl.Append("&pg=01");
            //分页
            string cpValue = urlParas["cp"];
            if (string.IsNullOrEmpty(cpValue))
            {
                cpValue = "0";
            }
            sb_ajaxUrl.Append("&cp=").Append(cpValue);

            sb_ajaxUrl.Append("&il=0");

            //排序
            string stValue = urlParas["st"];
            if (string.IsNullOrEmpty(stValue))
            {
                stValue = "0";
            }
            sb_ajaxUrl.Append("&st=").Append(stValue);

            sb_ajaxUrl.Append("&iy=-1");
            //hf 参数 cf 参数，拼接到hf 参数
            //品牌类别
            string hfValue = urlParas["hf"];
            string cfValue = urlParas["cf"];
            sb_ajaxUrl.Append("&hf=").Append(string.Concat(hfValue, ",", cfValue));

            sb_ajaxUrl.Append("&n=1&sc=0&sesab=ABAAA&id=IDENTIFYING&cc=010");
            return sb_ajaxUrl.ToString();
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

            WebCrawlerConnection connStrConfig = null;

            try
            {
                //开启查询头部筛选
                string searchHtmlContent = "";
                //创建html 文档对象
                HtmlParser htmlParser = new HtmlParser();
                //创建html 文档
                IHtmlDocument htmlDoc = null;

                if (webArgs.IsNeedResolveHeaderTags == true || webArgs.PageIndex == 0)
                {



                    ////1 打开tcp 链接 
                    ////2 发送参数
                    ////3 解析结果
                    if (webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
                    {
                        connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as WebCrawlerConnection;

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
                                StringBuilder errMsg = new StringBuilder("抓取【苏宁】网页请求失败！参数：");
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


                    if (!string.IsNullOrEmpty(searchHtmlContent) && !searchHtmlContent.Contains("no-result-tips"))
                    {
                        //文档转换
                        htmlDoc = htmlParser.Parse(HttpUtility.HtmlDecode(searchHtmlContent));

                        if (searchHtmlContent.Contains("search-opt"))
                        {
                            //普通的搜索列表页
                            ViewFilterContent_CommonTheme(ref resultBag, htmlDoc, searchHtmlContent);
                        }
                        else if (searchHtmlContent.Contains("filter-container"))
                        {
                            //没有左侧活动的100条数据列表
                            ViewFilterContent_Big100Theme(ref resultBag, htmlDoc, searchHtmlContent);
                        }
                        else
                        {
                            throw new Exception("未能识别的苏宁检索列表皮肤！");
                        }
                    }

                }



                #region products  解析
                //解析苏宁的商品 列表
                string htmlProductList = string.Empty;

                if (webArgs.PageIndex == 0 && null != htmlDoc)
                {

                    //首页的话，那么直接解析第一次请求的页面内容列表
                    htmlProductList = htmlDoc.QuerySelector("div#filter-results").OuterHtml;
                }
                else
                {
                    //如果不是第一页，那么开启ajax 查询加载

                    if (webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
                    {


                        connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as WebCrawlerConnection;

                        //json地址
                        string urlOfSlicedJson = this.ResolveSlicedSearchPageSilcedUrl(webArgs);

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
                                htmlProductList = dataContainer.Result;
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
                }


                var lstProducts = new ProductBaseCollection();

                //判断结果的有效性
                if (string.IsNullOrEmpty(htmlProductList))
                {
                    return null;
                }

                //多任务并行解析商品
                ConcurrentDictionary<string, SuningPriceJsonResult.PriceItem> blockingList_ProductPrices = new ConcurrentDictionary<string, SuningPriceJsonResult.PriceItem>();
                List<Task> lstQueryPriceTask = new List<Task>();


                //task1:通过正则 找出商品的id 集合，发送请求价格
                string pattern_FindItemId = @"li.*class.*product.*(\d{10}-\d+).*?";
                var matchs = Regex.Matches(htmlProductList, pattern_FindItemId, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (null != matchs && matchs.Count > 0)
                {
                    List<string> lstItemIds = new List<string>();
                    int counter = 0;
                    StringBuilder sb_tempIds = new StringBuilder();
                    foreach (Match item in matchs)
                    {
                        string itemId = item.Groups[1].Value;

                        if (string.IsNullOrEmpty(itemId))
                        {
                            continue;
                        }
                        string[] arry_id = itemId.Split('-');
                        string ventor = arry_id[0];
                        string id = arry_id[1];
                        lstItemIds.Add(id);
                        /*补位前缀+商品id ；总长度为 18，：000000000152709847_,000000000128866947_,000000000193392956_,
                        //自营和第三方的不同
                        //第三方,需要这种格式：000000000744752274__2_0070062935, 也就是后面再跟上：_2_{经销商编码}
                        */
                        string idFullString = id.PadLeft(18, '0');//长度不18 用0补充左边的位
                        sb_tempIds.Append(idFullString).Append("_");//
                        if (!ventor.Equals("0000000000"))
                        {
                            sb_tempIds.Append("_2_").Append(ventor);
                        }
                        sb_tempIds.Append(",");//end
                        counter += 1;

                        if (counter >= 10)
                        {
                            //构成一个新的请求m进行jsonp查询价格
                            string priceUrl = string.Format(
                                "https://ds.suning.cn/ds/generalForTile/{0}-010-2-0000000000-1--ds0000000002295.jsonp?callback=ds0000000002295",
                                sb_tempIds.ToString()
                                 );
                            var tsk_Price = this.QueryPriceAsync(priceUrl,
                                webArgs.KeyWord,
                                connStrConfig,
                                blockingList_ProductPrices);

                            lstQueryPriceTask.Add(tsk_Price);


                            //清除本次buffer
                            sb_tempIds.Clear();
                            counter = 0;
                        }
                    }


                }
                //task2:解析商品列表html
                var domsOfPriceList = htmlParser.Parse(htmlProductList).QuerySelectorAll("li.product");
                foreach (var itemProduct in domsOfPriceList)
                {
                    SuningProduct modelProduct = this.ResolverProductDom(itemProduct);

                    if (null != modelProduct)
                    {
                        lstProducts.Add(modelProduct);
                    }
                }

                //等待价格查询完毕
                var safeTaskArray = lstQueryPriceTask.Where(x => null != x).ToArray();
                Task.WaitAll(safeTaskArray);

                foreach (var itemProduct in lstProducts)
                {
                    SuningProduct modelProduct = itemProduct as SuningProduct;

                    string key = modelProduct.ItemId.ToString().PadLeft(18, '0');//长度不18 用0补充左边的位 string.Concat(modelProduct.BizCode, modelProduct.ItemId);
                    if (blockingList_ProductPrices.ContainsKey(key))
                    {
                        var priceInRemote = blockingList_ProductPrices[key];
                        if (null != priceInRemote)
                        {
                            modelProduct.Price = priceInRemote.price ?? 0;
                            modelProduct.BizCode = priceInRemote.bizCode;
                            //modelProduct.ShopId= log priceInRemote.bizCode;
                            if (modelProduct.IsSelfSale == false)
                            {
                                modelProduct.ShopName = priceInRemote.vendorName;
                            }

                        }

                    }

                }
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
        /// 异步查询价格
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keyword"></param>
        /// <param name="connStrConfig"></param>
        /// <param name="dataContainer"></param>
        /// <returns></returns>
        private Task QueryPriceAsync(string url, string keyword, WebCrawlerConnection connStrConfig, ConcurrentDictionary<string, SuningPriceJsonResult.PriceItem> priceContainer)
        {

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(keyword))
            {
                throw new Exception("苏宁查询价格必须输入关键词和地址！");
            }
            return Task.Factory.StartNew(() =>
            {
                //json地址

                var webArgs = new BaseFetchWebPageArgument();
                webArgs.Platform = SupportPlatformEnum.Suning;
                webArgs.KeyWord = keyword;
                webArgs.ResolvedUrl = new ResolvedSearchUrlWithParas { Url = url };

                string htmlPriceList = "";

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
                        htmlPriceList = dataContainer.Result;
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

                if (string.IsNullOrEmpty(htmlPriceList))
                {
                    return;
                }
                //jsonp请求失败结果
                if (!htmlPriceList.Contains("ds000000000"))
                {
                    PluginContext.Logger.Error("苏宁请求价格失败：" + url);
                }

                int startPos = htmlPriceList.IndexOf('{');
                int endPos = htmlPriceList.Length - startPos - 2;

                string jsonData = htmlPriceList.Substring(startPos, endPos);
                var dataJsonList = JsonConvert.DeserializeObject<SuningPriceJsonResult>(jsonData);
                if (null != dataJsonList && dataJsonList.status == 200)
                {
                    foreach (var item in dataJsonList.rs)
                    {
                        priceContainer.TryAdd(item.cmmdtyCode, item);
                    }
                }
            });
        }
        /// <summary>
        /// 对苏宁普通皮肤的头部筛选区域的解析
        /// 普通搜索列表-是有左侧的推广的-60个商品的搜索列表页面
        /// </summary>
        /// <param name="resultBag"></param>
        ///  <param name="htmlDoc"></param>
        /// <param name="searchHtmlContent"></param>
        private void ViewFilterContent_CommonTheme(ref Dictionary<string, object> resultBag, IHtmlDocument htmlDoc, string searchHtmlContent)
        {


            var div_filterDoms = htmlDoc.QuerySelector("div#search-opt");

            if (null != div_filterDoms)
            {


                var div_BrandsDom = div_filterDoms.QuerySelector("dl#brand_Name_FacetAll");

                #region 品牌解析
                var lstBrands = new List<BrandTag>();
                if (null != div_BrandsDom)
                {
                    List<IElement> lstBrandADoms = new List<IElement>();
                    //从属性区域解析dom-品牌内容
                    //1 可见的品牌
                    var brandADomList = div_BrandsDom.QuerySelectorAll("div.brand-item>div.clearfix>a");
                    if (null != brandADomList)
                    {
                        lstBrandADoms.AddRange(brandADomList.AsEnumerable());
                    }
                    //2 其他文本品牌
                    var txtBrandDom = div_BrandsDom.QuerySelector("textarea");
                    if (null != txtBrandDom)
                    {
                        var docBrand = new HtmlParser().Parse(HttpUtility.HtmlDecode(txtBrandDom.InnerHtml));
                        var notSeeBrandDoms = docBrand.QuerySelectorAll("a");
                        if (null != notSeeBrandDoms)
                        {
                            lstBrandADoms.AddRange(notSeeBrandDoms.AsEnumerable());
                        }
                    }

                    foreach (var itemADom in lstBrandADoms)
                    {


                        var model = new BrandTag();
                        model.Platform = SupportPlatformEnum.Suning;
                        model.FilterField = itemADom.GetAttribute("filter_id"); //使用的过滤字段参数
                        model.BrandId = itemADom.GetAttribute("filter_value");

                        model.BrandName = itemADom.GetAttribute("title").Replace("\n", "").Replace("\t", "");
                        model.CharIndex = itemADom.GetAttribute("class")[0].ToString();//定位字符
                        //var imgDom = itemADom.QuerySelector("img");
                        //if (null != imgDom)
                        //{
                        //    model.IconUrl = imgDom.GetAttribute("src").GetHttpsUrl();
                        //}

                        lstBrands.Add(model);
                    }

                }
                resultBag.Add("Brands", lstBrands);

                #endregion


                //category 解析  
                //相关分类
                var div_AttrsDom_Category = div_filterDoms
                    .QuerySelector("dl.dir-section");
                IHtmlCollection<IElement> div_AttrsDom_Category_CommonList = null;
                if (null != div_AttrsDom_Category)
                {
                    div_AttrsDom_Category_CommonList = div_AttrsDom_Category.QuerySelectorAll("div.item>a");

                }

                //普通tags
                var div_AttrsDom_CommonTagList = div_filterDoms.QuerySelectorAll("dl")
                    .Where(
                            x => x.ClassList.Contains("prive-section") || x.ClassList.Contains("model-section")
                    ).ToArray();

                //高级分类
                var div_AttrsDom_AdvancedList = div_filterDoms.QuerySelectorAll("dl#other-section>dd>ul>li");

                var lstTags = new List<KeyWordTagGroup>();
                var blockList = new BlockingCollection<KeyWordTagGroup>();
                var taskArray = new List<Task>();

                //相关分类tag 解析
                if (null != div_AttrsDom_Category_CommonList)
                {

                    // PLINQ 的操作 
                    //div_AttrsDom_CategoryList.AsParallel().ForAll((x) => { })

                    var tskBrand = Task.Factory.StartNew(() =>
                    {
                        //找到归属的组
                        string groupName = "相关分类";//itemCategory.QuerySelector("span.fc-key").TextContent;
                        var tagGroup = new KeyWordTagGroup(groupName);

                        //var childLiADomArray = itemCategory.QuerySelectorAll("div.category-normal>ul>li>a");
                        foreach (var itemADom in div_AttrsDom_Category_CommonList)
                        {
                            var modelTag = new KeyWordTag();
                            modelTag.Platform = SupportPlatformEnum.Suning;
                            modelTag.TagName = itemADom.TextContent;//标签名称
                            modelTag.GroupShowName = groupName;

                            modelTag.FilterFiled = "ci";
                            modelTag.Value = itemADom.GetAttribute("id");

                            tagGroup.Tags.Add(modelTag);
                        }

                        //----解析 a标签完毕-------
                        blockList.Add(tagGroup);
                    });

                    //将并行任务放到数组
                    taskArray.Add(tskBrand);

                }



                //普通tags 解析
                if (null != div_AttrsDom_CommonTagList)
                {

                    // PLINQ 的操作 
                    //div_AttrsDom_CategoryList.AsParallel().ForAll((x) => { })

                    for (int i = 0; i < div_AttrsDom_CommonTagList.Length; i++)
                    {

                        var itemCategory = div_AttrsDom_CommonTagList[i];
                        var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                        {

                            var itemSpceialCategory = paraItem as IElement;
                        //找到归属的组
                        string groupName = itemSpceialCategory.QuerySelector("dt").TextContent;
                            var tagGroup = new KeyWordTagGroup(groupName);
                            var childLiADomArray = itemSpceialCategory.QuerySelectorAll("div.clearfix>a");
                            foreach (var itemADom in childLiADomArray)
                            {
                                var modelTag = new KeyWordTag();
                                modelTag.Platform = SupportPlatformEnum.Suning;
                                modelTag.TagName = itemADom.GetAttribute("title");//标签名称
                                modelTag.GroupShowName = groupName;

                                modelTag.FilterFiled = itemSpceialCategory.GetAttribute("id");//根节点的筛选属性

                                modelTag.Value = itemADom.GetAttribute("id");

                                tagGroup.Tags.Add(modelTag);

                            }
                        //----解析 a标签完毕-------
                        blockList.Add(tagGroup);

                        }, itemCategory, TaskCreationOptions.None);
                        //将并行任务放到数组
                        taskArray.Add(taskResolveAEmelems);

                    }

                }


                //高级筛选 的解析---不启用
                ////////if (null != div_AttrsDom_AdvancedList)
                ////////{
                ////////    for (int i = 0; i < div_AttrsDom_AdvancedList.Length; i++)
                ////////    {

                ////////        var itemSline = div_AttrsDom_AdvancedList[i];
                ////////        var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                ////////        {

                ////////            var itemAdvancedCategory = paraItem as IElement;

                ////////        //找到归属的组
                ////////        string groupName = itemAdvancedCategory.QuerySelector("a.a-item>span").TextContent;
                ////////            if (groupName.Equals("全部分类"))
                ////////            {
                ////////                return;
                ////////            }
                ////////            var tagGroup = new KeyWordTagGroup(groupName);

                ////////            var childLiADomArray = itemAdvancedCategory.QuerySelectorAll("div.list-item>div>a");
                ////////            foreach (var itemADom in childLiADomArray)
                ////////            {
                ////////                var modelTag = new KeyWordTag();
                ////////                modelTag.Platform = SupportPlatformEnum.Suning;
                ////////                modelTag.TagName = itemADom.GetAttribute("title");//标签名称
                ////////                modelTag.GroupShowName = groupName;

                ////////                modelTag.FilterFiled = itemAdvancedCategory.GetAttribute("id");//根节点的筛选属性
                ////////                modelTag.Value = itemADom.GetAttribute("id");

                ////////                tagGroup.Tags.Add(modelTag);


                ////////            }
                ////////        //----解析 a标签完毕-------
                ////////        blockList.Add(tagGroup);

                ////////        }, itemSline, TaskCreationOptions.None);
                ////////        //将并行任务放到数组
                ////////        taskArray.Add(taskResolveAEmelems);

                ////////    }
                ////////}


                var safeTaskArray = taskArray.Where(x => null != x).ToArray();
                Task.WaitAll(safeTaskArray);
                lstTags = blockList.ToList();
                resultBag.Add("Tags", lstTags);


            }

        }

        /// <summary>
        /// 没有左侧推广的列表，100条商品数据
        /// </summary>
        /// <param name="resultBag"></param>
        ///  <param name="htmlDoc"></param>
        /// <param name="searchHtmlContent"></param>
        private void ViewFilterContent_Big100Theme(ref Dictionary<string, object> resultBag, IHtmlDocument htmlDoc, string searchHtmlContent)
        {



            var div_filterDoms = htmlDoc.QuerySelector("div.advanced-filter.filter-precise");

            if (null != div_filterDoms)
            {


                var div_BrandsDom = div_filterDoms.QuerySelector("div#brand_Name_FacetAll");

                #region 品牌解析
                var lstBrands = new List<BrandTag>();
                if (null != div_BrandsDom)
                {
                    //从属性区域解析dom-品牌内容
                    List<IElement> lstBrandADoms = new List<IElement>();
                    //从属性区域解析dom-品牌内容
                    //1 可见的品牌
                    var brandADomList = div_BrandsDom.QuerySelectorAll("div.brand-item>div.clearfix>a");
                    if (null != brandADomList)
                    {
                        lstBrandADoms.AddRange(brandADomList.AsEnumerable());
                    }
                    //2 其他文本品牌
                    var txtBrandDom = div_BrandsDom.QuerySelector("textarea");
                    if (null != txtBrandDom)
                    {
                        var docBrand = new HtmlParser().Parse(HttpUtility.HtmlDecode(txtBrandDom.InnerHtml));
                        var notSeeBrandDoms = docBrand.QuerySelectorAll("a");
                        if (null != notSeeBrandDoms)
                        {
                            lstBrandADoms.AddRange(notSeeBrandDoms.AsEnumerable());
                        }
                    }

                    foreach (var itemADom in brandADomList)
                    {


                        var model = new BrandTag();
                        model.Platform = SupportPlatformEnum.Suning;
                        model.FilterField = itemADom.GetAttribute("filter_id"); //使用的过滤字段参数
                        model.BrandId = itemADom.GetAttribute("filter_value");

                        model.BrandName = itemADom.GetAttribute("title").Replace("\n", "").Replace("\t", "");
                        string idxString = itemADom.GetAttribute("class");//定位字符
                        if (!string.IsNullOrEmpty(idxString))
                        {
                            model.CharIndex = idxString.Last().ToString();//最后一个字符 ：s-brand.choose.P
                        }
                        var imgDom = itemADom.QuerySelector("a>img");
                        if (null != imgDom)
                        {
                            model.IconUrl = imgDom.GetAttribute("src").GetHttpsUrl();
                        }

                        lstBrands.Add(model);
                    }

                }
                resultBag.Add("Brands", lstBrands);

                #endregion


                //category 解析  

                //普通tags
                var div_AttrsDom_CommonList = div_filterDoms.QuerySelectorAll("div.filter-section")
                    .Where(
                    (x) =>
                    {
                    //id 不为空 并且不是第一个品牌元素的节点
                    string id = x.GetAttribute("id");
                        if (string.IsNullOrEmpty(id))
                        {
                            return false;
                        }
                        if (id != "brand_Name_FacetAll")
                        {
                            return true;
                        }
                        return false;
                    }
                    ).ToArray();

                //高级分类
                var div_AttrsDom_AdvancedList = div_filterDoms.QuerySelectorAll("div.filter-section.b-solid.f-other>div.s-right>div.other-opts");

                var lstTags = new List<KeyWordTag>();
                var blockList = new BlockingCollection<KeyWordTag>();
                var taskArray = new List<Task>();

                //普通tags解析
                if (null != div_AttrsDom_CommonList)
                {

                    // PLINQ 的操作 
                    //div_AttrsDom_CategoryList.AsParallel().ForAll((x) => { })

                    for (int i = 0; i < div_AttrsDom_CommonList.Length; i++)
                    {

                        var itemCategory = div_AttrsDom_CommonList[i];
                        var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                        {

                            var itemSpceialCategory = paraItem as IElement;
                        //找到归属的组
                        string groupName = itemSpceialCategory.QuerySelector("div.s-left>label").TextContent;

                            var childLiADomArray = itemSpceialCategory.QuerySelectorAll("div.item-container>a");
                            foreach (var itemADom in childLiADomArray)
                            {
                                var modelTag = new KeyWordTag();
                                modelTag.Platform = SupportPlatformEnum.Suning;
                                modelTag.TagName = itemADom.GetAttribute("title");//标签名称
                                modelTag.GroupShowName = groupName;

                                modelTag.FilterFiled = itemSpceialCategory.GetAttribute("id");//根节点的筛选属性

                                modelTag.Value = itemADom.GetAttribute("id");
                            //----解析 a标签完毕-------
                            blockList.Add(modelTag);

                            }

                        }, itemCategory, TaskCreationOptions.None);
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
                        var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                        {

                            var itemAdvancedCategory = paraItem as IElement;

                        //找到归属的组
                        string groupName = itemAdvancedCategory.QuerySelector("a.o-text>span").TextContent;
                            if (groupName.Equals("全部分类"))
                            {
                                return;
                            }
                            var childLiADomArray = itemAdvancedCategory.QuerySelectorAll("div.o-details>div.d-container>a");
                            foreach (var itemADom in childLiADomArray)
                            {
                                var modelTag = new KeyWordTag();
                                modelTag.Platform = SupportPlatformEnum.Suning;
                                modelTag.TagName = itemADom.GetAttribute("title");//标签名称
                                modelTag.GroupShowName = groupName;

                                modelTag.FilterFiled = itemAdvancedCategory.GetAttribute("id");//根节点的筛选属性
                                modelTag.Value = itemADom.GetAttribute("id");


                            //----解析 a标签完毕-------
                            blockList.Add(modelTag);

                            }

                        }, itemSline, TaskCreationOptions.None);
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

        /// <summary>
        /// 解析商品节点
        /// </summary>
        /// <param name="productDom"></param>
        private SuningProduct ResolverProductDom(IElement productDom)
        {
            SuningProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new SuningProduct();
            try
            {
                //id <li  isHwg=""  id="" name="" class="product      basic 617087532    0000000000-617087532  " lazy="true">
                string pattern_itemid = @"li.*class.*product.*(\d{10}-\d+).*?";
                var match = Regex.Match(productDom.OuterHtml, pattern_itemid, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (match == null)
                {
                    return modelProduct;//凡是没有id 的商品，要么是广告 要么是其他非正常的商品
                }
                string itemId = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(itemId))
                {
                    string[] arry_id = itemId.Split('-');
                    modelProduct.BizCode = arry_id[0];

                    long.TryParse(arry_id[1], out long _ItemId);
                    modelProduct.ItemId = _ItemId;
                }

                //shop

                var domVendor = productDom.QuerySelector("input.hidenInfo");
                if (null == domVendor)
                {
                    return null;//对于广告 众筹 等 不是有效的商品
                }
                string vendorType = domVendor.GetAttribute("vendorType");
                string shopId = domVendor.GetAttribute("vendor");
                if (!string.IsNullOrEmpty(shopId) && shopId != "0000000000" && vendorType != "0")
                {
                    long.TryParse(shopId, out long _shopId);
                    modelProduct.SellerId = _shopId;
                    modelProduct.ShopUrl = string.Format("https://shop.suning.com/{0}/index.html", shopId);
                    //modelProduct.ShopName = productDom.sName;---在请求价格的结果中，将经销商赋值
                }



                //price
                //var priceDom = productDom.view_price;
                //if (null != priceDom)
                //{
                //    decimal.TryParse(priceDom, out decimal _price);
                //    modelProduct.Price = _price;
                //}
                //pic
                var domImage = productDom.QuerySelector("img.search-loading");
                modelProduct.PicUrl = domImage.GetAttribute("src2").GetHttpsUrl();


                //title
                var domImge = domImage.ParentElement;
                var domTitle = productDom.QuerySelector("p.sell-point");
                if (null==domTitle)
                {
                    domTitle = productDom.QuerySelector("a.sellPoint");
                }
                modelProduct.Title = domTitle.TextContent;// domTitle.GetAttribute("title");
                modelProduct.ItemUrl = domImge.GetAttribute("href").GetHttpsUrl();



                //status
                //成交量

                //modelProduct.Biz30Day = productDom.evaluateCount.ToString();

                //评论量
                var domRemark = productDom.QuerySelector("a.num");
                if (null != domRemark)
                {
                    modelProduct.TotalBizRemarkCount = domRemark.TextContent;
                    modelProduct.RemarkUrl = domRemark.GetAttribute("href");
                }



                //卖家地址
                //modelProduct.SellerAddress = productDom.item_loc;

                //是否自营
                if (shopId == "0000000000" || vendorType == "0")
                {
                    modelProduct.IsSelfSale = true;
                    modelProduct.ShopName = "苏宁自营";
                }

                //规格
                //modelProduct.SkuList = new List<SkuItem>();
                //modelProduct.SkuList.Add(new SkuItem { SkuId = productDom.skuId });


            }
            catch (Exception ex)
            {
                PluginContext.Logger.Error(ex);
            }
            return modelProduct;
        }

    }
}
