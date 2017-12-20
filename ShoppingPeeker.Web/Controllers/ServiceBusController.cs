using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage.Arguments;

using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Web.Mvc;
using ShoppingPeeker.Web.Framework.PlatformFecture.AutoMappingWord;
using ShoppingPeeker.Utilities.Logging;
using ShoppingPeeker.Web.ViewModels;
using ShoppingPeeker.Web.Framework.PlatformFecture.WebPageService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
/// <summary>
/// 示范的 Web API 地址。
/// 已经在 Setup  启动中 进行了全局路由规则限制，除非特定场景 否则不适用标注的形式 添加单个控制器
/// http://localhost:60813/api/values/get
/// routes.MapRoute(name: "default-webapi",
///template: "api/{controller}/{action}/{id?}");
/// </summary>
namespace ShoppingPeeker.Web.Controllers
{
    public class ServiceBusController : BaseApiController
    {

        /// <summary>
        /// 获取自动完成的词目
        /// </summary>
        /// <returns></returns>
        [ActionName("suggest")]
        [HttpGet]
        public SuggestionsViewModdel GetAutoMappingWords(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            IEnumerable<string> topWords = null;
            try
            {
                topWords = Single<AutoMappingService>().QueryThisKeywordMappings(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new SuggestionsViewModdel { suggestions = topWords };
        }

        /// <summary>
        /// 检索天猫的商品
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [ActionName("search_tmall_products")]
        [HttpPost]
        public BusinessViewModelContainer<SearchProductViewModel> SearchTmallProducts([FromBody]TmallFetchWebPageArgument webArgs)
        {
            BusinessViewModelContainer<SearchProductViewModel> container = new BusinessViewModelContainer<SearchProductViewModel>();

            if (null==webArgs||!webArgs.IsValid())
            {
                container.SetFalied("查询参数不是有效的查询参数！");
                return container;
            }
            try
            {
                //使用指定平台的页面检索服务 进行搜索商品
                var pageService = WebPageService.CreateNew();
                container.Data = pageService.QueryProductsByKeyWords(webArgs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return container;
        }

        /// <summary>
        /// 检索当当的商品
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [ActionName("search_dangdang_products")]
        [HttpPost]
        public BusinessViewModelContainer<SearchProductViewModel> SearchDangDangProducts([FromBody]DangdangFetchWebPageArgument webArgs)
        {
            BusinessViewModelContainer<SearchProductViewModel> container = new BusinessViewModelContainer<SearchProductViewModel>();

            if (null == webArgs || !webArgs.IsValid())
            {
                container.SetFalied("查询参数不是有效的查询参数！");
                return container;
            }

            try
            {
                //使用指定平台的页面检索服务 进行搜索商品
                var pageService = WebPageService.CreateNew();
                container.Data = pageService.QueryProductsByKeyWords(webArgs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return container;
        }


    }
}
