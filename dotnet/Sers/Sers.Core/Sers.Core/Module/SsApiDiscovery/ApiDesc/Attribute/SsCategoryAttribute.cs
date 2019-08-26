using System;

namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute
{
    /// <summary>
    /// ApiStation1/path2/api3
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SsCategoryAttribute : System.Attribute
    {
        public string Value { get; set; }
       
        public SsCategoryAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
