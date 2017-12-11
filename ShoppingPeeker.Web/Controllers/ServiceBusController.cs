using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Web.Mvc;
using ShoppingPeeker.Web.Framework.PlatformFecture.AutoMappingWord;
using ShoppingPeeker.Utilities.Logging;
using ShoppingPeeker.Web.ViewModels;

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
        
    }
}
