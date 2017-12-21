using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;

namespace ShoppingPeeker.Plugins
{
    /// <summary>
    /// 插件中的常量定义
    /// </summary>
    public class PluginConstant
    {
        /// <summary>
        /// 创建插件新实例的方法
        /// </summary>
        public const string InstanceFactoryMethodName = "InstanceFactory";
        /// <summary>
        /// 插件的自描述文件名
        /// </summary>
        public const string PluginMetaManifestName = "MetaManifest.txt";

        /// <summary>
        /// 插件的文件格式
        /// </summary>
        public const string PluginFileNameFormat = "Plugin.*.Extension.dll";

    }
    /// <summary>
    /// 插件的基类
    /// 不要使用基于泛型的基类，会出现崩溃-core 2.0的bug
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PluginBase<T>: IPlugin
        where T: PluginBase<T>,new ()
    {

        /// <summary>
        /// 创建插件的实例
        /// 插件必须有这个方法；否则使用反射的方式 会极大降低性能
        /// </summary>
        /// <returns></returns>
        public static IPlugin InstanceFactory()
        {
            var instance = new T();
            return instance;
        }


        /// <summary>
        /// 抽象属性，插件所在的目录
        /// </summary>
        public abstract string PluginDirectory { get; }


        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        public abstract  IPlugin CreateNew();
        /// <summary>
        /// 初始化操作
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// 尝试解析 来自web 参数
        /// 解析为具体的平台的搜索地址：附带参数
        /// </summary>
        /// <param name="webArgs"></param>
        /// <returns></returns>
        public virtual ResolvedSearchUrlWithParas ResolveSearchUrl(BaseFetchWebPageArgument webArgs)
        {
            return null;
        }

        /// <summary>
        /// 执行插件的方法
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract Dictionary<string, object> ResolveSearchPageContent(string content);

        public PluginMeta _MetaManifest;
        public PluginMeta MetaManifest
        {
            get
            {
                return _MetaManifest;
            }
        }

        public PluginBase()
        {
            this.Initialize();

            this._MetaManifest = new PluginMeta();
            if (string.IsNullOrEmpty(this.PluginDirectory))
            {
                throw new Exception("未能正确加载插件路径！");
            }
            string manifestPath = Path.Combine(this.PluginDirectory, PluginConstant.PluginMetaManifestName);
            this._MetaManifest.LoadMetaManifest(manifestPath);
            
              //2 判断是否插件名称初始化;如果没有，那么使用插件dll名称
            if (string.IsNullOrEmpty(this._MetaManifest.Name))
            {
                var pluginFile=new DirectoryInfo(this.PluginDirectory)
                .EnumerateFiles(PluginConstant.PluginFileNameFormat)
                .FirstOrDefault();

                if (null!=pluginFile)
                {
                    this._MetaManifest.Name = Path.GetFileNameWithoutExtension( pluginFile.FullName);
                }
            }
            //3 判断是否插件名称初始化;如果没有，那么使用插件所在的文件夹名称
            if (string.IsNullOrEmpty(this._MetaManifest.Name))
            {
                this._MetaManifest.Name = new DirectoryInfo(this.PluginDirectory).Name;
            }
        }
    }
}
