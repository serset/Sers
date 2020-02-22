using System;

namespace Vit.Core.Util.ComponentModel.Api
{
    /// <summary>
    /// demo "fold1/fold2"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SsRouteAttribute : System.Attribute
    {
        /// <summary>
        /// demo "fold1/fold2"
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// demo: GET、POST、DELETE、PUT ......
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// demo "fold1/fold2"
        /// </summary>
        /// <param name="Value"></param>
        public SsRouteAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
