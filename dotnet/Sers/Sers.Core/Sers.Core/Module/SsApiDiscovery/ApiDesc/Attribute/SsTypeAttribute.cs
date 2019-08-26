namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute
{
    /// <summary>
    /// 类型
    /// </summary>
    public class SsTypeAttribute : System.Attribute
    {
        public string Value { get; set; }
        public EValueType? Type { get; set; }

        public SsTypeAttribute(string Value = null)
        {
            this.Value = Value;
        }

        public SsTypeAttribute(EValueType Type)
        {
            this.Type = Type;
        }
    }
}
