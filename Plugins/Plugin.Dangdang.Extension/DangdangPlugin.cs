using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using System.Linq;

using NTCPMessage.EntityPackage.Products;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;

using ShoppingPeeker.Plugins;

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
        public override string ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
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
                    string attrIds = string.Join("-", currentPlatformTag.Select(x => x.Value));//&att=1000012:1985-1000012:1986
                    sbSearchUrl.Append("&att=").Append(attrIds);
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
            //品牌解析
            var lstBrands = new List<BrandTag>();

            resultBag.Add("BrandTag", lstBrands);

            // tags 解析
            var lstTags = new List<KeyWordTag> {
                new KeyWordTag {
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Tmall,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            //  products  解析
            ProductBaseCollection lstProducts = new ProductBaseCollection()
            {
                new TmallProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }
    }
}
