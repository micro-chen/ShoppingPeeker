using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using NTCPMessage.Client;

using ShoppingPeeker.Utilities.DataStructure;
using ShoppingPeeker.Utilities.Caching;
using ShoppingPeeker.Utilities.Logging;

namespace ShoppingPeeker.Utilities
{
    public static class ConfigHelper
    {

        #region 事件 定义
        /// <summary>
        /// 承载配置变更事件
        /// </summary>
        public static event EventHandler<ConfigChangedEventArgs> OnHostingConfigChangedEvent;
        #endregion


        #region 属性


        private static IConfiguration _hostingConfiguration;
        /// <summary>
        /// 运行时应用配置 host 配置
        /// </summary>
        public static IConfiguration HostingConfiguration
        {
            get
            {
                return _hostingConfiguration;
            }
            set
            {
                _hostingConfiguration = value;
            }
        }



        private static IConfiguration _appSettingsConfiguration;
        /// <summary>
        /// 应用程序的配置
        /// 系统启动 appsetting配置 DI对象
        /// </summary>
        public static IConfiguration AppSettingsConfiguration
        {
            get
            {
                return _appSettingsConfiguration;
            }
            set
            {
                _appSettingsConfiguration = value;
            }
        }


        private static ShoppingWebCrawlerSection _ShoppingWebCrawlerSection;
        /// <summary>
        /// 蜘蛛连接配置
        /// </summary>
        public static ShoppingWebCrawlerSection ShoppingWebCrawlerSection
        {
            get
            {
                if (null == _ShoppingWebCrawlerSection)
                {
                    _ShoppingWebCrawlerSection = GetShoppingWebCrawlerSection();
                }
                return _ShoppingWebCrawlerSection;
            }
        }

        private static IMemoryCache _MonitorConfingSnapshot;

        /// <summary>
        /// app 中的缓存 对象，每次访问一个配置 ，那么添加一个缓存。并创建一个
        /// 基于文件的缓存依赖，监视文件变更
        /// </summary>
        public static IMemoryCache MonitorConfingSnapshot
        {
            get
            {
                if (null == _MonitorConfingSnapshot)
                {
                    _MonitorConfingSnapshot = new MemoryCache(new MemoryCacheOptions());
                }
                return _MonitorConfingSnapshot;
            }
        }



        #endregion


        /// <summary>
        /// 从自定义的配置文件目录加载配置  /Configs 目录下的配置文件
        /// 系统默认的配置文件 appsettings.json 会跟随系统的DI 过来
        /// </summary>
        /// <param name="configFileName">json 文件名称</param>
        /// <param name="args">命令参数</param>
        /// <param name="isReloadOnChange">文件变更的时候，是否自动重新加载</param>
        /// <returns></returns>
        public static IConfiguration GetCustomConfiguration(string configFileName, string[] args = null, bool isReloadOnChange = true)
        {


            string configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
            string configFileFullPath = Path.Combine(configDir, configFileName);
            if (!File.Exists(configFileFullPath))
            {
                throw new FileNotFoundException(string.Format("the config file : {0} is not be found.", configFileFullPath));
            }

            var config = new ConfigurationBuilder()
                              .SetBasePath(configDir)
                              .AddJsonFile(configFileName, optional: true, reloadOnChange: isReloadOnChange)
                              .AddEnvironmentVariables(prefix: "ASPNETCORE_");

            if (null != args)
            {
                config.AddCommandLine(args);
            }


            var configRoot = config.Build();

            if (true == isReloadOnChange)
            {
                //如果设置为自动 加载，当变更的时候，会产生一个 IOptionsSnapshot 快照对象
                //但是这种强类型刷新快照按照官方的说法是基于DI 的方式，感觉找不到北，我们利用注册回调事件处理
                FileCacheDependency dependency = new FileCacheDependency(configFileFullPath);

                string snapshotKey = string.Concat("__Snapshot_Config_", configFileName);
                var value = DateTime.Now;

                PostEvictionDelegate handler = null;
                handler = (key, valueNew, reason, state) =>
                {
                    try
                    {
                        //强制刷新配置
                        configRoot.Reload();
                        //重新设定新的值 并从新插入到缓存中进行监听
                        value = DateTime.Now;
                        MonitorConfingSnapshot.Set(snapshotKey, value, dependency, handler);

                        //通知订阅的事件触发
                        if (configRoot == AppSettingsConfiguration && null != OnHostingConfigChangedEvent)
                        {
                            OnHostingConfigChangedEvent.Invoke("ConfigHelper", new ConfigChangedEventArgs { ResultOfChangedConfig = configRoot });
                        }
                        Logger.Info(string.Format("config file {0} has changed and the config object at application has reload!", configFileFullPath));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("can not Reload ConfigRoot! Error is:{0}", ex.ToString()));
                    }

                };

                MonitorConfingSnapshot.Set(snapshotKey, value, dependency, handler);


            }


            return configRoot;

        }

        /// <summary>
        /// 返回配置文件 *.json 中的指定key的 value
        /// </summary>
        /// <param name="config"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfig(this IConfiguration config, string key)
        {
            return config.GetValue<string>(key);
        }

        //public static string GetConfigString(string key)
        //{
        //    return AppSettingsConfiguration.GetValue<string>(key);
        //}

        /// <summary>
        /// 数据库配置字符串信息
        /// </summary>
        /// <returns></returns>
        public static ConnectionStringSection GetConnectionStringSection()
        {
            var key = ConnectionStringSection.SectionName;
            var section = AppSettingsConfiguration.GetSection(key);
            if (null == section)
            {
                throw new Exception("GetConnectionStringSection failed,not set ConnectionStringSection. ");
            }


            return section.Get<ConnectionStringSection>();

        }

        /// <summary>
        /// 获取配置蜘蛛节点信息
        /// </summary>
        /// <returns></returns>
        public static ShoppingWebCrawlerSection GetShoppingWebCrawlerSection()
        {
            var key = ShoppingWebCrawlerSection.SectionName;
            var section = AppSettingsConfiguration.GetSection(key);
            if (null == section)
            {
                throw new Exception("GetShoppingWebCrawlerSection failed,not set ShoppingWebCrawlerSection. ");
            }

            var configSection = section.Get<ShoppingWebCrawlerSection>();

            return configSection;
        }

        /// <summary>
        /// 得到AppSettings中的配置Bool信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetConfigBool(this IConfiguration config, string key)
        {
            bool result = false;
            string cfgVal = GetConfig(config, key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = bool.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }
            return result;
        }

        /// <summary>
        /// 得到AppSettings中的配置int信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetConfigInt(this IConfiguration config, string key)
        {
            int result = 0;
            string cfgVal = GetConfig(config, key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = int.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }

            return result;
        }

    }

    /// <summary>
    /// 配置变更 后触发的事件参数
    /// </summary>
    public class ConfigChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 变更后的 配置
        /// </summary>
        public IConfiguration ResultOfChangedConfig { get; set; }
    }
}
