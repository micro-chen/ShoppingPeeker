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


namespace Plugin.Tmall.Extension
{
    public class TmallPlugin : PluginBase<TmallPlugin>
    {



        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new TmallPlugin();
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


            StringBuilder sbSearchUrl = new StringBuilder("https://list.tmall.com/search_product.htm?spm=a220m.1000858.1000720.1.348abe64rj5JVg");


            #region 品牌
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Tmall);
                if (currentPlatformBrands.Any())
                {
                    //多个品牌用 , 号分割
                    string brandIds = string.Join(",", currentPlatformBrands.Select(x => x.BrandId));
                    sbSearchUrl.Append("&brand=").Append(brandIds);

                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Tmall);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform == SupportPlatformEnum.Tmall);
                if (null != currentPlatformTag)
                {
                    sbSearchUrl.Append("&prop=").Append(currentPlatformTag.Value);
                }
                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Tmall);
                if (null != otherPlatformTag)
                {
                    webArgs.KeyWord += " " + otherPlatformTag.TagName;
                }
            }
            #endregion

            #region 关键词
            sbSearchUrl.Append("&q=").Append(webArgs.KeyWord);
            #endregion

            #region  排序
            if (null == webArgs.OrderFiled)
            {
                sbSearchUrl.Append("&sort=s");//默认综合排序
            }
            else
            {
                sbSearchUrl.Append("&sort=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码

            var pageNumber = webArgs.PageIndex;
            if (pageNumber > 0)
            {
                sbSearchUrl.Append("&s=").Append(pageNumber * 60);//天猫的分页是基于页索引*60
                sbSearchUrl.Append("&search_condition=2");

            }
            #endregion
            # region 杂项
            sbSearchUrl.Append("&from=mallfp..pc_1_searchbutton");
            sbSearchUrl.Append("&type=pc");
            sbSearchUrl.Append("&style=g");
            #endregion
            resultUrl.Url = sbSearchUrl.ToString();

            return resultUrl;
        }
        /// <summary>
        /// 执行内容解析
        /// </summary>
        ///<param name="isNeedHeadFilter">是否要解析头部筛选</param> 
        /// <param name="content">要解析的内容</param>
        /// <returns>返回需要的字段对应的字典</returns>
        public override Dictionary<string, object> ResolveSearchPageContent(bool isNeedHeadFilter, string content)
        {

            var resultBag = new Dictionary<string, object>();

            //创建html 文档对象
            HtmlParser htmlParser = new HtmlParser();
            var htmlDoc = htmlParser.Parse(content);

            if (isNeedHeadFilter == true)
            {


                var div_AttrsDom = htmlDoc.QuerySelector("div.j_NavAttrs");
                var ulDomArray = div_AttrsDom.QuerySelectorAll("div.attrValues>ul");
                #region 品牌解析
                var lstBrands = new List<BrandTag>();
                if (null != div_AttrsDom)
                {
                    //从属性区域解析dom-品牌内容
                    var brandULDom = ulDomArray[0];// div_AttrsDom.QuerySelector("div.j_Brand>div.attrValues>ul");

                    if (null != brandULDom)
                    {
                        var regex_MatchBrandId = new Regex(@"brand=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        var li_ADomArray = brandULDom.QuerySelectorAll("li>a");
                        foreach (var itemADom in li_ADomArray)
                        {
                            var model = new BrandTag();
                            model.Platform = SupportPlatformEnum.Tmall;
                            var urlBrand = itemADom.GetAttribute("href");
                            if (!string.IsNullOrEmpty(urlBrand) && urlBrand.Contains("brand="))
                            {
                                model.BrandId = regex_MatchBrandId.Match(urlBrand).Groups[1].Value;//new//品牌id   href="?brand=110910&amp;q=%B4%F3%C3%D7&amp;sort=s&amp;style=g&amp;from=sn_1_brand-qp&amp;spm=a220m.1000858.1000720.1.348abe64rj5JVg#J_crumbs
                            }
                            model.BrandName = itemADom.GetAttribute("title");
                            lstBrands.Add(model);
                        }
                    }
                }
                resultBag.Add("Brands", lstBrands);

                #endregion

                // tags 解析
                //var lstTags = new List<KeyWordTag> {
                //new KeyWordTag {
                //    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Tmall,
                //    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                //} };

                var lstTags = new List<KeyWordTag>();
                if (null != div_AttrsDom)
                {
                    var blockList = new BlockingCollection<KeyWordTag>();

                    //分类 or 属性;品牌是第一个，其他属性是后续
                    var taskArray = new Task[ulDomArray.Length-1];
                    int counter = 0;
                    for (int i = 1; i < ulDomArray.Length; i++)
                    {
                        int cursor = i;

                        var taskResolveAEmelems = Task.Factory.StartNew(() =>
                        {
                            var itemUl = ulDomArray[cursor];

                            //找到归属的组
                            var attrKeyDom = itemUl.ParentElement.ParentElement.QuerySelector("div.attrKey");
                            string groupName = "";
                            if (null != attrKeyDom)
                            {
                                groupName = attrKeyDom.TextContent.Replace("\n", "").Trim();
                            }



                            var childLiADomArray = itemUl.QuerySelectorAll("li>a");
                            foreach (var itemADom in childLiADomArray)
                            {
                                var modelTag = new KeyWordTag();
                                modelTag.Platform = SupportPlatformEnum.Tmall;
                                modelTag.TagName = itemADom.TextContent.Replace("\n", "");//标签名称
                                modelTag.GroupShowName = groupName;

                                //////----解析 a标签开始-------
                                //////检查 a 的href 中的参数；cat 或者prop
                                string hrefValue = itemADom.GetAttribute("href");
                                if (!string.IsNullOrEmpty(hrefValue))
                                {
                                    var urlParas = HttpUtility.ParseQueryString(hrefValue, Encoding.UTF8);
                                    if (null != urlParas)
                                    {
                                        if (hrefValue.IndexOf("cat=") > -1)
                                        {
                                            //1 cat
                                            string catValue = urlParas["cat"];
                                            modelTag.FilterFiled = "cat";
                                            modelTag.Value = catValue;
                                        }
                                        else if (hrefValue.IndexOf("prop=") > -1)
                                        {
                                            //2 prop
                                            string propValue = urlParas["prop"];
                                            modelTag.FilterFiled = "prop";
                                            modelTag.Value = propValue;
                                        }
                                    }
                                }
                                //----解析 a标签完毕-------
                                blockList.Add(modelTag);

                            }

                        });
                        //将并行任务放到数组
                        taskArray[counter] = taskResolveAEmelems;
                        counter += 1;
                    }
                    var safeTaskArray = taskArray.Where(x => null != x).ToArray();
                    Task.WaitAll(safeTaskArray);
                    lstTags = blockList.ToList();
                }
                resultBag.Add("Tags", lstTags);

            }

            #region products  解析
            //ProductBaseCollection lstProducts = new ProductBaseCollection()
            //{
            //    new TmallProduct { ItemId=1,Title="测试大衣"}
            //};
            var lstProducts = new ProductBaseCollection();

            var div_J_ItemListDom = htmlDoc.QuerySelector("div#J_ItemList");
            if (null != div_J_ItemListDom)
            {
                var div_productDomArray = div_J_ItemListDom.QuerySelectorAll("div.product");
                if (null != div_productDomArray && div_productDomArray.Any())
                {
                    //多任务并行解析商品
                    BlockingCollection<TmallProduct> blockingList_Products = new BlockingCollection<TmallProduct>();
                    var taskArray = new Task[div_productDomArray.Length];
                    int counter = 0;
                    foreach (var itemProductDom in div_productDomArray)
                    {
                        var tsk = Task.Factory.StartNew(() =>
                        {
                            //解析一个商品的节点

                            TmallProduct modelProduct = this.ResolverProductDom(itemProductDom);
                            if (null != modelProduct)
                            {
                                blockingList_Products.Add(modelProduct);
                            }

                        });
                        taskArray[counter] = tsk;
                        counter += 1;
                    }
                    var safeTaskArray= taskArray.Where(x => null != x).ToArray();
                    Task.WaitAll(safeTaskArray);
                    var productsList = blockingList_Products.ToArray();
                    lstProducts.AddRange(productsList);

                }
            }
            resultBag.Add("Products", lstProducts);

            #endregion


            return resultBag;// string.Concat("has process input :" + content);
        }

        /// <summary>
        /// 解析商品节点
        /// </summary>
        /// <param name="modelProduct"></param>
        /// <param name="productDom"></param>
        private TmallProduct ResolverProductDom(IElement productDom)
        {
            TmallProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new TmallProduct();
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
                PluginContext.OutPutError(ex);
            }
            return modelProduct;
        }

    }
}
