using System;
using System.Collections.Generic;
using System.Text;
using ShoppingPeeker.Utilities;

namespace ShoppingPeeker.Utilities.DataStructure
{
    public class AuthenticationTicket
    {

        public AuthenticationTicket()
        {
            this.Name = Contanst.Login_Cookie_Client_Key;
            this.CookiePath = "/";
        }
        /// <summary>
        /// 获取身份验证票证的 Cookie 路径。
        /// </summary>
        public string CookiePath { get; set; }
        /// <summary>
        /// 获取 身份验证票证过期时的本地日期和时间。
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// 获取一个值，它指示 Forms 身份验证票证是否已过期。
        /// 是否不在有效期内
        /// </summary>
        public bool Expired
        {
            get
            {
                return this.Expiration < DateTime.Now ? true : false;
            }
        }

        /// <summary>
        /// 获取最初发出  身份验证票证时的本地日期和时间。
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// 获取与  身份验证票相关联的用户名。
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 获取一个存储在票证中的用户特定的字符串。
        /// </summary>
        public string UserData { get; set; }
    }
}
