namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute
{
    /// <summary>
    /// 描述
    /// </summary>
    public class SsDescriptionAttribute : System.Attribute
    {
        public string Value { get; set; }
       
        public SsDescriptionAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
