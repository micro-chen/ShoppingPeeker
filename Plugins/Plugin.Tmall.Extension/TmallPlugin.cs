using System;
using System.Reflection;
using System.IO;
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


        public override object Execute(string content)
        {
            return string.Concat("has process input :" + content);
        }

    }
}
