using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 调用来源限制(内部调用 外部调用)
    /// </summary>
    public class SsCallerSourceAttribute : SsValidationBaseAttribute
    {
        public SsCallerSourceAttribute(ECallerSource callerSource)
        {
            this.callerSource = callerSource;
        }

        public ECallerSource callerSource;
 
        public SsError ssError { get; set; }
 
        
        public override void GetSsValidation(List<SsValidation> validations)
        {
            string path = "caller.source";

            var ssError = this.ssError;
            if (null == ssError)
            {
                ssError= SsError.Err_NotAllowed;
            }


            JObject joValid ;

            joValid = new JObject
            {
                ["type"]= SersValidMng.EValidType.Equal.EnumToString(),
                ["value"] = callerSource.EnumToString()
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
