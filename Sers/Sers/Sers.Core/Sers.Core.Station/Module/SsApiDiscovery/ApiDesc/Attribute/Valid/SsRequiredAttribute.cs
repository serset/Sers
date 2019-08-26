using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    /// <summary>
    /// 非空限制
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class SsRequiredAttribute :  SsValidationBaseAttribute
    {

        public SsRequiredAttribute()
        {
        }
        public SsRequiredAttribute(string path)
        {
            this.path = path;
        }

        public string path="";

        public SsError ssError { get; set; }


        private const string ssValid = "{\"type\":\"Required\"}";

        public override void GetSsValidation(List<SsValidation> validations)
        {
            validations.Add(new SsValidation { path = path, ssError = ssError, ssValid = JObject.Parse(ssValid) });
        }


        /// <summary>
        /// 校验不通过时的提示消息
        /// </summary>
        [JsonIgnore]
        public string errorMessage
        {
            get => ssError?.errorMessage;
            set
            {
                if (null == ssError) ssError = new SsError();
                ssError.errorMessage = value;
            }
        }


    }
}
