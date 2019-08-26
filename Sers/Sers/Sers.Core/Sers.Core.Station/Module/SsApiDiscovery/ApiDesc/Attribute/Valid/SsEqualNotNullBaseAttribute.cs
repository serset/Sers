using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;


namespace Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid
{
    public class SsEqualNotNullBaseAttribute : SsValidationBaseAttribute
    {
        protected string _path;
        protected string _value;

        public SsEqualNotNullBaseAttribute(string path = null, string value=null)
        {
            this._path = path;
            this._value = value;
        }
     

        public SsError ssError { get; set; }


        public override void GetSsValidation(List<SsValidation> validations)
        {
            var ssError = this.ssError;
            if (null == ssError)
            {
                ssError = SsError.Err_NotAllowed;
            }


            JObject joValid;

            joValid = new JObject
            {
                ["type"] = SersValidMng.EValidType.Equal.EnumToString(),
                ["value"] = _value
            };
            validations.Add(new SsValidation { path = _path, ssError = ssError, ssValid = joValid });

            joValid = new JObject
            {
                ["type"] = SersValidMng.EValidType.Required.EnumToString()
            };
            validations.Add(new SsValidation { path = _path, ssError = ssError, ssValid = joValid });
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
