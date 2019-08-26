namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute
{
    /// <summary>
    /// 默认值
    /// </summary>
    public class SsDefaultValueAttribute : System.Attribute
    {
        public object Value { get; set; }
       
        public SsDefaultValueAttribute(object Value = null)
        {
            this.Value = Value;
        }
    }
}
