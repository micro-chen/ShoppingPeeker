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

namespace Plugin.Suning.Extension
{
    public class SuningPlugin : PluginBase <SuningPlugin>
    {

 
        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new SuningPlugin();
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


            StringBuilder sbSearchUrl = new StringBuilder("https://search.suning.com/@###@/");



            #region 品牌
         
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Suning);
                if (currentPlatformBrands.Any())
                {
                 

                    //多个品牌直接将id拼接为字符串，国美家的 是4位加密码进行的拼接组
                    string brandNames = string.Join(";", currentPlatformBrands.Select(x => x.BrandName));

                   
                    sbSearchUrl.Append("&hf=brand_Name_FacetAll:").Append(brandNames);
                     
                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Suning);
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
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Suning);
                if (currentPlatformTag.Any())
                {

                    #region 分类
                    var catIdTag = currentPlatformTag.FirstOrDefault(x => x.FilterFiled == "ci");
                    if (null != catIdTag)
                    {
                        sbSearchUrl.Append("&ci=").Append(catIdTag.Value);
                    }
                    #endregion

                    //https://search.suning.com/羽绒服/&iy=0&sc=0&cf=solr_13696_attrId:收腰型;常规&st=0#search-path-box
                    string attrIds = string.Join(";", currentPlatformTag.Select(x => x.Value));
                    sbSearchUrl.Append("&cf=")
                        .Append(currentPlatformTag.First().FilterFiled)
                        .Append("_attrId:")
                        .Append(attrIds);

                }

                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Suning);
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
                sbSearchUrl.Append("&st=0");//默认综合排序
            }
            else
            {
                sbSearchUrl.Append("&st=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码

            sbSearchUrl.Append("&cp=").Append(webArgs.PageIndex + 1);

            #endregion
            # region 杂项
            sbSearchUrl.Append("&iy=0");
            sbSearchUrl.Append("&sc=0");
 
            sbSearchUrl.Append("#second-filter");
            
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
            //1 tags 解析
            var lstTags = new List<KeyWordTag> {
                new KeyWordTag {
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Suning,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            var lstProducts =new  ProductBaseCollection()
            {
                new SuningProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
