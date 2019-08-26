using System;
using System.Collections.Generic;
using Sers.Core.Module.SsApiDiscovery.SersValid;

namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class SsValidationBaseAttribute : System.Attribute
    {
        public abstract void GetSsValidation(List<SsValidation> validations); 

       
    }
}
