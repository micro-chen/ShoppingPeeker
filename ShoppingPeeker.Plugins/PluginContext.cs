using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ShoppingPeeker.Plugins
{
    /// <summary>
    /// 插件运行时上下文
    /// </summary>
    public class PluginContext
    {


        public class Logger
        {
            #region 日志相关

            /// <summary>
            /// 插件运行时使用的日志工厂
            /// </summary>
            public static ILoggerFactory LogFactory { get; set; }

            public static ILogger _Logger;
            /// <summary>
            /// 程序内置的日志记录器
            /// </summary>
            private static ILogger LoggerWriter
            {
                get
                {
                    if (null == _Logger)
                    {
                        if (null == LogFactory)
                        {
                            throw new Exception("app logfactory is null  !");
                        }
                        _Logger = LogFactory.CreateLogger("PluginContextLogger");
                    }
                    return _Logger;
                }

            }
            /// <summary>
            /// Error 级别的log
            /// </summary>
            /// <param name="errMsg"></param>
            /// <param name="title"></param>

            public static void Error(string errMsg, string title = "Error")
            {
                LoggerWriter.LogError(errMsg);
            }

            /// <summary>
            /// Error 级别的log
            /// </summary>
            /// <param name="ex"></param>
            /// <param name="title"></param>

            public static void Error(Exception ex, string title = "Error")
            {
                string errMsg = string.Concat(title, ex.ToString());
                Error(errMsg, title);
            }

            #endregion
        }


    }
}
