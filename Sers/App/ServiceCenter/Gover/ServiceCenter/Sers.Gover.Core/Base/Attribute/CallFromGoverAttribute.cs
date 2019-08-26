using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 调用来源限制(只可Gover调用)
    /// </summary>
    public class CallFromGoverAttribute : SsValidationBaseAttribute
    {
          
 
        public SsError ssError { get; set; }
 
        
        public override void GetSsValidation(List<SsValidation> validations)
        {
            string path = "caller.source";

            var ssError = this.ssError;
            if (null == ssError)
            {
                ssError=new SsError{errorMessage = "无权限。只可通过GoverGateway调用" };
            }


            JObject joValid ;

            joValid = new JObject
            {
                ["type"]= SersValidMng.EValidType.Equal.EnumToString(),
                ["value"] = "GoverGateway"
            };
            validations.Add(new SsValidation { path = path, ssError = ssError, ssValid = joValid });

            joValid = new JObject
            {
                ["type"] = SersValidMng.EValidType.Required.EnumToString()
            };
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
