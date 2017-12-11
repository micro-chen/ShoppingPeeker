using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ShoppingPeeker.Utilities;
//using ShoppingPeeker.DomainEntity;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using ShoppingPeeker.Utilities.Http;
using ShoppingPeeker.Utilities.DEncrypt;
using ShoppingPeeker.Utilities.Logging;

namespace ShoppingPeeker.Web.Mvc
{
    /// <summary>
    /// WebApi 访问授权过滤器
    /// 注意：一旦加上这个过滤，那么 要求请求中必须把sign 标识带过来 否则会出现访问异常
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthAttributeFilter: ActionFilterAttribute
    {
        /// <summary>
        /// 是否需要检测授权
        /// 可以给单个控制器添加一个属性，然后设置为false  表示不进行授权
        /// </summary>
        public bool IsCheck { get; set; }


       // private FormsAuthenticationService _formAuthService;


        public AuthAttributeFilter()
        {
            this.IsCheck = false;
           // this._formAuthService = new FormsAuthenticationService();
        }


		/// <summary>
		/// 执行action
		/// </summary>
		/// <param name="actionContext"></param>
		public override void OnActionExecuting(ActionExecutingContext  actionContext)
        {
            //base.OnActionExecuting(actionContext);

            if (false == IsCheck)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            //首先验证是否来自浏览器
            bool checkBrowser = this.CheckIsComeFromWebBrowser(actionContext);
            if (false==checkBrowser)
            {
                //不是来自浏览器，获取当前的分钟是否是奇数  如果是奇数 那么进入错误页面 ，偶数正常；目的是为了混淆请求，增加猜参数的难度
                if (DateTime.Now.Minute%2!=0)
                {
                    WorkContext.GoToErrorPage();//一旦是奇数 那么进入错误页
                    return;
                }
            }
            ////当前登录过的用户id
            //UserInfoModel currentUser = null;

            //currentUser = this._formAuthService.GetAuthenticatedCustomerFromCookie();


            ////验证通过 那么直接执行 action  否则返回错误
            //if (null != currentUser)
            //{

            //    base.OnActionExecuting(actionContext);//有权限的话 直接继续执行要访问的action
            //    return;

            //}
            //else
            //{

            //   //记录错误日志
            //    string toAccessUrl = actionContext.ControllerContext.Request.RequestUri.ToString();
            //    string msg = string.Concat("非法访问;IP地址：", HttpContext.Current.Request.GetIP(), "。访问地址：", toAccessUrl);
            //    Logger.WriteToLog(new LogEventArgs { LogMessage = msg, LogType = LoggingType.DbInfo });

            //    //输出错误信息
            //    var result = new BaseResult<string>
            //    {
            //         Status = CodeStatusTable.NotHaveAuth
            //        , Msg = CodeStatusTable.NotHaveAuth.GetEnumDescription()
            //    };
            //    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result);


            //}



        }


        /// <summary>
        /// 监测是否来自浏览器
        /// </summary>
        /// <param name="actionContext"></param>
        private bool CheckIsComeFromWebBrowser(ActionExecutingContext actionContext) {

            bool result = false;
            string userAgent = actionContext.HttpContext.Request.Headers[HttpServerProxy.RequestHeaderKeyUserAgent];
            //验证UA
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }

            //验证请求参数sign
            string requestSign = actionContext.HttpContext.Request.GetQuery<string>("sign");
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }
            else
            {
                //验证sign 标识
                var isValidSign = WorkContext.CheckIsValidRequestSign(requestSign);
                if (false == isValidSign)
                {
                    return false;
                }
            }

            //验证cookie 标识
            var cookieWebbrowserSign = actionContext.HttpContext.GetCookie<string>(Contanst.Cookie_Key_BrowserSign);
            if (string.IsNullOrEmpty(cookieWebbrowserSign))
            {
                return false;
            }

            try
            {
                //这是加密内容cookieWebbrowserSingn = string.Concat(WorkContext.SiteName, ":", DateTime.Now.ToString());
                //解密cookie
                string signText = LZString.Decompress(cookieWebbrowserSign, true);

                if (!string.IsNullOrEmpty(signText))
                {
                    if (signText.Contains(WorkContext.SiteName))
                    {
                        string time = signText.Split('|')[1];//获取里面的时间 超过3小时 必须刷新页面 否则过期
                        if (!string.IsNullOrEmpty(time)&&DateTime.Now.Subtract(time.ToDatetime()).Hours<3)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            return result;
        }

    }
}
