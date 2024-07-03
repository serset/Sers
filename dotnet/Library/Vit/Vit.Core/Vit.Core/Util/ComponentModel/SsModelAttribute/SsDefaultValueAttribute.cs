namespace Vit.Core.Util.ComponentModel.Model
{
    /// <summary>
    /// default value
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
