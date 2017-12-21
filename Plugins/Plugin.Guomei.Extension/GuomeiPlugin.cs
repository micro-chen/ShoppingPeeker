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

namespace Plugin.Guomei.Extension
{
    public class GuomeiPlugin : PluginBase<GuomeiPlugin>
    {


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
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Guomei,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            ProductBaseCollection lstProducts = new ProductBaseCollection()
            {
                new GuomeiProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
