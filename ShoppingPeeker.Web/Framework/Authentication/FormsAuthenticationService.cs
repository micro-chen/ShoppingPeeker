using System;
using System.Web;
using System.Linq;

using ShoppingPeeker.Utilities;
//using ShoppingPeeker.DbManage;
//using ShoppingPeeker.DomainEntity;
using ShoppingPeeker.Utilities.DEncrypt;
using ShoppingPeeker.Utilities.Interface;
using Microsoft.AspNetCore.Http;

/*
 基本的身份验证
 模拟表单验证的，验证登录的Cookie
 */
namespace ShoppingPeeker.Web.Authentication
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public  class FormsAuthenticationService
    {
        #region  字段
        /// <summary>
        /// 默认5分钟  登录超时（滑动超时）
        /// </summary>
        private readonly TimeSpan _expirationTimeSpan = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 加密/解密服务
        /// </summary>
        private readonly EncryptionService _encryptionService = new EncryptionService();
        #endregion




        /// <summary>
        /// Ctor
        /// </summary>
        public FormsAuthenticationService()
        {
            //初始化  登录超时设置
            var authConfig = ConfigHelper.HostingConfiguration.GetConfig("AuthTimeout");// FormsAuthentication.Timeout;
            if (!authConfig.IsNullOrEmpty())
            {
                double tim;
                double.TryParse(authConfig, out tim);
                if (tim > 0)
                {
                    this._expirationTimeSpan = TimeSpan.FromMinutes(tim);

                }

            }
        }

        /// <summary>
        /// 登录使用的表单+Cookie的验证
        /// 验证通过后 会在客户端生成带有验证的身份标识的Cookie，当再一次回发到服务器的时候
        /// 会将身份标识解析到上下文中
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isRememberLogin"></param>
        public virtual bool SignIn(string userName, string pwd)
        {
            var result = false;

            //UserInfoModel user = null;

            ////---------Todo:通过【统一管理服务】拉取用户信息
            ////user = dal_Users
            ////      .GetElementsByCondition(x => x.UserName==userName)
            ////      .FirstOrDefault();

            //if (null == user)
            //{
            //    return result;
            //}

            ////2 查询出来用户后  对比加密过的授权信息，license key
            ////var encryPwd = EncryptionService.CreatePasswordHash(pwd, user.PasswordSalt);
            ////if (!string.Equals(encryPwd,user.Password))
            ////{
            ////    return result;
            ////}
            ////else
            ////{
            ////    result = true;
            ////}


            ////3 验证通过后  授权Cookie的创建  创建票据 并加密
            //var tokenData = user.ToJson().KeyEncrypt();
            //var ticket = this._encryptionService.GenerateFormsAuthenticationTicket(tokenData, this._expirationTimeSpan);
            //var encryptedTicket = this._encryptionService.EncryptFormsAuthenticationTicket(ticket);

            ////var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            ////cookie.HttpOnly = true;
            ////if (ticket.IsPersistent)
            ////{
            ////    cookie.Expires = ticket.Expiration;
            ////}
            //////cookie.Secure = FormsAuthentication.RequireSSL;
            ////cookie.Path = FormsAuthentication.FormsCookiePath;
            ////if (FormsAuthentication.CookieDomain != null)
            ////{
            ////    cookie.Domain = FormsAuthentication.CookieDomain;
            ////}

            //HttpContext.Current.SetCookie<string>(FormsAuthentication.CookieDomain,
            //    Contanst.Login_Cookie_Client_Key,
            //    encryptedTicket
            //    );


            result = true;
            return result;
        }

        /// <summary>
        /// 清除客户端的登录凭据
        /// </summary>
        public virtual void SignOut()
        {
            WorkContext.HttpContext.RemoveCookie(Contanst.Global_Site_Domain_Cookie, Contanst.Login_Cookie_Client_Key);
        }

        ///// <summary>
        ///// 从验证过的Cookie中获取登录用户信息
        ///// </summary>
        ///// <returns></returns>
        //public virtual UserInfoModel GetAuthenticatedCustomerFromCookie()
        //{

        //    try
        //    {


        //        //逆向
        //        var encryptedTicket = HttpContext.Current.GetCookie<string>(Contanst.Login_Cookie_Client_Key);// this._encryptionService.EncryptFormsAuthenticationTicket(ticket);
        //        if (encryptedTicket.IsNullOrEmpty())
        //        {
        //            return null;
        //        }
        //        //得到凭据
        //        var ticket = this._encryptionService.DecryptFormsAuthenticationTicket(encryptedTicket);
        //        if (null == ticket || string.IsNullOrEmpty(ticket.UserData))
        //        {
        //            return null;
        //        }

        //        //解密凭据中的数据

        //        var tokenData = ticket.UserData.KeyDecrypt();

        //        if (!tokenData.IsNullOrEmpty())
        //        {
        //            tokenData = tokenData.KeyDecrypt();//解密数据
        //        }


        //        return tokenData.FromJson<UserInfoModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //}

    }
}