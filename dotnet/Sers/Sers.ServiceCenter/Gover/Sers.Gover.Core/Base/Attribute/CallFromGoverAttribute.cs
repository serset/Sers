using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 调用来源限制(只可Gover调用)
    /// </summary>
    public class CallFromGoverAttribute : SsCallerSourceAttribute
    {
        public CallFromGoverAttribute()
        {
            callerSourceString = "GoverGateway";
        }
    }
}
