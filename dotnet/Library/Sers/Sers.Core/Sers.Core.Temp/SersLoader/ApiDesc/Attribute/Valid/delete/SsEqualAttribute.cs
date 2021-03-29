using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Valid.Sers1;

using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 值限制
    /// </summary>
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public class SsEqualAttribute : SsValidationBaseAttribute
    {
        
        [JsonProperty]
        public object value;

        [JsonProperty]
        public string path = "";

        [JsonProperty]
        public SsError ssError { get; set; }



        public SsEqualAttribute()
        {
            ssValid = "{\"type\":\"Equal\"}";
        }
        public SsEqualAttribute(string path):this()
        {
            this.path = path;
        }

       


        protected string ssValid;
        
        public override void GetSsValidation(List<SsValidation> validations)
        {
            var joValid = JObject.Parse(ssValid);
            joValid["value"] =  value.Serialize();

            validations.Add(new SsValidation { path = path, ssError = ssError, ssValid = joValid });
        }


        /// <summary>
        /// 校验不通过时的提示消息
        /// </summary>
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
