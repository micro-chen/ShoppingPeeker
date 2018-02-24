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
        /// <summary>
        /// 热搜词汇
        /// </summary>
        public List<string> HotWords { get; set; }
        #endregion

        public HomePageViewModel()
        {
            this.ServerTime = DateTime.Now;
            this.HotWords = new List<string>();
        }


    }
}
