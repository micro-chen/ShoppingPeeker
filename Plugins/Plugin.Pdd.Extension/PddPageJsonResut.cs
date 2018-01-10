using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Pdd.Extension
{
    /// <summary>
    /// 拼多多搜索结果json对象结构
    /// </summary>
    public class PddPageJsonResut
    {
        //public Filter filter { get; set; }
        //public int q_opt { get; set; }
        //public Ads ads { get; set; }
        //public bool is_black { get; set; }
        //public int server_time { get; set; }
        //public string qc { get; set; }
        //public int total { get; set; }
        //public long server_time_ms { get; set; }
        //public IEnumerable<object> debugInfo { get; set; }
        //public bool need_ad_logo { get; set; }
        //public int qc_level { get; set; }
        public IEnumerable<Items> items { get; set; }



        //public class price
        //{
        //    public int start { get; set; }
        //    public int end { get; set; }
        //}
        //public class Filter
        //{
        //    public IEnumerable<price> price { get; set; }
        //}
        //public class Ads
        //{
        //    public IEnumerable<object> malls { get; set; }
        //}
        //public class exp_list
        //{
        //    public string bucket { get; set; }
        //    public string name { get; set; }
        //    public string exp { get; set; }
        //    public string strategy { get; set; }
        //}
        //public class ad
        //{
        //    public bool needAdLogo { get; set; }
        //    public int scene_id { get; set; }
        //    public int goods_id { get; set; }
        //    public IEnumerable<int> cate_list { get; set; }
        //    public string search_query { get; set; }
        //    public IEnumerable<exp_list> exp_list { get; set; }
        //    public string search_id { get; set; }
        //    public IEnumerable<object> opt_list { get; set; }
        //    public int ad_id { get; set; }
        //    public int trigger_goods_id { get; set; }
        //    public int mall_id { get; set; }
        //    public int match_type { get; set; }
        //    public string bid { get; set; }
        //    public string keyword { get; set; }
        //    public int plan_id { get; set; }
        //}
        //public class Icon
        //{
        //    public int id { get; set; }
        //    public string url { get; set; }
        //}
        //public class p_search
        //{
        //}
        public class Items
        {
            public string goods_name { get; set; }
            //public string country { get; set; }
            public string thumb_url { get; set; }
            public int is_app { get; set; }
            //public ad ad { get; set; }
            public int coupon { get; set; }
            public string image_url { get; set; }
            //public Icon icon { get; set; }
            public int goods_id { get; set; }
            public string hd_thumb_url { get; set; }
            public int customer_num { get; set; }
            public double sales { get; set; }
            //public int event_type { get; set; }
            public int normal_price { get; set; }
            public decimal price { get; set; }
            public int market_price { get; set; }
            //public p_search p_search { get; set; }
            public string short_name { get; set; }
            public int tag { get; set; }
        }


    }

}
