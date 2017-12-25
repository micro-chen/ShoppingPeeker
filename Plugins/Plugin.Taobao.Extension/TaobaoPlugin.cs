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

namespace Plugin.Taobao.Extension
{
    public class TaobaoPlugin : PluginBase <TaobaoPlugin>
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
                var dir= Assembly.GetExecutingAssembly().GetDirectoryPath();
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
            resultUrl.Url= sbSearchUrl.ToString();
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
            //1 tags 解析
            var lstTags = new List<KeyWordTag> {
                new KeyWordTag {
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Taobao,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            var lstProducts =new  ProductBaseCollection()
            {
                new TaobaoProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
