using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Guomei.Extension
{
    /// <summary>
    /// 国美查询价格的json结果
    /// </summary>
    public class GuomeiPriceJsonResult
    {
        public bool success { get; set; }
        public Result result { get; set; }


        public class Result
        {
            public decimal price { get; set; }
            //public string priceType { get; set; }
            public string productId { get; set; }
            public string skuId { get; set; }
        }

    }
}
