using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
 
namespace ShoppingPeeker.Web.Framework.PlatformFecture.Resolvers
{
    public class ResolverFactory
    {
        /// <summary>
        /// 获取指定平台的商品搜索解析器
        /// 需要以插件的形式加载解析器
        /// 解析内容时刻可能不断的调整，但是站点不能一直编译发布，不能因为发布某个插件就停止站点
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static ISearchProductResolver GetSearchProductResolver(SupportPlatformEnum platform)
        {
            ISearchProductResolver resolver = null;
        
            switch (platform)
            {
                case SupportPlatformEnum.Tmall:
                    resolver= new TmallSearchProductResolver();
                    break;
                case SupportPlatformEnum.Taobao:
                    resolver = new TaobaoSearchProductResolver();

                    break;
                case SupportPlatformEnum.Jingdong:
                    resolver = new JingdongSearchProductResolver();

                    break;
                case SupportPlatformEnum.Pdd:
                    resolver = new PddSearchProductResolver();

                    break;
                case SupportPlatformEnum.Vip:
                    resolver = new VipSearchProductResolver();

                    break;
                case SupportPlatformEnum.Guomei:
                    resolver = new GuomeiSearchProductResolver();
                    break;
                case SupportPlatformEnum.Suning:
                    resolver = new SuningSearchProductResolver();

                    break;
                case SupportPlatformEnum.Dangdang:
                    resolver = new DangdangSearchProductResolver();

                    break;
                case SupportPlatformEnum.Yhd:
                    resolver = new YhdSearchProductResolver();

                    break;
                case SupportPlatformEnum.Meilishuo:
                    resolver = new MeilishuoSearchProductResolver();

                    break;
                case SupportPlatformEnum.Mogujie:
                    resolver = new MogujieSearchProductResolver();

                    break;
                case SupportPlatformEnum.Zhe800:
                    resolver = new Zhe800SearchProductResolver();

                    break;
                case SupportPlatformEnum.ETao:
                    resolver = new ETaoSearchProductResolver();

                    break;
                default:
                    throw new Exception(string.Format("未能找到指定平台的商品解析器！不支持该平台：{0}",platform.ToString()));
                    
            }

            return resolver;
        }
    }
}
