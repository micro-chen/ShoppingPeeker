using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingPeeker.Utilities;
using ShoppingPeeker.Utilities.Http;

namespace ShoppingPeeker.Web.Mvc
{

    public class BaseResult<T>
    {

        public BaseResult()
        {
        }

        /// <summary>
        /// 结果状态 
        /// </summary>
        public CodeStatusTable Status { get; set; }
        /// 结果信息
        /// </summary>
        public T Msg { get; set; }



        /// <summary>
        /// 标识为默认的正确结果
        /// 虚方法  可被子类重写
        /// </summary>
        public virtual void BeSuceess()
        {
            this.Status = CodeStatusTable.Success;
        }

      
    }
}
