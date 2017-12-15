using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Web.Mvc;
using NTCPMessage;
using NTCPMessage.Client;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using System.Net;
using System.Text;
using ShoppingPeeker.Utilities;


/// <summary>
/// 示范的 Web API 地址。
/// 已经在 Setup  启动中 进行了全局路由规则限制，除非特定场景 否则不适用标注的形式 添加单个控制器
/// /api/values/get
/// routes.MapRoute(name: "default-webapi",
///template: "api/{controller}/{action}/{id?}");
/// </summary>
namespace ShoppingPeeker.Web.Controllers
{
    public class ValuesController : BaseApiController
    {
        // GET: api/values/get
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        public string TestTcp()
        {
            var resut = string.Empty;
            try
            {

                var connStr = ConfigHelper.ShoppingWebCrawlerSection.ConnectionStringCollection.First();
                using (var conn = new SoapTcpConnection(connStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    //发送ping

                    var str = conn.SendString(CommandConstants.CMD_Ping);
                    resut = string.Concat("time :", DateTime.Now.ToString(), "; tcp server response: ", str);
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            //client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            //client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            //client.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            return resut;
        }

    }
}
