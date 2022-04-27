using System;

namespace Vit.Core.Util.ComponentModel.Api
{
    /// <summary>
    /// 站点名称。例如："AuthCenter"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SsStationNameAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
       
        public SsStationNameAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
