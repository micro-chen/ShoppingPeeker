using System;
using System.Collections.Generic;

using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;

namespace ShoppingPeeker.Web.ViewModels
{
    /// <summary>
    /// 查询商品视图模型
    /// </summary>
    public  class SearchProductViewModel
    {
        /// <summary>
        /// 本次使用的搜索关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 是否需要处理tags
        /// </summary>
        public bool IsNeedResolveHeaderTags { get; set; }

        /// <summary>
        /// 品牌集合
        /// </summary>
        public List<BrandTag> Brands { get; set; }

        /// <summary>
        /// 关联的tag集合
        /// </summary>
        public List<KeyWordTagGroup> Tags { get; set; }

        /// <summary>
        /// 搜索结果商品列表
        /// </summary>
        public ProductBaseCollection Products { get; set; }


    }
}
