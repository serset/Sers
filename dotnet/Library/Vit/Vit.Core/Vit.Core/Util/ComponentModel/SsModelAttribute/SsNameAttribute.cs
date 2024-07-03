namespace Vit.Core.Util.ComponentModel.Model
{
    /// <summary>
    /// name
    /// </summary> 
    public class SsNameAttribute : System.Attribute
    {
        /// <summary>
        /// name
        /// </summary>
        public string Value { get; set; }

        public SsNameAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
