using System;
using System.Collections.Generic;

namespace ShoppingPeeker.Web.ViewModels
{
    /// <summary>
    /// 自动完成结果模型
    /// </summary>
    public class SuggestionsViewModdel
    {
        public IEnumerable<string> suggestions { get; set; }

    }
}
