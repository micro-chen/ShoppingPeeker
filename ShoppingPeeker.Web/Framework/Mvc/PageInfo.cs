using System;
using System.Collections.Generic;

namespace ShoppingPeeker.Web.Mvc
{

    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageInfo
    {

        public PageInfo()
        {
            this.pageFootNum = 7;//默认显示7个分页按钮
        }



        /// <summary>
        /// 页大小
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 总数目
        /// </summary>
        public int TotalElements { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 当前页索引（页码-1）
        /// </summary>
        private int pageIndex;



        /// <summary>
        /// 当前页索引
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this.pageIndex;
            }
            set
            {
                this.pageIndex = value;
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber
        {
            get
            {
                return this.pageIndex + 1;
            }
        }


        /// <summary>
        /// 上一页 页码
        /// </summary>
        public int PreviousPageNumber
        {
            get
            {
                var num = this.PageNumber - 1;
                if (num <= 0)
                {
                    num = 1;
                }
                return num;
            }
        }

        /// <summary>
        /// 下一页 页码
        /// </summary>
        public int NextPageNumber
        {
            get
            {
                var num = this.PageNumber + 1;
                if (num >= TotalPages)
                {
                    num = TotalPages;
                }
                return num;
            }
        }


        private int pageFootNum;
        /// <summary>
        /// 分页页脚最大数量，建议是奇数
        /// </summary>
        public int PageFootNum
        {
            get
            {
                return pageFootNum;
            }
            set { pageFootNum = value; }
        }
        private List<int> pageFootInfo;
        /// <summary>
        /// 分页页脚集合
        /// </summary>
        public List<int> PageFootInfo
        {
            get
            {
                if (null==pageFootInfo)
                {
                    pageFootInfo = new List<int>();
                    int increment = (PageFootNum - 1) / 2;//页脚增量
                    int first = 1;
                    int last = TotalPages;
                    if (TotalPages > PageFootNum)
                    {
                        first = (pageIndex - increment) <= 0 ? 1 : (pageIndex - increment);//计算出开始数字
                        last = (pageIndex + increment) >= TotalPages ? TotalPages : (pageIndex + increment);//计算出结束数字
                        if (TotalPages > PageFootNum)
                        {
                            if (first == 1)
                            {
                                last = PageFootNum;
                            }
                            else if (last == TotalPages)
                            {
                                first = TotalPages - 4;
                            }
                        }
                    }
                    for (int i = first; i <= last; i++)//填充页脚数字集合
                    {
                        pageFootInfo.Add(i);
                    }
                }
                return pageFootInfo;
            }
            set { pageFootInfo = value; }
        }

    }
}
