using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 正则限制
    /// </summary>
    public class SsRegexAttribute : SsCmpAttribute
    {
        public SsRegexAttribute()
        {
            type = "Regex";
        }
    }
}
