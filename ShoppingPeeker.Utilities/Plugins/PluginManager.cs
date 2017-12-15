using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using ShoppingPeeker.Plugins;
using ShoppingPeeker.Utilities.TypeFinder;
using ShoppingPeeker.Utilities.Caching;
using Microsoft.Extensions.Caching.Memory;
using ShoppingPeeker.Utilities.Logging;

namespace ShoppingPeeker.Utilities.Plugins
{
    /// <summary>
    /// 插件管理器
    /// 插件dll的标准命名：Plugin.*.Extension.dll
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// 当前系统加载的插件集合
        /// </summary>
        public static ConcurrentDictionary<string, IPlugin> AppPlugins = new ConcurrentDictionary<string, IPlugin>();

        /// <summary>
        /// 插件放置的根目录
        /// </summary>
        public static string PluginRootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        /// <summary>
        /// 插件type
        /// </summary>
        private static Type _PluginType = typeof(PluginBase);
        /// <summary>
        /// 标识已经加载完毕插件的文件标识
        /// 一旦此文件 变更 或者被移除 ，那么重新加载全部的插件
        /// </summary>
        private const string _load_plugins_completed_token = "plugin_load_completed.bin";
        /// <summary>
        /// 自动发现插件
        /// 并监视Plugins文件夹，当里面的内容发生变更的时候，
        /// </summary>
        public static void AutoDiscoverPlugins()
        {
            //匹配全部的 插件格式的dll
            var pluginFiles = new DirectoryInfo(PluginRootDir)
                .EnumerateFiles("Plugin.*.Extension.dll", SearchOption.AllDirectories);//查询插件格式的dll;

            try
            {


                if (pluginFiles.IsNotEmpty())
                {
                    var typePlugin = _PluginType;
                    var typeFinder = new AppDomainTypeFinder();
                    List<Assembly> allPluginAssembly = new List<Assembly>();
                    foreach (var assFile in pluginFiles)
                    {
                        var ass = Assembly.LoadFrom(assFile.FullName);
                        allPluginAssembly.Add(ass);
                    }
                    var lstPluginTypes = typeFinder.FindClassesOfType(typePlugin, allPluginAssembly, true);
                    if (lstPluginTypes.IsNotEmpty())
                    {
                        foreach (var itemType in lstPluginTypes)
                        {
                            try
                            {
                                //仅仅加载可以正常实例化的插件，测试是否可以实例化
                                var pluginInstance = itemType.InvokeMember(
                                PluginBase.CreatNewInstanceMethodName,
                                BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                                null,
                                null,
                                new object[] { }) as IPlugin;
                                //注册到插件字典
                                AppPlugins.AddOrUpdate(
                                    pluginInstance.MetaManifest.Name,
                                    pluginInstance,
                                    (key, oldValue) => pluginInstance);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Error(ex);
            }
            finally
            {
                //监控插件变更
                ListenPluginsChanged();
            }
        }

        private static void ListenPluginsChanged()
        {
            string configFileFullPath = Path.Combine(PluginRootDir, _load_plugins_completed_token);
            //创建标识文件
            if (!File.Exists(configFileFullPath))
            {
                File.WriteAllText(configFileFullPath, DateTime.Now.ToString(), Encoding.UTF8);
            }
            FileCacheDependency dependency = new FileCacheDependency(configFileFullPath);

            string snapshotKey = string.Concat("__plugin_", _load_plugins_completed_token);
            var value = DateTime.Now;

            PostEvictionDelegate handler = null;
            handler = (key, valueNew, reason, state) =>
            {
                try
                {
                    Logger.Info(string.Format("plugin cache file {0} has changed and the plugins reload!", configFileFullPath));

                    //移除上次监视
                    ConfigHelper.MonitorConfingSnapshot.Remove(key);
                    //强制刷新插件，并进行下次的监视
                    PluginManager.AutoDiscoverPlugins();

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            };

            ConfigHelper.MonitorConfingSnapshot.Set(snapshotKey, value, dependency, handler);
        }

        /// <summary>
        /// 根据插件名，获取插件的实例
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public IPlugin Load(string pluginName)
        {
            IPlugin plugin = null;
            //1 首先尝试从注册登记的字典查询
            if (AppPlugins.ContainsKey(pluginName))
            {
                var pluginFactory = AppPlugins[pluginName];
                plugin = pluginFactory.CreateNew();
            }
            //2 如果依然为空的插件类实例，那么尝试从目录查找
            if (plugin==null)
            {
                //匹配全部的 插件格式的dll
                var pluginFiles = new DirectoryInfo(PluginRootDir)
                    .EnumerateFiles(pluginName+".dll", SearchOption.AllDirectories);//查询插件格式的dll;
                if (pluginFiles.IsNotEmpty())
                {
                    var assPath = pluginFiles.ElementAt(0).FullName;
                    var ass = Assembly.LoadFrom(assPath);
                    var itemType = new AppDomainTypeFinder()
                        .FindClassesOfType(_PluginType, new Assembly[] { ass }, true).FirstOrDefault();
                    if (null!= itemType)
                    {
                        //仅仅加载可以正常实例化的插件，测试是否可以实例化
                        plugin = itemType.InvokeMember(
                        PluginBase.CreatNewInstanceMethodName,
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                        null,
                        null,
                        new object[] { }) as IPlugin;
                        //注册到插件字典
                        AppPlugins.AddOrUpdate(
                            plugin.MetaManifest.Name,
                            plugin,
                            (key, oldValue) => plugin);
                    }
                }
            }
            return plugin;
        }
    }
}
