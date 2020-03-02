using System;

namespace Vit.Core.Util.ComponentModel.Api
{
    /// <summary>
    /// <para>可同时指定多个。                                                                                     </para>
    /// <para>若以"/"开始则为绝对路径（忽略SsStationName和SsRoutePrefix，如 /demo/v1/api/1/route/3）               </para>
    /// <para>若以"/*"结尾则代表范接口，可处理所有路由前缀相同的请求，如 1/route/4/* 会处理请求 1/route/4/a.html   </para>
    /// <para>demo "api/1/route/3"                                                                                 </para>
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
