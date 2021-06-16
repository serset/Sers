namespace Vit.Core.Util.ComponentModel.Model
{
    /// <summary>
    /// 参考值
    /// </summary>
    public class SsExampleAttribute : System.Attribute
    {
        public object Value { get; set; }
       
        public SsExampleAttribute(object Value = null)
        {
            this.Value = Value;
        }
    }
}
