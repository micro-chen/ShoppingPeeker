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

namespace Plugin.Pdd.Extension
{
    public class PddPlugin : PluginBase<PddPlugin>
    {

        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new PddPlugin();
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


            StringBuilder sbSearchUrl = new StringBuilder("http://apiv4.yangkeduo.com/search?q=@###@");

            #region 品牌
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault();
                if (null != otherPlatformBrands)
                {
                    webArgs.KeyWord += " " + otherPlatformBrands.BrandName;
                }
            }
            #endregion

            #region  属性标签
            if (null != webArgs.TagGroup)
            {

                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault();
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
            if (null == webArgs.OrderFiled||webArgs.OrderFiled.Rule== OrderRule.Default)
            {
                sbSearchUrl.Append("&sort=default");//默认综合排序
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
            sbSearchUrl.Append("&size=50");
            #endregion
            # region 杂项
            sbSearchUrl.Append("&requery=0");
            sbSearchUrl.Append("&pdduid=0");

            #endregion
            resultUrl.Url = sbSearchUrl.ToString();
            return resultUrl;
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


                string jsonData = content;

                if (jsonData.Contains("error_code"))
                {
                    return null;//无效的页面json结果数据
                }


                PddPageJsonResut pageJsonObj = JsonConvert.DeserializeObject<PddPageJsonResut>(jsonData);
                if (null == pageJsonObj)
                {
                    return null;
                }


                #region products  解析  
                var lstProducts = new ProductBaseCollection();
                resultBag.Add("Products", lstProducts);

                var itemListNode = pageJsonObj.items;
                if (null != itemListNode && itemListNode.Any())
                {

                    foreach (var itemProduct in itemListNode)
                    {
                        PddProduct modelProduct = this.ResolverProductDom(itemProduct);

                        if (null != modelProduct)
                        {
                            lstProducts.Add(modelProduct);
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
        /// <summary>
        /// 解析商品节点
        /// </summary>
        /// <param name="modelProduct"></param>
        /// <param name="productDom"></param>
        private PddProduct ResolverProductDom(PddPageJsonResut.Items productDom)
        {
            PddProduct modelProduct = null;
            if (null == productDom)
            {
                return modelProduct;
            }
            modelProduct = new PddProduct();
            try
            {
                //id
                modelProduct.ItemId = productDom.goods_id;

                //title
                modelProduct.Title = productDom.goods_name;
                modelProduct.ItemUrl = string.Format("http://mobile.yangkeduo.com/goods.html?goods_id={0}&page_el_sn=99369&is_spike=0&refer_page_name=search_result&refer_page_id=search_result_1515548838338_XlWum3MGYr&refer_page_sn=10015&refer_page_el_sn=99369", productDom.goods_id);

                //price
                modelProduct.Price = productDom.price / 100;

                //pic
                modelProduct.PicUrl = productDom.hd_thumb_url;
                //shop
                //string shopId = productDom.user_id;
                //long.TryParse(shopId, out long _shopId);
                ////modelProduct.ShopId = _shopId;
                //modelProduct.SellerId = _shopId;
                //modelProduct.ShopUrl = string.Format("https://store.Pdd.com/shop/view_shop.htm?user_number_id={0}", shopId);
                //modelProduct.ShopName = productDom.nick;


                //status
                //成交量
                if (productDom.sales>10000)
                {
                    double littlePrice = productDom.sales / 10000;
                    modelProduct.Biz30Day = littlePrice.ToString("#0.0万");
                }
                else
                {
                    modelProduct.Biz30Day = productDom.sales.ToString();
                }
             

                //评论量
                //modelProduct.TotalBizRemarkCount = productDom.comment_count;
                //modelProduct.RemarkUrl = productDom.comment_url;

                //卖家地址
                //modelProduct.SellerAddress = productDom.item_loc;

            }
            catch (Exception ex)
            {
                PluginContext.Logger.Error(ex);
            }
            return modelProduct;
        }


    }
}
