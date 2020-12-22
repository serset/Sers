namespace Vit.Core.Util.ComponentModel.Model
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
