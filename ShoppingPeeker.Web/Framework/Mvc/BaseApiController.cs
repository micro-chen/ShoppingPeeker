using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Utilities.Interface;
using ShoppingPeeker.Utilities.Ioc;


namespace ShoppingPeeker.Web.Mvc
{
    /// <summary>
    /// web api 的基础控制器
    /// 需要进行身份验证
    /// </summary>
    [AuthAttributeFilter]
    public class BaseApiController : BaseMvcController
    {
    }
}
