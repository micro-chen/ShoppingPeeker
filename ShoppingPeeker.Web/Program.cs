using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ShoppingPeeker.Utilities;

namespace ShoppingPeeker.Web
{
    public class Program
    {
        /// <summary>
        /// 应用程序载入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// 配置承载 host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args)
        {
            /*
              从配置文件 加载 host server  port 配置
              注意：如果是使用 IDE ，那么是从  launchSettings.json 加载的配置。此配置 是为发布包的启动进行的配置
              使用 dotnet run  。启动此端口的配置
             */

            //获取配置   并注册配置的变更事件
            var config = ConfigHelper.GetCustomConfiguration(Contanst.Global_Config_Hosting, args);
            ConfigHelper.HostingConfiguration = config;
            ConfigHelper.OnHostingConfigChangedEvent += WorkContext.ConfigHelper_OnHostingConfigChangedEvent;


            var host = WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(config)
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

            return host;

        }


    }
}
