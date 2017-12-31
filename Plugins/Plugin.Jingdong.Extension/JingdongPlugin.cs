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
            if (null != webArgs.OrderFiled&&webArgs.OrderFiled.Rule!= OrderRule.Default)
            {
                sbSearchUrl.Append("&psort=").Append(webArgs.OrderFiled.FieldValue);
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码
            sbSearchUrl.Append("&page=").Append(webArgs.PageIndex + 1);
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
        /// 执行内容解析
        /// </summary>
        ///<param name="webArgs"> </param> 
        /// <param name="content">要解析的内容</param>
        /// <returns>返回需要的字段对应的字典</returns>
        public override Dictionary<string, object> ResolveSearchPageContent(BaseFetchWebPageArgument webArgs, string content)
        {

            var resultBag = new Dictionary<string, object>();

            if (!content.Contains("在京东找到了"))
            {
                return null;//非法请求结果页面
            }
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
                            var li_ADomArray = brandULDom.QuerySelectorAll("li");
                            foreach (var itemADom in li_ADomArray)
                            {
                                var model = new BrandTag();
                                model.Platform = SupportPlatformEnum.Jingdong;
                                model.FilterField = "ev";//使用的过滤字段参数
                                var urlBrand = itemADom.GetAttribute("href");

                                model.BrandId = itemADom.GetAttribute("id");
                                if (!string.IsNullOrEmpty(model.BrandId))
                                {
                                    int stsartPos = model.BrandId.IndexOf('-') + 1;//id=brand-43244
                                    model.BrandId = model.BrandId.Substring(model.BrandId.IndexOf('-') + 1);
                                }

                                model.BrandName = itemADom.Children[0].TextContent.Replace("\n","").Replace("\t","");//<li>元素下的<a>
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

                    var lstTags = new List<KeyWordTag>();
                    var blockList = new BlockingCollection<KeyWordTag>();
                    var taskArray = new List<Task>();
                    if (null != div_AttrsDom_CategoryList)
                    {


                        for (int i = 1; i < div_AttrsDom_CategoryList.Length; i++)
                        {

                            var itemCategory = div_AttrsDom_CategoryList[i];
                            var taskResolveAEmelems = Task.Factory.StartNew(() =>
                            {


                                //找到归属的组
                                string groupName = itemCategory.QuerySelector("div.sl-key").Children[0].TextContent;

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
                                        modelTag.Value = catValueParas["ev"];
                                    }
                                    else if (catValueParas.AllKeys.Contains("cid2"))
                                    {
                                        modelTag.FilterFiled = "cid2";
                                        modelTag.Value = catValueParas["cid2"];
                                    }
                                    else if (catValueParas.AllKeys.Contains("cid3"))
                                    {
                                        modelTag.FilterFiled = "cid3";
                                        modelTag.Value = catValueParas["cid3"];
                                    }

                                    //----解析 a标签完毕-------
                                    blockList.Add(modelTag);

                                }

                            });
                            //将并行任务放到数组
                            taskArray.Add(taskResolveAEmelems);

                        }

                    }

                    //sline 的解析
                    if (null != div_AttrsDom_SlineList)
                    {
                        for (int i = 1; i < div_AttrsDom_SlineList.Length; i++)
                        {

                            var itemSline = div_AttrsDom_SlineList[i];
                            var taskResolveAEmelems = Task.Factory.StartNew(() =>
                            {


                                //找到归属的组
                                string groupName = itemSline.QuerySelector("div.sl-key").Children[0].TextContent;

                                var childLiADomArray = itemSline.QuerySelectorAll("ul.J_valueList>li>a");
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
                                        modelTag.Value = catValueParas["ev"];
                                    }
                                    else if (catValueParas.AllKeys.Contains("cid2"))
                                    {
                                        modelTag.FilterFiled = "cid2";
                                        modelTag.Value = catValueParas["cid2"];
                                    }
                                    else if (catValueParas.AllKeys.Contains("cid3"))
                                    {
                                        modelTag.FilterFiled = "cid3";
                                        modelTag.Value = catValueParas["cid3"];
                                    }


                                    //----解析 a标签完毕-------
                                    blockList.Add(modelTag);

                                }

                            });
                            //将并行任务放到数组
                            taskArray.Add(taskResolveAEmelems);

                        }
                    }

                    //高级选项的解析
                    if (null!=div_AttrsDom_Senior)
                    {
                        var lstAdvDoms = div_AttrsDom_Senior.QuerySelectorAll("div.sl-v-tab>a.trig-item");
                        var lstTabContentItems = div_AttrsDom_Senior.QuerySelectorAll("div.sl-tab-cont-item");
                        if (null!=lstAdvDoms)
                        {
                            int cursor = 0;
                            foreach (var itemAdv in lstAdvDoms)
                            {
                                var taskResolveAEmelems = Task.Factory.StartNew(() =>
                                {


                                    //找到归属的组
                                    string groupName= itemAdv.Children[0].TextContent;
                                    if (null!= lstTabContentItems[cursor])
                                    {
                                        var childLiADomArray = lstTabContentItems[cursor].QuerySelectorAll("ul.J_valueList>li>a");//找到匹配游标的内容组
                                        foreach (var itemADom in childLiADomArray)
                                        {
                                            var modelTag = new KeyWordTag();
                                            modelTag.Platform = SupportPlatformEnum.Jingdong;
                                            modelTag.TagName = itemADom.TextContent;//标签名称
                                            modelTag.GroupShowName = groupName;
                                            string hrefString = itemADom.GetAttribute("href");
                                            var  catValueParas = HttpUtility.ParseQueryString(hrefString);
                                            if (catValueParas.AllKeys.Contains("ev"))
                                            {
                                                modelTag.FilterFiled = "ev";
                                                modelTag.Value = catValueParas["ev"];
                                            }else if (catValueParas.AllKeys.Contains("cid2"))
                                            {
                                                modelTag.FilterFiled = "cid2";
                                                modelTag.Value = catValueParas["cid2"];
                                            }
                                            else if (catValueParas.AllKeys.Contains("cid3"))
                                            {
                                                modelTag.FilterFiled = "cid3";
                                                modelTag.Value = catValueParas["cid3"];
                                            }



                                            //----解析 a标签完毕-------
                                            blockList.Add(modelTag);

                                        }
                                    }
                                    

                                });
                                //将并行任务放到数组
                                taskArray.Add(taskResolveAEmelems);

                                cursor += 1;
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
            //ProductBaseCollection lstProducts = new ProductBaseCollection()
            //{
            //    new JingdongProduct { ItemId=1,Title="测试大衣"}
            //};
            var lstProducts = new ProductBaseCollection();

            //var div_J_ItemListDom = htmlDoc.QuerySelector("div#J_ItemList");
            //if (null != div_J_ItemListDom)
            //{
            //    var div_productDomArray = div_J_ItemListDom.QuerySelectorAll("div.product");
            //    if (null != div_productDomArray && div_productDomArray.Any())
            //    {
            //        //多任务并行解析商品
            //        BlockingCollection<JingdongProduct> blockingList_Products = new BlockingCollection<JingdongProduct>();
            //        var taskArray = new Task[div_productDomArray.Length];
            //        int counter = 0;
            //        foreach (var itemProductDom in div_productDomArray)
            //        {
            //            var tsk = Task.Factory.StartNew(() =>
            //            {
            //                //解析一个商品的节点

            //                JingdongProduct modelProduct = this.ResolverProductDom(itemProductDom);
            //                if (null != modelProduct)
            //                {
            //                    blockingList_Products.Add(modelProduct);
            //                }

            //            });
            //            taskArray[counter] = tsk;
            //            counter += 1;
            //        }
            //        var safeTaskArray = taskArray.Where(x => null != x).ToArray();
            //        Task.WaitAll(safeTaskArray);
            //        var productsList = blockingList_Products.ToArray();
            //        lstProducts.AddRange(productsList);

            //    }
            //}
            resultBag.Add("Products", lstProducts);

            #endregion


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
                string itemId = productDom.GetAttribute("data-id");
                if (string.IsNullOrEmpty(itemId))
                {
                    return modelProduct;//凡是没有id 的商品，要么是广告 要么是其他非正常的商品
                }
                long.TryParse(itemId, out long _ItemId);
                modelProduct.ItemId = _ItemId;

                //title
                var titleDom = productDom.QuerySelector("p.productTitle>a");
                modelProduct.Title = titleDom.TextContent.Replace("\n", "");
                modelProduct.ItemUrl = titleDom.GetAttribute("href");

                //price
                var priceDom = productDom.QuerySelector("p.productPrice>em");
                if (null != priceDom)
                {
                    decimal.TryParse(priceDom.GetAttribute("title"), out decimal _price);
                    modelProduct.Price = _price;
                }
                //pic
                var picDom = productDom.QuerySelector("div.productImg-wrap>a>img");
                if (null != picDom)
                {
                    modelProduct.PicUrl = picDom.GetAttribute("src");
                }

                //shop
                var shopDom = productDom.QuerySelector("div.productShop>a");
                if (null != shopDom)
                {
                    string shopHref = shopDom.GetAttribute("href");
                    if (shopHref.Contains("user_number_id"))
                    {
                        var queryString = shopHref.Substring(shopHref.IndexOf('?'));
                        string shopId = HttpUtility.ParseQueryString(queryString, Encoding.UTF8)["user_number_id"];
                        long.TryParse(shopId, out long _shopId);
                        //modelProduct.ShopId = _shopId;//天猫店铺id 在搜索列表未出现
                        modelProduct.SellerId = _shopId;
                    }


                    modelProduct.ShopName = shopDom.TextContent.Replace("\n", "");
                }

                //status
                var statusDom = productDom.QuerySelector("p.productStatus");
                //成交量
                if (null != statusDom)
                {

                    var biz30dayDomSpan = statusDom.Children[0];
                    if (null != biz30dayDomSpan)
                    {
                        string bizTotal = biz30dayDomSpan.Children[0].TextContent;
                        if (!string.IsNullOrEmpty(bizTotal))
                        {
                            modelProduct.Biz30Day = bizTotal.Trim();
                        }
                    }


                    //评论量
                    var remarkDomSpan = statusDom.Children[1];
                    if (null != remarkDomSpan)
                    {
                        string remarkTotal = remarkDomSpan.Children[0].TextContent;
                        if (!string.IsNullOrEmpty(remarkTotal))
                        {
                            modelProduct.TotalBizRemarkCount = remarkTotal.Trim();
                        }
                        modelProduct.RemarkUrl = remarkDomSpan.Children[0].GetAttribute("href");
                    }

                }
                //sku list
                var skuListDom = productDom.QuerySelector("div.proThumb-wrap");
                if (null != skuListDom)
                {
                    var skuDomArry = skuListDom.QuerySelectorAll("b.proThumb-img");
                    if (skuDomArry != null && skuDomArry.Length > 0)
                    {
                        foreach (var itemSkuDom in skuDomArry)
                        {
                            var skuItemObj = new SkuItem();
                            skuItemObj.SkuId = itemSkuDom.GetAttribute("data-sku");
                            skuItemObj.SkuUrl = string.Concat(modelProduct.ItemUrl, "&sku_properties=", skuItemObj.SkuId);
                            skuItemObj.SkuImgUrl = itemSkuDom.Children[0].GetAttribute("data-ks-lazyload");

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
