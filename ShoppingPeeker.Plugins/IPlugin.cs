using System;

namespace ShoppingPeeker.Plugins
{
    public interface IPlugin
    {

        
        /// <summary>
        /// 插件描述清单
        /// </summary>
        PluginMeta MetaManifest { get; }

        /// <summary>
        /// 自我创建新实例
        /// </summary>
        /// <returns></returns>
        IPlugin CreateNew();

        /// <summary>
        /// 执行插件的方法
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        object Execute(string content);
    }
}
