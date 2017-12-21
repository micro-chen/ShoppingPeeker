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

namespace Plugin.Mogujie.Extension
{
    public class MogujiePlugin : PluginBase <MogujiePlugin>
    {
        
        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override IPlugin CreateNew()
        {
            var instance = new MogujiePlugin();
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
            var timeToken = JavascriptContext.getUnixTimestamp();
            //http://list.mogujie.com/search?callback=jQuery211013398370030336082_{0}&_version=8193&q=%E5%8F%A3%E7%BA%A2&cKey=43&minPrice=&_mgjuuid=66b111f4-e6ce-4b8b-bf0c-311fa8cf0c31&ppath=&page=1&maxPrice=&sort=pop&userId=&cpc_offset=&ratio=2%3A3&_=1500446274789

            StringBuilder sbSearchUrl = new StringBuilder(string.Format("http://list.mogujie.com/search?callback=jQuery211013398370030336082_{0}&_version=8193&q=@###@&cKey=43", timeToken));

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
            sbSearchUrl.Append("&minPrice=");
            sbSearchUrl.Append("&maxPrice=");
            #endregion

            #region  页码
            sbSearchUrl.Append("&page=").Append(webArgs.PageIndex + 1);
            #endregion

            # region 杂项
            sbSearchUrl.Append("&cKey=43");
            sbSearchUrl.Append("&_mgjuui=c87fe209-480b-4031-b92e-feb3714ae5ba");
            sbSearchUrl.Append("&userId=");
            sbSearchUrl.Append("&cpc_offset=");
            sbSearchUrl.Append("&_=").Append(timeToken);
            

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
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Mogujie,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            ProductBaseCollection lstProducts =new  ProductBaseCollection()
            {
                new MogujieProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
