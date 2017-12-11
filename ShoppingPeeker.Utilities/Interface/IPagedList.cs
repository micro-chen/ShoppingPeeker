
using System.Collections.Generic;

namespace ShoppingPeeker.Utilities.Interface
{
    /// <summary>
    /// Paged list interface
    /// 分页后的列表  接口
    /// 定义分页后的数据集合应有 的属性
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
