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

            # region 杂项
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

            string jsonData = string.Empty;

            if (content.IndexOf("g_page_config") < 0)
            {
                return null;//无效的页面结果数据
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

            if (isNeedHeadFilter == true)
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
            var itemListNode = pageJsonObj.mods.itemlist;
            if (null != itemListNode && itemListNode.data != null && null != itemListNode.data.auctions)
            {
                var lstProducts = new ProductBaseCollection();
                foreach (var itemProduct in itemListNode.data.auctions)
                {
                    TaobaoProduct modelProduct = this.ResolverProductDom(itemProduct);

                    if (null != modelProduct)
                    {
                        lstProducts.Add(modelProduct);
                    }

                }

                resultBag.Add("Products", lstProducts);
            }


            #endregion


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
                modelProduct.ItemUrl = productDom.detail_url;

                //price
                var priceDom = productDom.view_price;
                if (null != priceDom)
                {
                    decimal.TryParse(priceDom, out decimal _price);
                    modelProduct.Price = _price;
                }
                //pic
                modelProduct.PicUrl = productDom.pic_url;
                //shop
                string shopId = productDom.user_id;
                long.TryParse(shopId, out long _shopId);
                //modelProduct.ShopId = _shopId;//天猫店铺id 在搜索列表未出现
                modelProduct.SellerId = _shopId;

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
                PluginContext.OutPutError(ex);
            }
            return modelProduct;
        }



        /// <summary>
        /// 解析商品节点
        /// </summary>
        /// <param name="modelProduct"></param>
        /// <param name="productDom"></param>
        private TaobaoProduct ResolverProductDom(IElement productDom)
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
