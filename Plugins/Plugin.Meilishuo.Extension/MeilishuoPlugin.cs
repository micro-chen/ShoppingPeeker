using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
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
        /// 执行内容解析
        /// </summary>
        /// <param name="content">要解析的内容</param>
        /// <returns>返回需要的字段对应的字典</returns>
        public override object Execute(string content)
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
            ProductBaseCollection lstProducts =new  ProductBaseCollection()
            {
                new MeilishuoProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
