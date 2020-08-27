using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 最大值限制
    /// </summary>
    public class SsMaxValueAttribute : SsValidationAttribute
    {
        public object Value { get; set; }
 

        public SsMaxValueAttribute(object Value = null, string ErrorMessage = null)
        {
            this.Value = Value;
            this.errorMessage = ErrorMessage;
        }
    }
}
