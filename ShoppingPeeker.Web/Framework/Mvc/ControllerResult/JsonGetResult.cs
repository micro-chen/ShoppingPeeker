using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingPeeker.Web.Mvc;


namespace ShoppingPeeker.Web.Mvc
{
    public class JsonGetResult<T> : BaseResult<T>
    {
        /// <summary>
        /// 需要跳转的链接地址
        /// </summary>
        public string Url { get; set; }
    }
}
