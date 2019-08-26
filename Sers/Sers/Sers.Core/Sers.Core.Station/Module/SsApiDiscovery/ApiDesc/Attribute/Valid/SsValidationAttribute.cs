using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class SsValidationAttribute : SsValidationBaseAttribute
    {      

        public string path;

        public SsError ssError { get; set; }

        public JObject ssValid { get; set; }

        

      


        [JsonIgnore]
        public string valid
        {
            set
            {
                ssValid = JObject.Parse(value);
            }
            get => ssValid?.ToString();
             
        }

        public override void GetSsValidation(List<SsValidation> validations)
        {
            validations.Add(new SsValidation { path = path, ssError = ssError, ssValid = ssValid });
        }


        /// <summary>
        /// 校验不通过时的提示消息
        /// </summary>
        [JsonIgnore]
        public string errorMessage {
            get
            {
                return ssError?.errorMessage;
            }
            set
            {
                if (null == ssError) ssError = new SsError();
                ssError.errorMessage=value;
            }
        }

       
    }
}
