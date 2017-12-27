using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Taobao.Extension
{
    /// <summary>
    /// 淘宝返回的json查询结果模型
    /// </summary>
    public class TaobaoPageJsonResut
    {
        //public string pageName { get; set; }
        public Mods mods { get; set; }
        public MainInfo mainInfo { get; set; }
        //public Feature feature { get; set; }

        //public class shopcombotip
        //{
        //    public string status { get; set; }
        //}
        public class LevelClasses
        {
            public string levelClass { get; set; }
        }
        public class Shopcard
        {
            public IEnumerable<LevelClasses> levelClasses { get; set; }
            public bool isTmall { get; set; }
            public IEnumerable<int> delivery { get; set; }
            public IEnumerable<int> description { get; set; }
            public IEnumerable<int> service { get; set; }
            public string encryptedUserId { get; set; }
            public int sellerCredit { get; set; }
            public int totalRate { get; set; }
        }
        public class icon
        {
            public string title { get; set; }
            public string dom_class { get; set; }
            public string position { get; set; }
            public string show_type { get; set; }
            public string icon_category { get; set; }
            public string outer_text { get; set; }
            public string html { get; set; }
            public string icon_key { get; set; }
            public string trace { get; set; }
            public int traceIdx { get; set; }
            public string innerText { get; set; }
            public string url { get; set; }
        }
        //public class samestyle
        //{
        //    public string url { get; set; }
        //}
        //public class i2iTags
        //{
        //    public samestyle samestyle { get; set; }
        //    public similar similar { get; set; }
        //}
        public class Auctions
        {
            public int p4p { get; set; }
            public bool p4pSameHeight { get; set; }
            public string nid { get; set; }
            public string category { get; set; }
            public string pid { get; set; }
            public string title { get; set; }
            public string raw_title { get; set; }
            public string pic_url { get; set; }
            public string detail_url { get; set; }
            public string view_price { get; set; }
            public string view_fee { get; set; }
            public string item_loc { get; set; }
            public string view_sales { get; set; }
            public string comment_count { get; set; }
            public string user_id { get; set; }
            public string nick { get; set; }
            public Shopcard shopcard { get; set; }
            public IEnumerable<icon> icon { get; set; }
            public bool isHideIM { get; set; }
            public bool isHideNick { get; set; }
            public string comment_url { get; set; }
            public string shopLink { get; set; }
            //public i2iTags i2iTags { get; set; }
            public IEnumerable<object> p4pTags { get; set; }
            public string risk { get; set; }
            public string recommend_nav { get; set; }
        }
        public class itemData
        {
            public string postFeeText { get; set; }
            public string trace { get; set; }
            public IEnumerable<Auctions> auctions { get; set; }
            public IEnumerable<object> recommendAuctions { get; set; }
            public bool isSameStyleView { get; set; }
            public IEnumerable<object> sellers { get; set; }
            public string query { get; set; }
            public string spmModId { get; set; }
            public string clickstaturl { get; set; }
        }
        public class itemlist
        {
            public string status { get; set; }
            public itemData data { get; set; }
        }
        public class traceData
        {
            public string click { get; set; }
        }
        public class sub
        {
            public string text { get; set; }
            public bool isExpandShow { get; set; }
            public string key { get; set; }
            public string value { get; set; }
            //public string trace { get; set; }
            //public traceData traceData { get; set; }
        }
        public class common
        {
            public string text { get; set; }
            public string type { get; set; }
            //public bool isMulti { get; set; }
            public IEnumerable<sub> sub { get; set; }
          //  public bool forceShowMore { get; set; }
            //public string trace { get; set; }
            public bool show2line { get; set; }
            public string key { get; set; }
            public string value { get; set; }
            //public traceData traceData { get; set; }
        }
        public class adv
        {
            public string text { get; set; }
            public string type { get; set; }
            public bool isMulti { get; set; }
            public IEnumerable<sub> sub { get; set; }
            //public bool forceShowMore { get; set; }
            //public string trace { get; set; }
        }
        public class catpath
        {
            public string catid { get; set; }
            public string name { get; set; }
        }
        public class breadcrumbs
        {
            public IEnumerable<catpath> catpath { get; set; }
        }
        public class navData
        {
            public IEnumerable<common> common { get; set; }
            public IEnumerable<adv> adv { get; set; }
            public breadcrumbs breadcrumbs { get; set; }
        }
        public class nav
        {
            public string status { get; set; }
            public navData data { get; set; }
        }
        public class Mods
        {
            //public shopcombotip shopcombotip { get; set; }
            //public phonenav phonenav { get; set; }
            //public debugbar debugbar { get; set; }
            //public shopcombo shopcombo { get; set; }
            public itemlist itemlist { get; set; }
            //public bottomsearch bottomsearch { get; set; }
            //public tips tips { get; set; }
            //public feedback feedback { get; set; }
            //public navtabtags navtabtags { get; set; }
            //public sc sc { get; set; }
            //public bgshopstar bgshopstar { get; set; }
            //public spuseries spuseries { get; set; }
            //public related related { get; set; }
            //public tab tab { get; set; }
            //public pager pager { get; set; }
            //public apasstips apasstips { get; set; }
            //public tbcode tbcode { get; set; }
            //public vbaby vbaby { get; set; }
            //public hongbao hongbao { get; set; }
            public nav nav { get; set; }
            //public sortbar sortbar { get; set; }
            //public d11filterbar d11filterbar { get; set; }
            //public personalbar personalbar { get; set; }
            //public p4p p4p { get; set; }
            //public choosecar choosecar { get; set; }
            //public shopstar shopstar { get; set; }
            //public header header { get; set; }
            //public spucombo spucombo { get; set; }
            //public supertab supertab { get; set; }
            //public navtablink navtablink { get; set; }
            //public noresult noresult { get; set; }
        }
        public class modLinks
        {
            public string filter { get; set; }
            public string @default { get; set; }
            public string nav { get; set; }
            public string breadcrumb { get; set; }
            public string pager { get; set; }
            public string tab { get; set; }
            public string sortbar { get; set; }
        }
        //public class srpGlobal
        //{
        //    public string q { get; set; }
        //    public string encode_q { get; set; }
        //    public string utf8_q { get; set; }
        //    public string cat { get; set; }
        //    public string catLevelOne { get; set; }
        //    public string ppath { get; set; }
        //    public int s { get; set; }
        //    public string tnk { get; set; }
        //    public int bucketid { get; set; }
        //    public string multi_bucket { get; set; }
        //    public string style { get; set; }
        //    public string initiative_id { get; set; }
        //    public string machine { get; set; }
        //    public string buckets { get; set; }
        //    public string sp_url { get; set; }
        //    public string srpName { get; set; }
        //}
        //public class traceData
        //{
        //    public string catdirect { get; set; }
        //    public string remoteip { get; set; }
        //    public string tabType { get; set; }
        //    public string is_rs { get; set; }
        //    public string catpredict_bury { get; set; }
        //    public string hostname { get; set; }
        //    public IEnumerable<string> activityClick { get; set; }
        //    public string lastAlitrackid { get; set; }
        //    public string at_lflog { get; set; }
        //    public string list_model { get; set; }
        //    public string page_size { get; set; }
        //    public IEnumerable<string> rsPositions { get; set; }
        //    public string if_tank { get; set; }
        //    public string rsshop { get; set; }
        //    public string alitrackid { get; set; }
        //    public string cps { get; set; }
        //    public string query { get; set; }
        //    public string price_rank { get; set; }
        //    public string sort { get; set; }
        //    public string catLevelOne { get; set; }
        //    public IEnumerable<string> auctionNids { get; set; }
        //    public IEnumerable<string> ifDoufuAuction { get; set; }
        //    public string at_host { get; set; }
        //    public string querytype_bury { get; set; }
        //    public IEnumerable<string> allOldBiz30Day { get; set; }
        //    public string tdTags { get; set; }
        //    public IEnumerable<string> relateHotTrace { get; set; }
        //    public string totalHits { get; set; }
        //    public IEnumerable<string> allCategories { get; set; }
        //    public IEnumerable<string> auctionIconServices { get; set; }
        //    public string rn { get; set; }
        //    public IEnumerable<string> isp4p { get; set; }
        //    public string rs { get; set; }
        //    public string colo { get; set; }
        //    public IEnumerable<string> allPrices { get; set; }
        //    public string show_compass { get; set; }
        //    public IEnumerable<string> auctionPrices { get; set; }
        //    public string auctionReturnNum { get; set; }
        //    public string multivariate { get; set; }
        //    public IEnumerable<string> p4pDelTraceInfo { get; set; }
        //    public string bucketId { get; set; }
        //    public string rewrite_bury { get; set; }
        //    public string nick { get; set; }
        //    public IEnumerable<string> allPersonalUpReason { get; set; }
        //    public IEnumerable<object> allDoufuNids { get; set; }
        //    public IEnumerable<string> priceSorts { get; set; }
        //    public string at_bucketid { get; set; }
        //    public string srppage { get; set; }
        //    public string if_rs { get; set; }
        //    public IEnumerable<string> allNids { get; set; }
        //    public string cat { get; set; }
        //    public string statsClickInUrl { get; set; }
        //    public string spUrl { get; set; }
        //    public string sort2 { get; set; }
        //    public string qp_bury { get; set; }
        //    public string doufuAuctionNum { get; set; }
        //    public string at_colo { get; set; }
        //    public string bandit { get; set; }
        //    public string rs_count { get; set; }
        //    public string has_sku_pic { get; set; }
        //    public string from_pos { get; set; }
        //    public string statsClick { get; set; }
        //    public IEnumerable<object> allDoufuPrices { get; set; }
        //    public IEnumerable<string> rsKeywords { get; set; }
        //    public IEnumerable<string> tagList { get; set; }
        //    public IEnumerable<string> auctionNicks { get; set; }
        //    public IEnumerable<string> sp_seller_types { get; set; }
        //    public string catdirectForMaidian { get; set; }
        //    public string qinfo { get; set; }
        //    public string noResultCode { get; set; }
        //    public string apass { get; set; }
        //    public string spu_combo { get; set; }
        //    public IEnumerable<string> allTags { get; set; }
        //    public string multi_bucket { get; set; }
        //    public string navStatus { get; set; }
        //}
        //public class traceInfo
        //{
        //    public string pvStat { get; set; }
        //    public traceData traceData { get; set; }
        //}
        public class MainInfo
        {
            public string currentUrl { get; set; }
            public modLinks modLinks { get; set; }
            //public srpGlobal srpGlobal { get; set; }
            //public traceInfo traceInfo { get; set; }
            public IEnumerable<object> remainMods { get; set; }
        }
        //public class Feature
        //{
        //    public bool webpOff { get; set; }
        //    public bool retinaOff { get; set; }
        //    public bool shopcardOff { get; set; }
        //}

    }
}
