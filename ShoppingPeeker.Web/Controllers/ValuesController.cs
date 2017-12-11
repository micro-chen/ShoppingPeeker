using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Web.Mvc;

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
    public class ValuesController : BaseApiController
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
