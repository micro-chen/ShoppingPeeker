using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Web.Mvc;
using NTCPMessage.Client;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using System.Net;
using System.Text;

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

            var endPoint = new IPEndPoint(IPAddress.Parse(WorkContext.ShoppingWebCrawlerAddress), WorkContext.ShoppingWebCrawlerPort);
            SingleConnectionCable client = new SingleConnectionCable(endPoint, 7);
            //ISerialize<SoapMessage> iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>();
            //ISerialize<DataContainer> iReturnDataSerializer = new NTCPMessage.Serialize.JsonSerializer<DataContainer>();
            try
            {
                client.Connect(5000);
                //发送ping
                var buffer = Encoding.UTF8.GetBytes("ping");
                var resultBytes = client.SyncSend((UInt32)MessageType.None, buffer);

                var str = Encoding.UTF8.GetString(resultBytes);

                resut = string.Concat("time :", DateTime.Now.ToString(), "; tcp server response: ", str);
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
