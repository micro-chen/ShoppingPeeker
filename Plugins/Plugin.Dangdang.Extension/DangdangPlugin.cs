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
using ShoppingPeeker.Plugins.NPinYin;

namespace Plugin.Dangdang.Extension
{
    public class DangdangPlugin : PluginBase<DangdangPlugin>
    {



        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new DangdangPlugin();
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

            StringBuilder sbSearchUrl = new StringBuilder("http://search.dangdang.com/?key=@###@");


            #region 品牌
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Dangdang);
                if (currentPlatformBrands.Any())
                {
                    //多个品牌用 _ 号分割
                    string brandIds = string.Join("_", currentPlatformBrands.Select(x => x.BrandId));
                    sbSearchUrl.Append("&att=1:").Append(brandIds);

                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Dangdang);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Dangdang);
                if (currentPlatformTag.Any())
                {
                    //1 分类 cat
                    var catFilter = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "category_id");
                    if (null != catFilter)
                    {
                        sbSearchUrl.Append("&category_id=").Append(catFilter.Value);
                    }
                    //2 属性 att
                    var attFilter = currentPlatformTag.Where(x => x.FilterFiled == "att");
                    if (attFilter.Any())
                    {
                        string attrIds = string.Join("-", currentPlatformTag.Select(x => x.Value));//&att=1000012:1985-1000012:1986
                        sbSearchUrl.Append("&att=").Append(attrIds);
                    }

                    //3 其他分类路径
                    var catePathFilter = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "category_path");
                    if (null != catePathFilter)
                    {
                        sbSearchUrl.Append("&category_path=").Append(catePathFilter.Value);
                    }

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
            sbSearchUrl.Replace("@###@", webArgs.KeyWord);//将关键词的占位符 进行替换
            #endregion

            #region  排序
            if (null == webArgs.OrderFiled)
            {
                sbSearchUrl.Append("&sort_type=sort_default");//默认综合排序
            }
            else
            {
                sbSearchUrl.Append("&sort_type=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码

            sbSearchUrl.Append("&page_index=").Append(webArgs.PageIndex + 1);

            #endregion
            # region 杂项
            sbSearchUrl.Append("&act=input");
            sbSearchUrl.Append("&show=big");//大图的形式获取，而不是列表 &show=list
            sbSearchUrl.Append("&show_shop=0#J_tab");
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

            if (content.Contains("没有找到"))
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

                    var div_filterDoms = htmlDoc.QuerySelector("div#navigation");

                    if (null != div_filterDoms)
                    {


                        //var div_BrandsDom = div_filterDoms.QuerySelector("div.s-brand");
                        var brandULDom = div_filterDoms.QuerySelector("ul.img_list_content_ul");//图片的品牌格式
                        bool isTextBrand = false;
                        if (null == brandULDom)
                        {
                            //文字的品牌格式
                            brandULDom = div_filterDoms.QuerySelectorAll("li.child_li").FirstOrDefault();
                            if (null != brandULDom)
                            {
                                var isBrand = brandULDom.QuerySelector("div.list_left").TextContent.Trim().Contains("品牌");
                                if (!isBrand)
                                {
                                    brandULDom = null;
                                }
                                else
                                {
                                    isTextBrand = true;
                                }
                            }
                        }
                        #region 品牌解析
                        var lstBrands = new List<BrandTag>();
                        if (null != brandULDom)
                        {
                            //从属性区域解析dom-品牌内容


                            if (isTextBrand == false)
                            {
                                var li_DomArray = brandULDom.QuerySelectorAll("li");
                                foreach (var itemLiDom in li_DomArray)
                                {
                                    var itemADom = itemLiDom.FirstElementChild;//<li>元素下的<a>

                                    var model = new BrandTag();
                                    model.Platform = SupportPlatformEnum.Dangdang;
                                    model.FilterField = "att";//使用的过滤字段参数

                                    model.BrandName = itemADom.GetAttribute("title");

                                    var urlBrand = HttpUtility.UrlDecode(itemADom.GetAttribute("href"), Encoding.GetEncoding("gb2312"));
                                    model.BrandId = HttpUtility.ParseQueryString(urlBrand)["att"];
                                    if (!string.IsNullOrEmpty(model.BrandId))
                                    {
                                        model.BrandId = model.BrandId.Replace("#J_tab", "");
                                    }
                                    model.CharIndex = PinYin.GetFirstLetter(model.BrandName);//定位字符

                                    //var imgDom = itemADom.QuerySelector("img");
                                    //if (null != imgDom)
                                    //{
                                    //    model.IconUrl = imgDom.GetAttribute("src");
                                    //}


                                    lstBrands.Add(model);

                                }
                            }
                            else
                            {
                                //解析文本品牌节点
                                var brandRoot = brandULDom.QuerySelector("div.clearfix");
                                var childLiADomArray = brandRoot.QuerySelectorAll("span>a");
                                foreach (var itemADom in childLiADomArray)
                                {
                                    var model = new BrandTag();
                                    model.Platform = SupportPlatformEnum.Dangdang;
                                    model.FilterField = "att";//使用的过滤字段参数

                                    model.BrandName = itemADom.TextContent;//标签名称

                                    var urlBrand = HttpUtility.UrlDecode(itemADom.GetAttribute("href"), Encoding.GetEncoding("gb2312"));
                                    model.BrandId = HttpUtility.ParseQueryString(urlBrand)["att"];
                                    if (!string.IsNullOrEmpty(model.BrandId))
                                    {
                                        model.BrandId = model.BrandId.Replace("#J_tab", "");
                                    }
                                    model.CharIndex = PinYin.GetFirstLetter(model.BrandName);//定位字符

                                    //////var imgDom = itemADom.QuerySelector("img");
                                    //////if (null != imgDom)
                                    //////{
                                    //////    model.IconUrl = imgDom.GetAttribute("src");
                                    //////}


                                    lstBrands.Add(model);

                                }
                              
                            }

                        }
                        resultBag.Add("Brands", lstBrands);

                        #endregion

                        // tags 解析
                        //var lstTags = new List<KeyWordTag> {
                        //new KeyWordTag {
                        //    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Dangdang,
                        //    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                        //} };

                        //category 解析  
                        var div_AttrsDom_Category = div_filterDoms.QuerySelector("li[name='navigation-category']");

                        //普通tag--排除品牌 和类别的其他的标签
                        var div_AttrsDom_Senior = div_filterDoms.QuerySelectorAll("li.child_li").Where(x =>
                        {
                            if (x.ClassList.Contains("brand_pic") || x.ClassList.Contains("first_sort"))
                            {
                                return false;
                            }

                            return true;

                        });

                        var lstTags = new List<KeyWordTagGroup>();
                        var blockList = new BlockingCollection<KeyWordTagGroup>();
                        var taskArray = new List<Task>();
                        if (null != div_AttrsDom_Category)
                        {

                            // PLINQ 的操作 
                            //div_AttrsDom_CategoryList.AsParallel().ForAll((x) => { })

                            var taskResolveAEmelems = Task.Factory.StartNew(() =>
                            {

                                var itemCategory = div_AttrsDom_Category;

                                //找到归属的组
                                string groupName = "分类";
                                var tagGroup = new KeyWordTagGroup(groupName);
                                var childLiADomArray = itemCategory.QuerySelectorAll("div.clearfix>span>a");
                                foreach (var itemADom in childLiADomArray)
                                {
                                    var modelTag = new KeyWordTag();
                                    modelTag.Platform = SupportPlatformEnum.Dangdang;
                                    modelTag.TagName = itemADom.GetAttribute("title");//标签名称
                                    modelTag.GroupShowName = groupName;
                                    string hrefString = itemADom.GetAttribute("href");
                                    var catValueParas = HttpUtility.ParseQueryString(hrefString);
                                    if (catValueParas.AllKeys.Contains("category_id"))
                                    {
                                        modelTag.FilterFiled = "category_id";
                                        modelTag.Value = catValueParas["category_id"].Replace("#J_tab", "");
                                    }
                                    else if (catValueParas.AllKeys.Contains("category_path"))
                                    {
                                        modelTag.FilterFiled = "category_path";
                                        modelTag.Value = catValueParas["category_path"].Replace("#J_tab", "");
                                    }

                                    tagGroup.Tags.Add(modelTag);

                                }
                                //----解析 a标签完毕-------
                                blockList.Add(tagGroup);

                            }, TaskCreationOptions.None);
                            //将并行任务放到数组
                            taskArray.Add(taskResolveAEmelems);



                        }



                        //高级选项的解析
                        if (null != div_AttrsDom_Senior)
                        {
                            foreach (var itemAttrGroup in div_AttrsDom_Senior)
                            {


                                var taskResolveAEmelems = Task.Factory.StartNew((paraItem) =>
                                {

                                    var itemCategory = paraItem as IElement;

                                    //找到归属的组
                                    string groupName = itemCategory.QuerySelector("div.list_left").TextContent;
                                    if (groupName.Contains("品牌"))
                                    {
                                        return;//不解析文字格式的品牌tags
                                    }
                                    var tagGroup = new KeyWordTagGroup(groupName);
                                    var childLiADomArray = itemCategory.QuerySelectorAll("div.clearfix>span>a");
                                    foreach (var itemADom in childLiADomArray)
                                    {
                                        var modelTag = new KeyWordTag();
                                        modelTag.Platform = SupportPlatformEnum.Dangdang;
                                        modelTag.TagName = itemADom.TextContent;//标签名称
                                        modelTag.GroupShowName = groupName;

                                        var urlBrand = HttpUtility.UrlDecode(itemADom.GetAttribute("href"), Encoding.GetEncoding("gb2312"));
                                        var catValueParas = HttpUtility.ParseQueryString(urlBrand);
                                        if (catValueParas.AllKeys.Contains("att"))
                                        {
                                            modelTag.FilterFiled = "att";
                                            modelTag.Value = catValueParas["att"].Replace("#J_tab", "");
                                        }

                                        tagGroup.Tags.Add(modelTag);

                                    }
                                    //----解析 a标签完毕-------
                                    blockList.Add(tagGroup);

                                }, itemAttrGroup, TaskCreationOptions.None);
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

                #region products  解析

                var lstProducts = new ProductBaseCollection();
                //多任务并行解析商品
                // ConcurrentDictionary<string, ProductOrdered<DangdangProduct>> blockingList_Products = new ConcurrentDictionary<string, ProductOrdered<DangdangProduct>>();

                //普通商品区域--不含广告的商品区域
                var div_ItemListDom = htmlDoc.QuerySelector("div#search_nature_rg");
                if (null != div_ItemListDom)
                {
                    var lstProductDoms = div_ItemListDom.QuerySelectorAll("ul.cloth_shoplist>li");

                    if (lstProductDoms.Any())
                    {

                        foreach (var itemProductDom in lstProductDoms)
                        {
                            //解析一个商品的节点
                            DangdangProduct modelProduct = this.ResolverProductDom(itemProductDom);
                            if (null != modelProduct && modelProduct.ItemId > 0)
                            {
                                lstProducts.Add(modelProduct);
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
        /// 解析商品节点
        /// LI 元素
        /// </summary>
        /// <param name="productDom"></param>
        private DangdangProduct ResolverProductDom(IElement productDom)
        {
            DangdangProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new DangdangProduct();
            try
            {
                //id
                string itemId = productDom.GetAttribute("id");
                if (string.IsNullOrEmpty(itemId))
                {
                    return modelProduct;//凡是没有id 的商品，要么是广告 要么是其他非正常的商品
                }
                long.TryParse(itemId, out long _ItemId);
                modelProduct.ItemId = _ItemId;

                var bottomTxtDom = productDom.QuerySelector("p.name");
                //title
                var titleDom = bottomTxtDom.QuerySelector("a");
                modelProduct.Title = titleDom.GetAttribute("title");
                modelProduct.ItemUrl = titleDom.GetAttribute("href");
                var hotDom = productDom.QuerySelector("p.search_hot_word");
                if (null != hotDom)
                {
                    modelProduct.Title += hotDom.TextContent;
                }
                //pic
                var picDom = productDom.QuerySelector("a.pic>img");
                if (null != picDom)
                {
                    modelProduct.PicUrl = picDom.GetAttribute("src");
                }

                //price
                var priceDom = productDom.QuerySelector("span.price_n");
                if (null != priceDom)
                {
                    string priceContent = priceDom.TextContent.Replace("¥", "").Trim();
                    decimal.TryParse(priceContent, out decimal _price);
                    modelProduct.Price = _price;
                }


                //shop
                var shopDom = productDom.QuerySelector("p.link>a");
                if (null != shopDom)
                {
                    string shopHref = shopDom.GetAttribute("href");
                    modelProduct.ShopUrl = shopHref;

                    if (!string.IsNullOrEmpty(shopHref))
                    {
                        int startPos = shopHref.LastIndexOf('/') + 1;

                        string shopId = shopHref.Substring(startPos);
                        long.TryParse(shopId, out long _shopId);
                        modelProduct.SellerId = _shopId;
                        modelProduct.ShopId = _shopId;
                    }
                    modelProduct.ShopName = shopDom.TextContent;
                }

                //status
                var statusDom = productDom.QuerySelector("p.star");

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
                    var remarkDom = statusDom.QuerySelector("a");
                    if (null != remarkDom)
                    {
                        string remarkTotal = remarkDom.TextContent;
                        if (!string.IsNullOrEmpty(remarkTotal))
                        {
                            modelProduct.TotalBizRemarkCount = remarkTotal.Trim();
                        }
                        modelProduct.RemarkUrl = remarkDom.GetAttribute("href");
                    }

                }


                var iconsDom = productDom.QuerySelectorAll("span.new_lable");
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
                var skuListDom = productDom.QuerySelector("div.pic_list_lunbo");
                if (null != skuListDom)
                {
                    var skuDomArry = skuListDom.QuerySelectorAll("ul>li");
                    if (skuDomArry != null && skuDomArry.Length > 0)
                    {
                        foreach (var itemSkuDom in skuDomArry)
                        {
                            var skuItemObj = new SkuItem();

                            var littleImgeADom = itemSkuDom.Children[0];
                            var iconSkuDom = littleImgeADom.FirstElementChild;//img
                            if (null != iconSkuDom)
                            {
                                //http://img3m0.ddimg.cn/53/17/1438240670-1_x.jpg
                                skuItemObj.SkuImgUrl = iconSkuDom.GetAttribute("data-original");

                                string skuIdStr = skuItemObj.SkuImgUrl.Substring(skuItemObj.SkuImgUrl.LastIndexOf('/')); //itemSkuDom.GetAttribute("sku");
                                skuItemObj.SkuId = skuIdStr.Substring(0, skuIdStr.IndexOf('-'));
                                skuItemObj.SkuName = littleImgeADom.GetAttribute("title");
                                skuItemObj.SkuUrl = string.Format("http://product.dangdang.com/{0}.html", skuItemObj.SkuId);


                                modelProduct.SkuList.Add(skuItemObj);
                            }



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
