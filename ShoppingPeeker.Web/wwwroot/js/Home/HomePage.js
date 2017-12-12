/// <reference path="../Extension/jquery-1.10.2.js" />
/// <reference path="../Extension/jquery-1.10.2.intellisense.js" />
/// <reference path="../Extension/applicationCore.js" />
/// <reference path="../Extension/httpClient.js" />
/// <reference path="../Extension/autocomplete/autocomplete.js" />

/*bind to home page
注意：使用基于闭包的形式，防止 全局变量导致的变量冲突和引用问题
*/

$(function () {

    var homePage = {

        /*page controls begin*/
        //btn_save: $("#btn_save"),//保存按钮
        txt_search_keyword: $('#txt_search_keyword'),//搜索输入框
        btn_search: $('#btn_search'),//搜索按钮
        /*page controls end*/

        api_auto_complete_suggest: "api/servicebus/suggest",//搜索框自动完成api
        //平台商品检索配置

        api_search_tmall_products: "api/servicebus/search_tmall_products",//天猫商品检索
        api_search_taobao_products: "api/servicebus/search_taobao_products",//淘宝商品检索
        api_search_jd_products: "api/servicebus/search_jd_products",//京东商品检索
        api_search_pdd_products: "api/servicebus/search_pdd_products",//拼多多商品检索
        api_search_vip_products: "api/servicebus/search_vip_products",//唯品会商品检索
        api_search_guomei_products: "api/servicebus/search_guomei_products",//国美商品检索
        api_search_suning_products: "api/servicebus/search_suning_products",//苏宁商品检索
        api_search_yhd_products: "api/servicebus/search_yhd_products",//一号店商品检索
        api_search_mls_products: "api/servicebus/search_mls_products",//美丽说商品检索
        api_search_mgj_products: "api/servicebus/search_mgj_products",//蘑菇街商品检索
        api_search_dangdang_products: "api/servicebus/search_dangdang_products",//当当商品检索
        api_search_zhe800_products: "api/servicebus/search_zhe800_products",//zhe800商品检索
        api_search_etao_products: "api/servicebus/search_etao_products",//一淘商品检索
        api_search_taoquan: "api/servicebus/search_taoquan",//淘宝天猫优惠券检索


        /*init page */
        init: function (agrs) {
            //debugger;
            //点击保存按钮事件
            //this.btn_save.click(homePage.saveDetails);
            //搜索输入框自动完成事件
            this.txt_search_keyword.autocomplete({
                serviceUrl: this.api_auto_complete_suggest,
                dataType: "json",
                deferRequestBy: 300,//不要立即请求 间隔一个缓冲
                paramName: "key",
                params: { "key": this.txt_search_keyword.val(), "sign": ShoppingPeeker.apiSignFunc() },
                onSelect: function (suggestion) {
                    console.log('You selected: ' + suggestion.value + ', ' + suggestion.data);
                    if (!isNullOrEmpty(suggestion.value)) {
                        homePage.txt_search_keyword.val(suggestion.value)
                    }

                }
            });

            /*搜索输入框的回车事件*/
            this.txt_search_keyword.keyup(homePage.btnSearchHandler);
            /*搜索按钮点击事件*/
            this.btn_search.click(homePage.btnSearchHandler);


        },
        /*save handler
        saveDetails: function () {
            console.log("has save");
        },*/
        /*search handler
        搜索按钮点击事件，将关键词发送到服务端进行检索商品
        */
        btnSearchHandler: function () {
            console.log("has btnSearchHandler");

            var keyWord = homePage.txt_search_keyword.val();
            if (isNullOrEmpty(keyWord)) {
                var warnInfo = homePage.txt_search_keyword.attr("placeholder");
                MessageBox.toast(warnInfo);
            }

            //向api发送商品搜索
            homePage.handler_api_search_tmall_products();
            homePage.handler_api_search_taobao_products();
            homePage.handler_api_search_jd_products();
            homePage.handler_api_search_pdd_products();
            homePage.handler_api_search_vip_products();
            homePage.handler_api_search_guomei_products();
            homePage.handler_api_search_suning_products();
            homePage.handler_api_search_yhd_products();
            homePage.handler_api_search_mls_products();
            homePage.handler_api_search_mgj_products();
            homePage.handler_api_search_dangdang_products();
            homePage.handler_api_search_zhe800_products();
            homePage.handler_api_search_etao_products()
        },

        //天猫商品检索
        handler_api_search_tmall_products: function () {
            var queryAddress = homePage.api_search_tmall_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_tmall_products);
        },
        //淘宝商品检索
        handler_api_search_taobao_products: function () {
            var queryAddress = homePage.api_search_taobao_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_taobao_products);
        },
        //京东商品检索
        handler_api_search_jd_products: function () {
            var queryAddress = homePage.api_search_jd_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_jd_products);
        },
        //拼多多商品检索
        handler_api_search_pdd_products: function () {
            var queryAddress = homePage.api_search_pdd_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_pdd_products);
        },
        //唯品会商品检索
        handler_api_search_vip_products: function () {
            var queryAddress = homePage.api_search_vip_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_vip_products);
        },
        //国美商品检索
        handler_api_search_guomei_products: function () {
            var queryAddress = homePage.api_search_guomei_products;
            var paras = {}; httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_guomei_products);
        },
        //苏宁商品检索
        handler_api_search_suning_products: function () {
            var queryAddress = homePage.api_search_suning_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_suning_products);
        },
        //一号店商品检索
        handler_api_search_yhd_products: function () {
            var queryAddress = homePage.api_search_yhd_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_yhd_products);
        },
        //美丽说商品检索
        handler_api_search_mls_products: function () {
            var queryAddress = homePage.api_search_mls_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_mls_products);
        },
        //蘑菇街商品检索
        handler_api_search_mgj_products: function () {
            var queryAddress = homePage.api_search_mgj_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_mgj_products);
        },
        //当当商品检索
        handler_api_search_dangdang_products: function () {
            var queryAddress = homePage.api_search_dangdang_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_dangdang_products);
        },
        //zhe800商品检索
        handler_api_search_zhe800_products: function () {
            var queryAddress = homePage.api_search_zhe800_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_zhe800_products);
        },
        //一淘商品检索
        handler_api_search_etao_products: function () {
            var queryAddress = homePage.api_search_etao_products;
            var paras = {};
            httpClient.post(queryAddress, paras, homePage.callBackHandler_api_search_etao_products);
        },

        //淘宝天猫优惠券检索
        handler_api_search_taoquan: function () {
            var queryAddress = homePage.api_search_taoquan;
            var paras = {};
            httpClient.post(api_search_taoquan, paras, homePage.callBackHandler_api_search_taoquan);
        },  


        callBackHandler_api_search_tmall_products: function (data) {
            console.log('callBackHandler_api_search_tmall_products');
        },
        callBackHandler_api_search_taobao_products: function (data) {
            console.log('callBackHandler_api_search_taobao_products');
        },
        callBackHandler_api_search_jd_products: function (data) {
            console.log('callBackHandler_api_search_jd_products');
        },
        callBackHandler_api_search_pdd_products: function (data) {
            console.log('callBackHandler_api_search_pdd_products');
        },
        callBackHandler_api_search_vip_products: function (data) {
            console.log('callBackHandler_api_search_vip_products');
        },
        callBackHandler_api_search_guomei_products: function (data) {
            console.log('callBackHandler_api_search_guomei_products');
        },
        callBackHandler_api_search_suning_products: function (data) {
            console.log('callBackHandler_api_search_suning_products');
        },
        callBackHandler_api_search_yhd_products: function (data) {
            console.log('callBackHandler_api_search_yhd_products');
        },
        callBackHandler_api_search_mls_products: function (data) {
            console.log('callBackHandler_api_search_mls_products');
        },
        callBackHandler_api_search_mgj_products: function (data) {
            console.log('callBackHandler_api_search_mgj_products');
        },
        callBackHandler_api_search_dangdang_products: function (data) {
            console.log('callBackHandler_api_search_dangdang_products');
        },
        callBackHandler_api_search_zhe800_products: function (data) {
            console.log('callBackHandler_api_search_zhe800_products');
        },
        callBackHandler_api_search_etao_products: function (data) {
            console.log('callBackHandler_api_search_etao_products');
        },
        callBackHandler_api_search_taoquan: function (data) {
            console.log('callBackHandler_api_search_taoquan');
        },
    };

    //regist to global
    ShoppingPeeker.HomePage = homePage;
    //init page object
    homePage.init();
});