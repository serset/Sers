using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 最小值限制
    /// </summary>
    public class SsMinValueAttribute : SsValidationAttribute
    {
        public object Value { get; set; }
    

        public SsMinValueAttribute(object Value = null, string ErrorMessage = null)
        {
            this.Value = Value;
            this.errorMessage = ErrorMessage;
        }
    }
}
