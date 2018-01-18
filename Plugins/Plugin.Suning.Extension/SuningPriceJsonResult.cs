using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Suning.Extension
{
 
    public class SuningPriceJsonResult
    {
        public int status { get; set; }
        public IEnumerable<PriceItem> rs { get; set; }
        public string message { get; set; }



        //public class explosion
        //{
        //    public string imageUrl { get; set; }
        //    public string patternCss { get; set; }
        //    public string text { get; set; }
        //}
        //public class dr
        //{
        //}
        public class PriceItem
        {
            public string cmmdtyCode { get; set; }
            public decimal price { get; set; }
            //public string priceType { get; set; }
            //public decimal vipPrice { get; set; }
            public string bizCode { get; set; }
            public string vendorName { get; set; }
            public string vendorType { get; set; }
            //public decimal govPrice { get; set; }
            //public string type { get; set; }
            //public string subCode { get; set; }
            //public string invStatus { get; set; }
            //public string locatCode { get; set; }
            //public string stdLocatCode { get; set; }
            //public string plantCode { get; set; }
            //public string chargePlantCode { get; set; }
            //public string cityFrom { get; set; }
            //public string arrivalDate { get; set; }
            //public string purchaseFlag { get; set; }
            //public string vendorType { get; set; }
            //public string supplierCode { get; set; }
            //public string commondityTry { get; set; }
            //public string reservationType { get; set; }
            //public string reservationPrice { get; set; }
            //public string subscribeType { get; set; }
            //public string subscribePrice { get; set; }
            //public string collection { get; set; }
            //public string visited { get; set; }
            //public string sellingPoint { get; set; }
            //public IEnumerable<object> promoTypes { get; set; }
            //public IEnumerable<object> promotionList { get; set; }
            //public string imageUrl { get; set; }
            //public string patternCss { get; set; }
            //public string text { get; set; }
            //public string energySubsidy { get; set; }
            //public string feature { get; set; }
            //public string priceDifference { get; set; }
            //public string jdPrice { get; set; }
            //public string jdPriceUpdateTime { get; set; }
            //public string snPrice { get; set; }
            //public string refPrice { get; set; }
            //public string discount { get; set; }
            //public string originalPrice { get; set; }
            //public string oversea { get; set; }
            //public string shoppingCart { get; set; }
            //public string bigPromotion { get; set; }
            //public string storeStock { get; set; }
            //public string distance { get; set; }
            //public string storeStockName { get; set; }
            //public string prototype { get; set; }
            //public string prototypeStoreName { get; set; }
            //public string prototypeDistance { get; set; }
            //public IEnumerable<explosion> explosion { get; set; }
            //public string subCodeImageVersion { get; set; }
            //public string directoryIds { get; set; }
            //public string pinPrice { get; set; }
            //public string promotionLable { get; set; }
            //public string promotionColor { get; set; }
            //public string purchase { get; set; }
            //public string replacementRisk { get; set; }
            //public string minimumSale { get; set; }
            //public dr dr { get; set; }
        }

    }

}
