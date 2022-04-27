using System;

namespace Vit.Core.Util.ComponentModel.Api
{
    /// <summary>
    /// 路由前缀,例如："demo/v1"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SsRoutePrefixAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
       
        public SsRoutePrefixAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
