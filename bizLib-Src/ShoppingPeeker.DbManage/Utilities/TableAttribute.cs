using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingPeeker.DbManage
{
    public class TableAttribute:Attribute
    {
        //
        // 摘要:
        //     Gets or sets the name of the table or view.
        //
        // 返回结果:
        //     By default, the value is the same as the name of the class.
        public string Name { get; set; }
    }
}
