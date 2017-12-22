using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using NTCPMessage.Client;

using ShoppingPeeker.Utilities;
using ShoppingPeeker.Utilities.Ioc;
using ShoppingPeeker.Utilities.Logging;
using ShoppingPeeker.Utilities.Plugins;
using ShoppingPeeker.Plugins;

namespace ShoppingPeeker.Web
{
	public class Startup
	{
		/// <summary>
		/// 程序启动 加载入口配置
		/// </summary>
		/// <param name="configuration"></param>
		public Startup(IConfiguration configuration)
		{
			//保证配置的全局
			ConfigHelper.AppSettingsConfiguration = configuration;
            //配置蜘蛛程序连接池
            var crawlerConfigSection = ConfigHelper.ShoppingWebCrawlerSection;
            if (null != crawlerConfigSection)
            {
                SoapTcpPool.InitPoolManager(crawlerConfigSection.ConnectionStringCollection);
            }
            //插件自动加载
            PluginManager.AutoDiscoverPlugins();

        }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMemoryCache();
			services.AddMvc();

			//将http 上下文中间件 添加的依赖注册容器
			services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

			//var t= ConfigHelper.Configuration.GetValue<string>("option1");
		}


		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		/// <param name="svp">依赖注册的方式 ，内置DI容器</param>
		/// <param name="loggerFactory"></param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp, ILoggerFactory loggerFactory)
		{

            //配置mvc 环境
            //设置数据库连接
            //var connStringSection = ConfigHelper.GetConnectionStringSection();
            //var providerName = connStringSection.ProviderName;
            //InitDatabase.SetDatabaseConnection(connStringSection.ConnectionString, providerName);

            //将全局内置的DI容器暴露
            NativeIocContainer.ServiceProvider = svp;
			WorkContext.HostingEnvironment = env;

			#region 判断是否是线上

			env.EnvironmentName = EnvironmentName.Development;
			string webStatus = ConfigHelper.AppSettingsConfiguration.GetConfig("WebStatus");
			if (!string.IsNullOrEmpty(webStatus) && webStatus.Equals(EnvironmentName.Production.ToString()))
			{
				env.EnvironmentName = EnvironmentName.Production;
			}

			#endregion
			var logger = loggerFactory as LoggerFactory;
            Logger.LogFactory = loggerFactory;
            PluginContext.LogFactory = loggerFactory;



            logger.AddLog4Net();//注册log4日志记录组件
            if (env.IsDevelopment())
			{
				//开发环境 把日志打印到控制台
				logger.AddConsole(LogLevel.Debug);//这个扩展方法并没有Obsolete  在后续版本代码中  团队去除了这个标注
                logger.AddDebug();//将debug 日志信息 也同时输出
                
                app.UseDeveloperExceptionPage();
			}
			else
			{
				//通过异常自定义的 过滤器  记录日志
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				///mvc and webapi is in one  so add one route at here.
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				routes.MapRoute(
					name: "default-webapi",
					template: "api/{controller}/{action}/{id?}");
			});

			//配置 使用 ForwardedHeaders
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor |
				ForwardedHeaders.XForwardedProto
			});


		}
	}
}
