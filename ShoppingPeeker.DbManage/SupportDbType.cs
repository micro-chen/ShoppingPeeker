using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPeeker.DbManage
{

    /// <summary>
    /// 支持的数据库类型
    /// </summary>
    public enum SupportDbType
    {

        Sqlserver=1,

        Mysql=2,
        /// <summary>
        /// 暂未支持 PostgreSQL
        /// </summary>
        PostgreSQL = 3,

        /// <summary>
        /// 暂未支持 Oracle
        /// </summary>
        Oracle = 4

    }
}
