using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Guomei.Extension
{
    /// <summary>
    /// 国美json 结果
    /// </summary>
    public class GuomeiPageJsonResut
    {
        public Content content { get; set; }
        //public Header header { get; set; }


        public class Content
        {
            //public commonInfo commonInfo { get; set; }
            //public facets facets { get; set; }
            //public pageBar pageBar { get; set; }
            public ProdInfo prodInfo { get; set; }
            //public selectData selectData { get; set; }
            //public seoData seoData { get; set; }
            //public toolBar0 toolBar0 { get; set; }
            //public zblueActivity zblueActivity { get; set; }
        }
        public class ProdInfo
        {
            public bool market { get; set; }
            public bool merchandise { get; set; }
            public bool clothes { get; set; }
            public IEnumerable<ProductItem> products { get; set; }
        }

        public class ProductItem
        {
            public string secondCat { get; set; }
            public int salesVolume { get; set; }
            public string promoDesc { get; set; }
            public string sUrl { get; set; }
            //public IEnumerable<string> brandIds { get; set; }
            //public string gomeCardType { get; set; }
            //public double score { get; set; }
            //public int promoFlag { get; set; }
            //public string cityName { get; set; }
            public int evaluateCount { get; set; }
            public int mallTag { get; set; }
            public int productTag { get; set; }
            public bool onSale { get; set; }
            public string shopId { get; set; }
            public int stock { get; set; }
            public string skuId { get; set; }
            //public int taoType { get; set; }
            //public IEnumerable<object> images { get; set; }
            //public bool XSearch { get; set; }
            public string sImg { get; set; }
            //public int shopFlag { get; set; }
            //public int rebate { get; set; }
            public string sName { get; set; }
            public string alt { get; set; }
            public int marketTag { get; set; }
            public double weight { get; set; }
            public string pId { get; set; }
            public decimal promoScore { get; set; }
            //public int isVip { get; set; }
            public string goodsType { get; set; }
            public string mUrl { get; set; }
            //public bool isMulti { get; set; }
            //public int energyTag { get; set; }
            //public int promoStock { get; set; }
            //public bool isBigImg { get; set; }
            public bool thirdProduct { get; set; }
            //public int _3ppFlag { get; set; }
            //public string skuNo { get; set; }
            //public string name { get; set; }
            //public string defCatId { get; set; }
            //public string shopType { get; set; }
            //public string firstCat { get; set; }
            //public string color { get; set; }
        }

    }

  
    //public class commonInfo
    //{
    //    public int searchLevel { get; set; }
    //    public string remain { get; set; }
    //    public string showWord { get; set; }
    //    public string illegal { get; set; }
    //}
    //public class commomCatFacets
    //{
    //    public int index { get; set; }
    //    public string id { get; set; }
    //    public string label { get; set; }
    //    public int type { get; set; }
    //    public IEnumerable<object> items { get; set; }
    //}
    //public class facets
    //{
    //    public IEnumerable<object> hotCategory { get; set; }
    //    public commomCatFacets commomCatFacets { get; set; }
    //}
    //public class pageBar
    //{
    //    public int pageNumber { get; set; }
    //    public int aggHwg { get; set; }
    //    public int totalPage { get; set; }
    //    public int pageSize { get; set; }
    //    public int totalCount { get; set; }
    //}


    //public class toolBar
    //{
    //    public int selectedInstock { get; set; }
    //    public string selectedSort { get; set; }
    //    public int slectedDeliv { get; set; }
    //}
    //public class category
    //{
    //}
    //public class selectData
    //{
    //    public toolBar toolBar { get; set; }
    //    public string keywords { get; set; }
    //    public bool isSearch { get; set; }
    //    public string ecBrandId { get; set; }
    //    public category category { get; set; }
    //    public string marketPage { get; set; }
    //    public facets facets { get; set; }
    //}
    //public class seoData
    //{
    //    public string sortNo { get; set; }
    //    public bool XSearch { get; set; }
    //    public int totalCount { get; set; }
    //}
    //public class market
    //{
    //    public bool isDefault { get; set; }
    //    public string url { get; set; }
    //}
    //public class promo
    //{
    //    public string url { get; set; }
    //}
    //public class sale
    //{
    //    public promo promo { get; set; }
    //    public rebate rebate { get; set; }
    //    public grabGroup grabGroup { get; set; }
    //}
    //public class deliv
    //{
    //    public delivMerchant delivMerchant { get; set; }
    //    public delivGome delivGome { get; set; }
    //    public delivAll delivAll { get; set; }
    //}
    //public class price
    //{
    //    public string url3 { get; set; }
    //    public string url1 { get; set; }
    //    public int isPriceSort { get; set; }
    //    public string url2 { get; set; }
    //}
    //public class sort
    //{
    //    public @default @default { get; set; }
    //    public sale sale { get; set; }
    //    public price price { get; set; }
    //    public evaluateCount evaluateCount { get; set; }
    //    public startDate startDate { get; set; }
    //}
    //public class toolBar0
    //{
    //    public market market { get; set; }
    //    public sale sale { get; set; }
    //    public int aggHwg { get; set; }
    //    public deliv deliv { get; set; }
    //    public sort sort { get; set; }
    //    public instock instock { get; set; }
    //}
    //public class zblueActivity
    //{
    //    public string promType { get; set; }
    //    public string promDesc { get; set; }
    //    public string couponValue { get; set; }
    //}

    //public class Header
    //{
    //    public bool bwSec { get; set; }
    //    public string refPage { get; set; }
    //    public string rawquestion { get; set; }
    //    public int appTime { get; set; }
    //    public int aCnt { get; set; }
    //    public string regionId { get; set; }
    //    public int bwFrom { get; set; }
    //    public string tagWightVersion { get; set; }
    //    public int bwSize { get; set; }
    //    public string rid { get; set; }
    //    public int status { get; set; }
    //}

}
