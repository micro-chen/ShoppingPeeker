using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using NTCPMessage.Client;
using ShoppingPeeker.Utilities;

namespace ShoppingPeeker.Web.Framework.PlatformFecture
{
    /// <summary>
    /// 蜘蛛连接负载管理
    /// </summary>
    public class WebCrawlerConnConfigManager : IDisposable
    {


        public WebCrawlerConnConfigManager()
        {
            this._Connection = GetOneConnConfig();
        }

        private WebCrawlerConnection _Connection;

        /// <summary>
        /// 蜘蛛连接对象
        /// </summary>
        public WebCrawlerConnection Connection
        {
            get
            {
                return this._Connection;
            }
        }

        /// <summary>
        /// 获取一个可用的节点端口
        /// </summary>
        private WebCrawlerConnection GetOneConnConfig()
        {
            WebCrawlerConnection slaveNode = null;
            var allConns = ConfigHelper.WebCrawlerSection.ConnectionStringCollection;
            if (null == allConns || allConns.Count <= 0)
            {
                throw new Exception("未能加载有效的 蜘蛛连接配置！");
            }


            //1 随机数;
            // 2轮询；（todo）
            // 3压力综合 

            //随机数
            //var randomObj = new Random(DateTime.Now.Millisecond);
            //int pos1 = randomObj.Next(0, _slaveNodes.Count - 1);
            //int pos2 = randomObj.Next(0, _slaveNodes.Count - 1);
            //int pos_final = Math.Max(pos1, pos2);
            //slaveNode = _slaveNodes.ElementAt(pos_final);
            #region 压力综合 

            slaveNode =  allConns.OrderBy(x => x._SysCurrentUseCount).First();


            //设置引用计数+1
            slaveNode._SysCurrentUseCount += 1;

            #endregion

            return slaveNode;

        }

        #region Dispose


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;


        /// <summary>
        ///  Dispose 执行
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 清理托管资源
                    if (this._Connection._SysCurrentUseCount>0)
                    {
                        this._Connection._SysCurrentUseCount -= 1;
                    }
                    this._Connection = null;
                }
                // 清理非托管资源
                //让类型知道自己已经被释放
                disposed = true;
            }
        }

        #endregion
    }
}
