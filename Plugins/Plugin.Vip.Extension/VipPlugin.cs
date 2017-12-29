using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

using NTCPMessage.EntityPackage.Products;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;

using ShoppingPeeker.Plugins;

namespace Plugin.Vip.Extension
{
    public class VipPlugin : PluginBase <VipPlugin>
    {


 
        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new VipPlugin();
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
            resultUrl.ParasPost=new Dictionary<string, object> ();
            var urlParaContainer = resultUrl.ParasPost;

            //1 查询品牌
            var brandParaModel = new VipSearchParaBrand(webArgs.KeyWord);
            urlParaContainer.Add("para_brand",JsonConvert.SerializeObject(brandParaModel) );
            // 2 查询分类
            var categoryTreeParaModel = new VipSearchParaCategoryTree(webArgs.KeyWord);
            urlParaContainer.Add("para_categoryTree", JsonConvert.SerializeObject(categoryTreeParaModel));

            //3检索内容
            var searchListParaModel = new VipSearchParaSearchList(webArgs.KeyWord);
            //分页
            searchListParaModel.paramsDetails.np = webArgs.PageIndex + 1;
            //排序
            int tempSort = 0;
            int.TryParse(webArgs.OrderFiled.FieldValue, out tempSort);
            searchListParaModel.paramsDetails.sort = tempSort;
            //品牌
            if (null != webArgs.Brands && webArgs.Brands.Any())
            {
                searchListParaModel.paramsDetails.brand_store_sn = string.Join(",", webArgs.Brands.Select(x => x.BrandId));
            }
            //分类+规格
            if (null != webArgs.TagGroup)
            {
                //分类
                var category_id_1_5_show = webArgs.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_1_5_showTags");
                searchListParaModel.paramsDetails.category_id_1_5_show = string.Join(",", category_id_1_5_show.Select(x => x.Value));
                var category_id_1_show = webArgs.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_1_showTags");
                searchListParaModel.paramsDetails.category_id_1_show = string.Join(",", category_id_1_show.Select(x => x.Value));
                var category_id_2_show = webArgs.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_2_showTags");
                searchListParaModel.paramsDetails.category_id_2_show = string.Join(",", category_id_2_show.Select(x => x.Value));
                var category_id_3_show = webArgs.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_3_showTags");
                searchListParaModel.paramsDetails.category_id_3_show = string.Join(",", category_id_3_show.Select(x => x.Value));
                //规格
                var props = webArgs.TagGroup.Tags.Where(x => x.FilterFiled == "props");
                searchListParaModel.paramsDetails.props = string.Join(";", props.Select(x => x.Value));
            }

            urlParaContainer.Add("para_searchList", JsonConvert.SerializeObject(searchListParaModel));

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
            //1 tags 解析
            var lstTags = new List<KeyWordTag> {
                new KeyWordTag {
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Vip,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            var lstProducts =new  ProductBaseCollection()
            {
                new VipProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
