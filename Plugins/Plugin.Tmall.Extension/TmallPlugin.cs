using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using NTCPMessage.EntityPackage.Products;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;

using ShoppingPeeker.Plugins;

namespace Plugin.Tmall.Extension
{
    public class TmallPlugin : PluginBase
    {

        /// <summary>
        /// 创建插件的实例
        /// 插件必须有这个方法；否则使用反射的方式 会极大降低性能
        /// </summary>
        /// <returns></returns>
        public static IPlugin InstanceFactory()
        {
            return new TmallPlugin();
        }
        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public override  IPlugin CreateNew()
        {
            return new TmallPlugin();
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
                    Platform = NTCPMessage.EntityPackage.SupportPlatformEnum.Tmall,
                    TagName = "大衣", Value = "dayi", FilterFiled = "sku"
                } };
            resultBag.Add("Tags", lstTags);

            // 2 products  解析
            ProductBaseCollection lstProducts =new  ProductBaseCollection()
            {
                new TmallProduct { ItemId=1,Title="测试大衣"}
            };
            resultBag.Add("Products", lstProducts);



            return resultBag;// string.Concat("has process input :" + content);
        }

    }
}
