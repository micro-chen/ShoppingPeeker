using System;
using ShoppingPeeker.Web.Mvc;


namespace ShoppingPeeker.Web.Mvc
{
    public class JsonPostResult<T> : BaseResult<T>
    {
        /// <summary>
        /// Post后获取到的数据
        /// </summary>
        public object PostData { get; set; }
    }
}
