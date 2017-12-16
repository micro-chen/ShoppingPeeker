using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;

namespace ShoppingPeeker.Plugins
{
    public abstract class PluginBase: IPlugin
    {
        /// <summary>
        /// 创建插件新实例的方法
        /// </summary>
        public const string CreatNewInstanceMethodName = "InstanceFactory";

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
        /// 执行插件的方法
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract object Execute(string content);

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
            this._MetaManifest = new PluginMeta();
            if (string.IsNullOrEmpty(this.PluginDirectory))
            {
                throw new Exception("未能正确加载插件路径！");
            }
            string manifestPath = Path.Combine(this.PluginDirectory, "MetaManifest.txt");
            this._MetaManifest.LoadMetaManifest(manifestPath);
            
              //2 判断是否插件名称初始化;如果没有，那么使用插件dll名称
            if (string.IsNullOrEmpty(this._MetaManifest.Name))
            {
                var pluginFile=new DirectoryInfo(this.PluginDirectory)
                .EnumerateFiles("Plugin.*.Extension.dll")
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
