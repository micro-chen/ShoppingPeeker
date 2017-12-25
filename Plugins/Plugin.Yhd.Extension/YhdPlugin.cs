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

namespace Plugin.Yhd.Extension
{
    public class YhdPlugin : PluginBase <YhdPlugin>
    {

 
        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new YhdPlugin();
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

            StringBuilder sbSearchUrl = new StringBuilder("http://search.yhd.com/c0-0/");


            #region 品牌
            //string brandString = "mbname";
            if (null != webArgs.Brands && webArgs.Brands.Count > 0)
            {
                //1 当前平台的品牌
                var currentPlatformBrands = webArgs.Brands.Where(x => x.Platform == SupportPlatformEnum.Yhd);
                if (currentPlatformBrands.Any())
                {
                    //http://search.yhd.com/c0-0/mbname金龙鱼,十月稻田-b9429,15840/
                    //多个品牌
                    string brandNames = string.Join(",", currentPlatformBrands.Select(x => x.BrandName));
                    string brandIds = string.Join(",", currentPlatformBrands.Select(x => x.BrandId));
                    sbSearchUrl.Append("mbname").Append(brandNames).Append("-").Append(brandIds).Append("/");
                }
                else
                {
                    sbSearchUrl.Append("mbname-b/");
                }

                //2 非当前平台的品牌--选择其中的一个 作为关键词 分割
                var otherPlatformBrands = webArgs.Brands.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Yhd);
                if (null != otherPlatformBrands)
                {
                    webArgs.KeyWord += " " + otherPlatformBrands.BrandName;
                }
            }
            #endregion

            #region  属性标签   
            string attrString = "a";
            if (null != webArgs.TagGroup)
            {
                //1 当前平台的
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Yhd);
                if (currentPlatformTag.Any())
                {
                    //http://search.yhd.com/c0-0/mbname十月稻田-b15840/a83213||83214::1916_268464519||268472939::268435461
                 
                    var attrIdGroups = currentPlatformTag.GroupBy(x => x.FilterFiled); //string.Join("-", currentPlatformTag.Select(x => x.Value));//&att=1000012:1985-1000012:1986
                    foreach (var gp in attrIdGroups)
                    {
                        string attrIds = string.Join("||", gp.Select(x => x.Value));
                        attrString += string.Concat(attrIds, "::", gp.Key);
                    }
                
                }
                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Yhd);
                if (null != otherPlatformTag)
                {
                    webArgs.KeyWord += " " + otherPlatformTag.TagName;
                }
            }
            sbSearchUrl.Append(attrString);
            
            //http://search.yhd.com/c0-0/mbname十月稻田-b15840/a83213||83214::1916_268464519||268472939::268435461-s1-v4-p1-price-d0-f0-m1-rt0-pid-mid0-color-size-k大米/#page=1&sort=5
            sbSearchUrl.Append("-s1-v4-p2-price-d0-f0-m1-rt0-pid-mid0-color-size-");//p1：不开启； p2：开启 控制后面的分页参数是否启用
            #endregion


            #region 关键词
            sbSearchUrl.AppendFormat("k{0}", webArgs.KeyWord);//将关键词的占位符 进行替换
            #endregion

            #region  页码

            sbSearchUrl.Append("#page=").Append(webArgs.PageIndex + 1);

            #endregion

            #region  排序
            if (null == webArgs.OrderFiled)
            {
                sbSearchUrl.Append("&sort=1");//默认综合排序
            }
            else
            {
                sbSearchUrl.Append("&sort=").Append(webArgs.OrderFiled.FieldValue);//默认综合排序
            }
            #endregion

            #region  筛选-价格区间
            #endregion

        
            # region 杂项
            //sbSearchUrl.Append("&act=input");
            //sbSearchUrl.Append("&show=big");//大图的形式获取，而不是列表 &show=list
            //sbSearchUrl.Append("&show_shop=0#J_tab");
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
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Yhd,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            var lstProducts =new  ProductBaseCollection()
            {
                new YhdProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
