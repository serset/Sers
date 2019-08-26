using Sers.Core.Module.ApiDesc.Attribute;


namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    public class SsNotEqualAttribute : SsCmpAttribute
    {
        public SsNotEqualAttribute()
        {
            type = "!=";
        }
    }
}
