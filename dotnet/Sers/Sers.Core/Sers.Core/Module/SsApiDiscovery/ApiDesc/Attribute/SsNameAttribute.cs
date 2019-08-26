namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute
{
    /// <summary>
    /// 名称
    /// </summary> 
    public class SsNameAttribute : System.Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Value { get; set; }
       
        public SsNameAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
