namespace Vit.Core.Util.ComponentModel.Model
{
    /// <summary>
    /// example
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
