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


namespace Plugin.Jingdong.Extension
{
    public class JingdongPlugin : PluginBase<JingdongPlugin>
    {



        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new JingdongPlugin();
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


            StringBuilder sbSearchUrl = new StringBuilder("https://search.jd.com/Search?keyword=@###@&enc=utf-8&wq=@###@");


            #region 品牌 规格 分类 都在参数ev 中 
            //例如：exbrand_娇兰（Guerlain）||NARS^1107_82376||8240^
            string paraBrandAndSkusEv = "";
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Jingdong);
                if (currentPlatformBrands.Any())
                {
                    //多个品牌用 , 号分割
                    string brandNames = string.Join("||", currentPlatformBrands.Select(x => x.BrandName));
                    paraBrandAndSkusEv = string.Concat("exbrand_", brandNames, "^");

                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Jingdong);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Jingdong);
                if (null != currentPlatformTag)
                {
                    //归属科目 cid2
                    var cid2Para = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "cid2");
                    if (null != cid2Para)
                    {
                        sbSearchUrl.Append("&cid2=").Append(cid2Para.Value);
                    }
                    //归属科目 cid3
                    var cid3Para = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "cid3");
                    if (null != cid3Para)
                    {
                        sbSearchUrl.Append("&cid3=").Append(cid3Para.Value);
                    }

                    var tagGroup = currentPlatformTag.GroupBy(x => x.FilterFiled);
                    string skuAttrs = "";
                    foreach (var itemGroup in tagGroup)
                    {
                        string key = itemGroup.Key + "_";//属性_
                        string values = string.Join("||", itemGroup.Select(x => x.Value));
                        skuAttrs += string.Concat(key, values);
                        skuAttrs += "^";
                    }
                    paraBrandAndSkusEv += skuAttrs;
                }
                if (!string.IsNullOrEmpty(paraBrandAndSkusEv))
                {
                    sbSearchUrl.Append("&ev=").Append(paraBrandAndSkusEv);
                }
                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Jingdong);
                if (null != otherPlatformTag)
                {
                    webArgs.KeyWord += " " + otherPlatformTag.TagName;
                }
            }
            #endregion

            #region 关键词
            sbSearchUrl.Replace("@###@", webArgs.KeyWord);
            #endregion

            #region  排序
            if (null != webArgs.OrderFiled && webArgs.OrderFiled.Rule != OrderRule.Default)
            {
                sbSearchUrl.Append("&psort=").Append(webArgs.OrderFiled.FieldValue);
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码
            int pageNumber = (webArgs.PageIndex * 2) + 1;//京东每页分割为2个子页，按照页索引0开始，倍乘2,然后加1 为正确的页码
            sbSearchUrl.Append("&page=").Append(pageNumber);

            //京东前后翻页的时候 需要这个s 参数，前为prev 参数 ,后翻为next 参数
            if (null != webArgs.AttachParas && webArgs.AttachParas.ContainsKey("jd_pager_s"))
            {
                sbSearchUrl.Append("&s=").Append(webArgs.AttachParas["jd_pager_s"]);
            }
            #endregion
            # region 杂项
            sbSearchUrl.Append("&qrst=1");
            sbSearchUrl.Append("&rt=1");
            sbSearchUrl.Append("&stop=1");
            sbSearchUrl.Append("&vt=2");
            sbSearchUrl.Append("&stock=1");

            #endregion
            resultUrl.Url = sbSearchUrl.ToString();
            return resultUrl;
        }

        /// <summary>
        /// 解析搜索页的剩余的,获取商品的地址
        /// </summary>
        /// <param name="webArgs"></param>
        /// <param name="next_start">下一页的起始位置（在加载page的时候 js 中s.init(1,200,"79万+","0",1,0,25,1,0,2);）</param>
        /// <param name="show_items">已经显示的条目 逗号分割的pid集合</param>
        /// <returns></returns>
        private string ResolveSlicedSearchPageSilcedUrl(BaseFetchWebPageArgument webArgs, int next_start, string show_items)
        {
            ResolvedSearchUrlWithParas resultUrl = new ResolvedSearchUrlWithParas();


            StringBuilder sbSearchUrl = new StringBuilder("https://search.jd.com/s_new.php?keyword=@###@&enc=utf-8");


            #region 品牌 规格 分类 都在参数ev 中 
            //例如：exbrand_娇兰（Guerlain）||NARS^1107_82376||8240^
            string paraBrandAndSkusEv = "";
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Jingdong);
                if (currentPlatformBrands.Any())
                {
                    //多个品牌用 , 号分割
                    string brandNames = string.Join("||", currentPlatformBrands.Select(x => x.BrandName));
                    paraBrandAndSkusEv = string.Concat("exbrand_", brandNames, "^");

                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Jingdong);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Jingdong);
                if (null != currentPlatformTag)
                {
                    //归属科目 cid2
                    var cid2Para = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "cid2");
                    if (null != cid2Para)
                    {
                        sbSearchUrl.Append("&cid2=").Append(cid2Para.Value);
                    }
                    //归属科目 cid3
                    var cid3Para = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "cid3");
                    if (null != cid3Para)
                    {
                        sbSearchUrl.Append("&cid3=").Append(cid3Para.Value);
                    }

                    var tagGroup = currentPlatformTag.GroupBy(x => x.FilterFiled);
                    string skuAttrs = "";
                    foreach (var itemGroup in tagGroup)
                    {
                        string key = itemGroup.Key + "_";//属性_
                        string values = string.Join("||", itemGroup.Select(x => x.Value));
                        skuAttrs += string.Concat(key, values);
                        skuAttrs += "^";
                    }
                    paraBrandAndSkusEv += skuAttrs;
                }
                if (!string.IsNullOrEmpty(paraBrandAndSkusEv))
                {
                    sbSearchUrl.Append("&ev=").Append(paraBrandAndSkusEv);
                }
                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Jingdong);
                if (null != otherPlatformTag)
                {
                    webArgs.KeyWord += " " + otherPlatformTag.TagName;
                }
            }
            #endregion

            #region 关键词
            sbSearchUrl.Replace("@###@", webArgs.KeyWord);
            #endregion

            #region  排序
            if (null != webArgs.OrderFiled && webArgs.OrderFiled.Rule != OrderRule.Default)
            {
                sbSearchUrl.Append("&psort=").Append(webArgs.OrderFiled.FieldValue);
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码
            int pageNumber = (webArgs.PageIndex + 1) * 2;//京东每页分割为2个子页
            sbSearchUrl.Append("&page=").Append(pageNumber);
            sbSearchUrl.Append("&s=").Append(next_start);
            #endregion
            # region 杂项
            sbSearchUrl.Append("&qrst=1");
            sbSearchUrl.Append("&rt=1");
            sbSearchUrl.Append("&stop=1");
            sbSearchUrl.Append("&vt=2");
            sbSearchUrl.Append("&scrolling=y");
            sbSearchUrl.Append("&log_id=y");
            sbSearchUrl.Append("&tpl=3_L");
            sbSearchUrl.Append("&show_items=").Append(show_items);
            #endregion
            //resultUrl.Url = sbSearchUrl.ToString();

            return sbSearchUrl.ToString();

        }
        /// <summary>
        /// 执行内容解析
        /// </summary>
        ///<param name="webArgs"> </param> 
        /// <param name="content">要解析的内容</param>
        /// <returns>返回需要的字段对应的字典</returns>
        public override Dictionary<string, object> ResolveSearchPageContent(BaseFetchWebPageArgument webArgs, string content)
        {



            if (!content.Contains("在京东找到了"))
            {
                return null;//非法请求结果页面
            }

            var resultBag = new Dictionary<string, object>();

            try
            {




                //创建html 文档对象
                HtmlParser htmlParser = new HtmlParser();
                var htmlDoc = htmlParser.Parse(content);

                if (webArgs.IsNeedResolveHeaderTags == true)
                {

                    var div_filterDoms = htmlDoc.QuerySelector("div#J_selector.selector");

                    if (null != div_filterDoms)
                    {


                        var div_BrandsDom = div_filterDoms.QuerySelector("div.s-brand");

                        #region 品牌解析
                        var lstBrands = new List<BrandTag>();
                        if (null != div_BrandsDom)
                        {
                            //从属性区域解析dom-品牌内容
                            var brandULDom = div_BrandsDom.QuerySelector("ul.J_valueList");

                            if (null != brandULDom)
                            {
                                var li_DomArray = brandULDom.QuerySelectorAll("li");
                                foreach (var itemLiDom in li_DomArray)
                                {
                                    var itemADom = itemLiDom.FirstElementChild;//<li>元素下的<a>

                                    var model = new BrandTag();
                                    model.Platform = SupportPlatformEnum.Jingdong;
                                    model.FilterField = "ev";//使用的过滤字段参数
                                    var urlBrand = itemADom.GetAttribute("href");
                                    model.BrandName = itemADom.TextContent.Replace("\n", "").Replace("\t", "");


                                    model.BrandId = itemLiDom.GetAttribute("id");
                                    if (!string.IsNullOrEmpty(model.BrandId))
                                    {
                                        int stsartPos = model.BrandId.IndexOf('-') + 1;//id=brand-43244
                                        model.BrandId = model.BrandId.Substring(model.BrandId.IndexOf('-') + 1);
                                    }
                                    model.CharIndex = itemLiDom.GetAttribute("data-initial");//定位字符

                                    var imgDom = itemADom.QuerySelector("img");
                                    if (null != imgDom)
                                    {
                                        model.IconUrl = imgDom.GetAttribute("src").GetHttpsUrl();
                                    }


                                    lstBrands.Add(model);
                                }
                            }
                        }
                        resultBag.Add("Brands", lstBrands);

                        #endregion

                        // tags 解析
                        //var lstTags = new List<KeyWordTag> {
                        //new KeyWordTag {
                        //    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Jingdong,
                        //    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                        //} };

                        //category 解析  
                        var div_AttrsDom_CategoryList = div_filterDoms.QuerySelectorAll("div.J_selectorLine.s-category");
                        var div_AttrsDom_SlineList = div_filterDoms.QuerySelectorAll("div.J_selectorLine.s-line");
                        var div_AttrsDom_Senior = div_filterDoms.QuerySelector("div#J_selectorSenior");

                        var lstTags = new List<KeyWordTagGroup>();
                        var blockList = new BlockingCollection<KeyWordTagGroup>();
                        var taskArray = new List<Task>();
                        if (null != div_AttrsDom_CategoryList)
                        {

                            // PLINQ 的操作 
                            //div_AttrsDom_CategoryList.AsParallel().ForAll((x) => { })

                            for (int i = 0; i < div_AttrsDom_CategoryList.Length; i++)
                            {

                                var itemCate = div_AttrsDom_CategoryList[i];
                                var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                                {

                                    var itemCategory = paraItem as IElement;

                                    //找到归属的组
                                    string groupName = itemCategory.QuerySelector("div.sl-key").Children[0].TextContent;
                                    var tagGroup = new KeyWordTagGroup(groupName);

                                    var childLiADomArray = itemCategory.QuerySelectorAll("ul.J_valueList>li>a");
                                    foreach (var itemADom in childLiADomArray)
                                    {
                                        var modelTag = new KeyWordTag();
                                        modelTag.Platform = SupportPlatformEnum.Jingdong;
                                        modelTag.TagName = itemADom.TextContent;//标签名称
                                        modelTag.GroupShowName = groupName;
                                        string hrefString = itemADom.GetAttribute("href");
                                        var catValueParas = HttpUtility.ParseQueryString(hrefString);
                                        if (catValueParas.AllKeys.Contains("ev"))
                                        {
                                            modelTag.FilterFiled = "ev";
                                            modelTag.Value = catValueParas["ev"].Replace("^", "");
                                        }
                                        else if (catValueParas.AllKeys.Contains("cid2"))
                                        {
                                            modelTag.FilterFiled = "cid2";
                                            modelTag.Value = catValueParas["cid2"].Replace("#J_searchWrap", "");
                                        }
                                        else if (catValueParas.AllKeys.Contains("cid3"))
                                        {
                                            modelTag.FilterFiled = "cid3";
                                            modelTag.Value = catValueParas["cid3"].Replace("#J_searchWrap", "");
                                        }

                                        tagGroup.Tags.Add(modelTag);


                                    }
                                    //----解析 a标签完毕-------
                                    blockList.Add(tagGroup);

                                }, itemCate, TaskCreationOptions.None);
                                //将并行任务放到数组
                                taskArray.Add(taskResolveAEmelems);

                            }

                        }

                        //sline 的解析
                        if (null != div_AttrsDom_SlineList)
                        {
                            for (int i = 0; i < div_AttrsDom_SlineList.Length; i++)
                            {

                                var itemSline = div_AttrsDom_SlineList[i];
                                var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                                {
                                    var itemAdvancedCategory = paraItem as IElement;

                                    //找到归属的组
                                    string groupName = itemAdvancedCategory.QuerySelector("div.sl-key").Children[0].TextContent;
                                    if (groupName.Contains("高级选项"))
                                    {
                                        return;//高级筛选不再这处理
                                    }
                                    var tagGroup = new KeyWordTagGroup(groupName);

                                    var childLiADomArray = itemAdvancedCategory.QuerySelectorAll("ul.J_valueList>li>a");
                                    foreach (var itemADom in childLiADomArray)
                                    {
                                        var modelTag = new KeyWordTag();
                                        modelTag.Platform = SupportPlatformEnum.Jingdong;
                                        modelTag.TagName = itemADom.TextContent;//标签名称
                                        modelTag.GroupShowName = groupName;
                                        string hrefString = itemADom.GetAttribute("href");
                                        var catValueParas = HttpUtility.ParseQueryString(hrefString);
                                        if (catValueParas.AllKeys.Contains("ev"))
                                        {
                                            modelTag.FilterFiled = "ev";
                                            modelTag.Value = catValueParas["ev"].Replace("^", "");
                                        }
                                        else if (catValueParas.AllKeys.Contains("cid2"))
                                        {
                                            modelTag.FilterFiled = "cid2";
                                            modelTag.Value = catValueParas["cid2"].Replace("#J_searchWrap", "");
                                        }
                                        else if (catValueParas.AllKeys.Contains("cid3"))
                                        {
                                            modelTag.FilterFiled = "cid3";
                                            modelTag.Value = catValueParas["cid3"].Replace("#J_searchWrap", "");
                                        }




                                        tagGroup.Tags.Add(modelTag);
                                    }

                                    //----解析 a标签完毕-------
                                    blockList.Add(tagGroup);
                                }, itemSline, TaskCreationOptions.None);
                                //将并行任务放到数组
                                taskArray.Add(taskResolveAEmelems);

                            }
                        }

                        //高级选项的解析
                        if (null != div_AttrsDom_Senior)
                        {
                            var lstAdvDoms = div_AttrsDom_Senior.QuerySelectorAll("div.sl-tab-trigger>a.trig-item");
                            var lstTabContentItems = div_AttrsDom_Senior.QuerySelectorAll("div.sl-tab-cont-item");
                            if (null != lstAdvDoms)
                            {

                                for (int i = 0; i < lstAdvDoms.Length; i++)
                                {
                                    int cursor = i;//执行并行计算的时候 变量游标不要传递到task中，延迟运行的task，变量i 会被在外面循环更改！！导致溢出index
                                    var itemAdv = lstAdvDoms[i];

                                    var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                                    {

                                        var itemAdvanced2Category = paraItem as IElement;
                                        //找到归属的组
                                        string groupName = itemAdvanced2Category.Children[0].TextContent;

                                        if (groupName.Equals("其他分类"))
                                        {
                                            return;//不解析与当前关键词无关的分类信息
                                        }
                                        var tagGroup = new KeyWordTagGroup(groupName);

                                        if (null != lstTabContentItems[cursor])
                                        {
                                            var childLiADomArray = lstTabContentItems[cursor].QuerySelectorAll("ul.J_valueList>li>a");//找到匹配游标的内容组
                                            foreach (var itemADom in childLiADomArray)
                                            {
                                                var modelTag = new KeyWordTag();
                                                modelTag.Platform = SupportPlatformEnum.Jingdong;
                                                modelTag.TagName = itemADom.TextContent;//标签名称
                                                modelTag.GroupShowName = groupName;
                                                string hrefString = itemADom.GetAttribute("href");
                                                var catValueParas = HttpUtility.ParseQueryString(hrefString);
                                                if (catValueParas.AllKeys.Contains("ev"))
                                                {
                                                    modelTag.FilterFiled = "ev";
                                                    modelTag.Value = catValueParas["ev"].Replace("^", "");
                                                }
                                                else if (catValueParas.AllKeys.Contains("cid2"))
                                                {
                                                    modelTag.FilterFiled = "cid2";
                                                    modelTag.Value = catValueParas["cid2"].Replace("#J_searchWrap", "");
                                                }
                                                else if (catValueParas.AllKeys.Contains("cid3"))
                                                {
                                                    modelTag.FilterFiled = "cid3";
                                                    modelTag.Value = catValueParas["cid3"].Replace("#J_searchWrap", "");
                                                }


                                                tagGroup.Tags.Add(modelTag);


                                            }
                                            //----解析 a标签完毕-------
                                            blockList.Add(tagGroup);
                                        }


                                    }, itemAdv, TaskCreationOptions.None);
                                    //将并行任务放到数组
                                    taskArray.Add(taskResolveAEmelems);


                                }
                            }
                        }


                        var safeTaskArray = taskArray.Where(x => null != x).ToArray();
                        Task.WaitAll(safeTaskArray);
                        lstTags = blockList.ToList();
                        resultBag.Add("Tags", lstTags);


                    }
                }

                #region products  解析

                var lstProducts = new ProductBaseCollection();
                //多任务并行解析商品
                ConcurrentDictionary<string, ProductOrdered<JingdongProduct>> blockingList_Products = new ConcurrentDictionary<string, ProductOrdered<JingdongProduct>>();


                var div_J_ItemListDom = htmlDoc.QuerySelector("div#J_goodsList");
                if (null != div_J_ItemListDom)
                {
                    var lstProductDoms = new List<IElement>();
                    var div_productDomArray = div_J_ItemListDom.QuerySelectorAll("ul.gl-warp>li.gl-item");
                    if (null != div_productDomArray && div_productDomArray.Any())
                    {
                        lstProductDoms.AddRange(div_productDomArray);
                    }

                    #region 分片的页数据加载


                    //send request for load other data of first search page
                    //加载请求京东当前页面的后半页数据

                    var skuIds = div_productDomArray
                        .Select(x => { return x.GetAttribute("data-sku"); });

                    //设定排序对象
                    int counter_sku = 0;
                    foreach (var itemSkuId in skuIds)
                    {
                        blockingList_Products.TryAdd(itemSkuId, new ProductOrdered<JingdongProduct> { UniqKey = itemSkuId, IndexOrder = counter_sku });
                        counter_sku++;
                    }

                    var pids1 = div_productDomArray
                      .Select(x => { return x.GetAttribute("data-pid"); });
                    string show_items = string.Join(",", pids1);
                    int next_start = 0;
                    //	s.init(1,200,"79万+","0",1,0,25,1,0,2);
                    var pageInitDomMatch = Regex.Match(content, @"s\.init\((.*?)\);", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    if (null != pageInitDomMatch && pageInitDomMatch.Groups.Count >= 2)
                    {
                        string initContent = pageInitDomMatch.Groups[1].Value;
                        if (!string.IsNullOrEmpty(initContent))
                        {
                            string nextStr = initContent.Split(',')[6];//第6个参数
                            int.TryParse(nextStr, out next_start);
                        }
                    }


                    string htmlItemsContent = "";
                    ////1 打开tcp 链接 
                    ////2 发送参数
                    ////3 解析结果
                    if (webArgs.SystemAttachParas.ContainsKey("SoapTcpConnectionString"))
                    {


                        var connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as WebCrawlerConnection;

                        //重写解析地址-首页的分片jsonp地址
                        string urlOfSlicedJsonp = this.ResolveSlicedSearchPageSilcedUrl(webArgs, next_start, show_items);
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
                                htmlItemsContent = dataContainer.Result;
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

                    /// <summary>
                    /// 上一页的起始位置
                    /// </summary>
                    int pager_prev_start = 1;
                    /// <summary>
                    /// 下一页的起始位置
                    /// </summary>
                    int pager_next_start = 1;


                    if (!string.IsNullOrEmpty(htmlItemsContent))
                    {

                        var nextpageInitDomMatch = Regex.Match(htmlItemsContent, @"SEARCH\.page_html\((.*?)\);", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                        if (null != nextpageInitDomMatch && nextpageInitDomMatch.Groups.Count >= 2)
                        {
                            string initContent = nextpageInitDomMatch.Groups[1].Value;
                            if (!string.IsNullOrEmpty(initContent))
                            {
                                var parasArray = initContent.Split(',');
                                string nextStr = parasArray[4];//第5个参数
                                int.TryParse(nextStr, out pager_next_start);

                                string prevStr = parasArray[5];//第6个参数
                                int.TryParse(prevStr, out pager_prev_start);
                            }
                        }

                        var slicedHtmlDoc = htmlParser.Parse(htmlItemsContent);
                        var sliced_productDomArray = slicedHtmlDoc.QuerySelectorAll("li.gl-item");
                        if (null != sliced_productDomArray && sliced_productDomArray.Any())
                        {
                            //设定排序
                            var skuIds2 = sliced_productDomArray.Select(x => { return x.GetAttribute("data-sku"); });
                            foreach (var itemSkuId in skuIds2)
                            {
                                if (!string.IsNullOrEmpty(itemSkuId))
                                {
                                    blockingList_Products.TryAdd(itemSkuId, new ProductOrdered<JingdongProduct> { UniqKey = itemSkuId, IndexOrder = counter_sku });
                                    counter_sku++;
                                }

                            }

                            lstProductDoms.AddRange(sliced_productDomArray);
                        }
                    }

                    #endregion

                    if (lstProductDoms.Any())
                    {
                        //并行解析 并保留原序列
                        lstProductDoms.AsParallel()
                              .ForAll((itemProductDom) =>
                        {

                            //解析一个商品的节点
                            JingdongProduct modelProduct = this.ResolverProductDom(itemProductDom);
                            if (null != modelProduct && modelProduct.ItemId > 0)
                            {
                                modelProduct.Prev_start = pager_prev_start;
                                modelProduct.Next_start = pager_next_start;
                                var orderedObj = blockingList_Products[modelProduct.ItemId.ToString()];
                                orderedObj.Product = modelProduct;
                            }

                        });
                        //进行排序
                        var productsList = blockingList_Products
                            .Where(x => x.Value != null)
                            .OrderBy(x => x.Value.IndexOrder)
                            .Select(x => x.Value.Product);
                        lstProducts.AddRange(productsList);

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
        /// 解析商品节点
        /// </summary>
        /// <param name="modelProduct"></param>
        /// <param name="productDom"></param>
        private JingdongProduct ResolverProductDom(IElement productDom)
        {
            JingdongProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new JingdongProduct();
            try
            {
                //id
                string itemId = productDom.GetAttribute("data-sku");
                if (string.IsNullOrEmpty(itemId))
                {
                    return modelProduct;//凡是没有id 的商品，要么是广告 要么是其他非正常的商品
                }
                long.TryParse(itemId, out long _ItemId);
                modelProduct.ItemId = _ItemId;


                //pid
                modelProduct.Pid = productDom.GetAttribute("data-pid");

                var bottomTxtDom = productDom.QuerySelector("div.p-name");
                //title
                var titleDom = bottomTxtDom.QuerySelector("a");
                modelProduct.Title = titleDom.QuerySelector("em").TextContent.Replace("\n", "");
                modelProduct.ItemUrl = titleDom.GetAttribute("href").GetHttpsUrl();

                //price
                var priceDom = productDom.QuerySelector("div.p-price>strong>i");
                if (null != priceDom)
                {
                    string priceContent = priceDom.TextContent.Replace("\n", "").Trim();
                    decimal.TryParse(priceContent, out decimal _price);
                    modelProduct.Price = _price;
                }
                //pic
                var picDom = productDom.QuerySelector("div.p-img>a>img");
                if (null != picDom)
                {
                    if (picDom.HasAttribute("src"))
                    {
                        modelProduct.PicUrl = picDom.GetAttribute("src").GetHttpsUrl();
                    }
                    else if (picDom.HasAttribute("data-lazy-img"))
                    {
                        modelProduct.PicUrl = picDom.GetAttribute("data-lazy-img").GetHttpsUrl();
                    }

                }

                //shop
                var shopDom = productDom.QuerySelector("div.p-shop>span>a");
                if (null != shopDom)
                {
                    string shopHref = shopDom.GetAttribute("href");
                    modelProduct.ShopUrl = shopHref.GetHttpsUrl();
                    //https://mall.jd.com/index-1000016041.html
                    if (!string.IsNullOrEmpty(shopHref))
                    {
                        int startPos = shopHref.IndexOf('-') + 1;

                        string shopId = shopHref.Substring(startPos).Replace(".html", "");
                        long.TryParse(shopId, out long _shopId);
                        modelProduct.SellerId = _shopId;
                        modelProduct.ShopId = _shopId;
                    }
                    modelProduct.ShopName = shopDom.TextContent.Replace("\n", "");
                }

                //status
                var statusDom = productDom.QuerySelector("div.p-commit");

                if (null != statusDom)
                {
                    ////成交量
                    //var biz30dayDomSpan = statusDom.Children[0];
                    //if (null != biz30dayDomSpan)
                    //{
                    //    string bizTotal = biz30dayDomSpan.Children[0].TextContent;
                    //    if (!string.IsNullOrEmpty(bizTotal))
                    //    {
                    //        modelProduct.Biz30Day = bizTotal.Trim();
                    //    }
                    //}


                    //评论量
                    var remarkDom = statusDom.QuerySelector("strong>a");
                    if (null != remarkDom)
                    {
                        string remarkTotal = remarkDom.TextContent;
                        if (!string.IsNullOrEmpty(remarkTotal))
                        {
                            modelProduct.TotalBizRemarkCount = remarkTotal.Trim();
                        }
                        modelProduct.RemarkUrl = remarkDom.GetAttribute("href").GetHttpsUrl();
                    }

                }


                var iconsDom = productDom.QuerySelectorAll("div.p-icons>i");
                if (null != iconsDom && iconsDom.Length > 0)
                {
                    //是否自营
                    for (int i = 0; i < iconsDom.Length; i++)
                    {
                        var itemIcon = iconsDom[i];
                        if (itemIcon.TextContent.Contains("自营"))
                        {
                            modelProduct.IsSelfSale = true;
                            break;
                        }
                    }
                }

                //sku list
                var skuListDom = productDom.QuerySelector("div.p-scroll");
                if (null != skuListDom)
                {
                    var skuDomArry = skuListDom.QuerySelectorAll("ul.ps-main>li>a");
                    if (skuDomArry != null && skuDomArry.Length > 0)
                    {
                        foreach (var itemSkuDom in skuDomArry)
                        {
                            var littleImgeDom = itemSkuDom.Children[0];
                            var skuItemObj = new SkuItem();
                            skuItemObj.SkuId = littleImgeDom.GetAttribute("data-sku");
                            skuItemObj.SkuName = itemSkuDom.GetAttribute("title");
                            skuItemObj.SkuUrl = string.Format("https://item.jd.com/{0}.html", skuItemObj.SkuId);
                            skuItemObj.SkuImgUrl = littleImgeDom.GetAttribute("data-lazy-img").GetHttpsUrl();

                            modelProduct.SkuList.Add(skuItemObj);
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
