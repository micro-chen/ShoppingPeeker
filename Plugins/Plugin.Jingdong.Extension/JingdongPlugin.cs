using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
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
        public override string ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
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
            if (null != webArgs.OrderFiled)
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
            return sbSearchUrl.ToString();
        }


        /// <summary>
        /// 执行内容解析
        /// </summary>
        /// <param name="content">要解析的内容</param>
        /// <returns>返回需要的字段对应的字典</returns>
        public override Dictionary<string, object> ResolveSearchPageContent(string content)
        {

            var resultBag = new Dictionary<string, object>();
            //1 tags 解析
            var lstTags = new List<KeyWordTag> {
                new KeyWordTag {
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Jingdong,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            ProductBaseCollection lstProducts = new ProductBaseCollection()
            {
                new JingdongProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
