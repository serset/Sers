namespace Vit.Core.Util.ComponentModel.Model
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
