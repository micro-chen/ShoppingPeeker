using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingPeeker.Web.Mvc
{
    /// <summary>
    /// 业务实体-视图模型容器
    /// </summary>
    public class ViewModelContainer<T>
    {

        /// <summary>
        /// 数据承载
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PagerInfomation { get; set; }


        /// <summary>
        /// 判断数据是否是空白数据类型
        /// </summary>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public  bool IsNoDataViewModel()
        {
         
            if (null == this || this.Data == null)
            {
                return true;
            }

            try
            {
                //判断是否是集合类型的数据
                var dataType = this.Data.GetType();

                if (typeof(IEnumerable).IsAssignableFrom(dataType))
                {
                    //集合类型
                    if ((this.Data as IEnumerable<object>).Count() <= 0)
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }
    }
}
