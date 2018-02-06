using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPeeker.Utilities
{
    public class Contanst
    {
        /// <summary>
        /// 分页存储过程调用名称
        /// </summary>
        public const string PageSql_Call_Name = "DbManage_GetRecordsByPage";//调用入口

        /// <summary>
        /// 登录授权后 客户端颁发的cookie 键
        /// </summary>
        public const string Login_Cookie_Client_Key = "huidangso.auth.token";

        /// <summary>
        /// 网站绑定的域名
        /// </summary>
        public const string Global_Site_Domain_Cookie= ".huidangso";

        ///// <summary>
        ///// 承载配置文件
        ///// </summary>
        //public const string Global_Config_Hosting = "hosting.json";

        /// <summary>
        /// 默认站点名称
        /// </summary>
        public const string Default_Site_Domain_Name = "惠当搜";


        /// <summary>
        /// 站点名称的配置节
        /// </summary>
        public const string Config_Node_SiteName = "siteName";
        /// <summary>
        /// 站点名称的配置节
        /// </summary>
        public const string Config_Node_SignTimeOut = "signTimeOut";
        /// <summary>
        /// 抓取页面是否开启结果缓存
        /// </summary>
        public const string Config_Node_IsFetchPageCacheaAble = "IsFetchPageCacheAble";

        /// <summary>
        /// 默认请求过期标识
        /// </summary>
        public const int Default_SignTimeOut = 5;
        /// <summary>
        /// cookie -浏览器用户标识
        /// </summary>
        public const string Cookie_Key_BrowserSign = "_ckb_sn";
    }
}
