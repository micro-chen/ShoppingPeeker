using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShoppingPeeker.Utilities;
using ShoppingPeeker.Utilities.DEncrypt;
using ShoppingPeeker.Utilities.Interface;
using ShoppingPeeker.Utilities.Ioc;
using ShoppingPeeker.Web.Framework;
using Microsoft.AspNetCore.Hosting;

namespace ShoppingPeeker.Web.Mvc
{
    public class BaseMvcController : Controller
    {


        /// <summary>
        /// 获取操作类实例（单例模式)
        /// </summary>
        /// <typeparam name="T">需要获取的业务类类型</typeparam>
        /// <returns>业务类型对象</returns>
        protected T Single<T>() where T : IService, new()
        {
            if (null == Singleton<T>.Instance)
            {
                Singleton<T>.Instance = new T();
            }
            return Singleton<T>.Instance;
        }

        /// <summary>
        /// 重写  控制器的 action 执行方法
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //注意 web api 在本设计中 ，不让设置 页面标识 Cookie 。增加这个Cookie 是为了防止采集和蜘蛛
            if (context.Controller is BaseApiController)
            {
                base.OnActionExecuting(context);
                return;
            }

            //1监测是否有了必须的Cookie标识  如果没有 那么追加
            var cookieWebbrowserSign = context.HttpContext.GetCookie<string>(Contanst.Cookie_Key_BrowserSign);
            if (string.IsNullOrEmpty(cookieWebbrowserSign))
            {
                cookieWebbrowserSign = string.Concat(WorkContext.SiteName, "|", DateTime.Now.ToOfenTimeString());
                string encodedSign = LZString.Compress(cookieWebbrowserSign, true);
                //将加密后的cookie  写入到响应客户端 
                string domain = null;
                //string webStatus = ConfigHelper.AppSettingsConfiguration.GetConfig("WebStatus");
                //判断是否为正式环境
                if (WorkContext.HostingEnvironment.IsProduction())
                {
                    //正式环境cookie 过期为1天
                    domain = Contanst.Global_Site_Domain_Cookie;
                    context.HttpContext.SetCookie<string>(domain, Contanst.Cookie_Key_BrowserSign, encodedSign, 1, true);
                }
                else
                {
                    //测试环境 cookie 不过期
                    context.HttpContext.SetCookie<string>(domain, Contanst.Cookie_Key_BrowserSign, encodedSign, true);
                }
            }
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get
            {
                int pNumber = 0;
                try
                {
                    pNumber = HttpContext.Request.GetQuery<int>("pid");

                }
                catch
                {

                    pNumber = 0;
                }

                if (pNumber <= 0)
                {
                    return pNumber;
                }
                else
                {
                    return pNumber -= 1;//页码-1 为页索引
                }

            }
        }



        /// <summary>
        /// 默认分页大小
        /// </summary>
        protected int DefaultPagerSize { get { return 1; } }
    }
}
