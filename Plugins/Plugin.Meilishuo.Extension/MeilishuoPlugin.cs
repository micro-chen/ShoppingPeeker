using System;
using System.Text;
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

namespace Plugin.Meilishuo.Extension
{
    public class MeilishuoPlugin : PluginBase <MeilishuoPlugin>
    {
 
        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new MeilishuoPlugin();
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

            //http://www.meilishuo.com/search/goods?page=1&searchKey=围巾&ppath={"2048":"10112"}&cpc_offset=0

            StringBuilder sbSearchUrl = new StringBuilder("http://www.meilishuo.com/search/goods?searchKey=@###@");


           

            #region  属性 分类 都在参数 ppath 中 标签
            if (null != webArgs.TagGroup)
            {
                //1 当前平台的
                var currentPlatformTag = webArgs.TagGroup.Tags.Where(x => x.Platform == SupportPlatformEnum.Meilishuo);
                if (null != currentPlatformTag)
                {
                    var dicPara = new Dictionary<string, string>();
                    foreach (var item in currentPlatformTag)
                    {
                        dicPara.Add(item.FilterFiled, item.Value);
                    }
                    //将参数序列化为json
                    sbSearchUrl.Append("&ppath=").Append(JsonConvert.SerializeObject(dicPara));
                }
                
                //2 其他平台的tag 作为关键词的一部分
                var otherPlatformTag = webArgs.TagGroup.Tags.FirstOrDefault(x => x.Platform != SupportPlatformEnum.Meilishuo);
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
                sbSearchUrl.Append("&sort=").Append(webArgs.OrderFiled.FieldValue);
            }
            #endregion

            #region  筛选-价格区间
            #endregion

            #region  页码
            sbSearchUrl.Append("&page=").Append(webArgs.PageIndex + 1);
            #endregion
            # region 杂项
            sbSearchUrl.Append("&cpc_offset=0");
          

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
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Meilishuo,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            var lstProducts =new  ProductBaseCollection()
            {
                new MeilishuoProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
