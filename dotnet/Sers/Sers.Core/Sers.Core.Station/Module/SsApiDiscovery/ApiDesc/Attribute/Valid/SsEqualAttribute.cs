using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Extensions;


namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    public class SsEqualAttribute : SsCmpAttribute
    {
        public SsEqualAttribute()
        {
            type = "==";
        }
    }
}
