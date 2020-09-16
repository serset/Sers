using System;

namespace Vit.Core.Util.ComponentModel.Model
{
    /// <summary>
    /// 指定类型
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class SsTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Type Value { get; set; }
       
        public SsTypeAttribute(Type Value = null)
        {
            this.Value = Value;
        }
    }
}
