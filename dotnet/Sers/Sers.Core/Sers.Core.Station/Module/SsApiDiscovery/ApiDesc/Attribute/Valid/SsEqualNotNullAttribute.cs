namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    public class SsEqualNotNullAttribute : SsEqualNotNullBaseAttribute
    {

        public string path
        {
            get => _path;
            set => _path = value;
        }

        public string value
        {
            get => _value;
            set => _value = value;
        }
 
        public SsEqualNotNullAttribute(string path=null, string value=null) : base(path, value)
        {
        }
     

     

    }
}
