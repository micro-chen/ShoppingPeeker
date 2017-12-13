using System;
using System.Collections.Generic;

namespace ShoppingPeeker.Web.ViewModels
{
    public class HomePageViewModel
    {
        #region 属性
        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime ServerTime { get;  private set; }

        #endregion

        public HomePageViewModel()
        {
            this.ServerTime = DateTime.Now;
        }


    }
}
