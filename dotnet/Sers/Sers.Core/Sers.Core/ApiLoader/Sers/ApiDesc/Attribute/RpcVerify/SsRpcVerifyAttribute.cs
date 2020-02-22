using System;
using Newtonsoft.Json.Linq;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;


namespace Sers.ApiLoader.Sers.Attribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public  class SsRpcVerifyAttribute : SsRpcVerifyBaseAttribute
    {
        //{"condition":{"type":"!=","path":"caller.source"  ,  "value":"Internal"  },    "value": {"type":"_", ssError}  } 


      


        public override void GetApiRpcVerify(JArray rpcVerifySwitchBody)
        {
            if (string.IsNullOrEmpty(condition)) return;

            var item = new JObject();
            rpcVerifySwitchBody.Add(item);

            //ssError
            var error = ssError ?? SsError.Err_NotAllowed;
            item["value"] = error.ConvertBySerialize<JObject>();
            item["value"]["type"] = "_";

            //condition
            item["condition"]=JObject.Parse(condition);

        }


        /// <summary>
        /// 例如：{"condition":{"type":"!=","path":"caller.source"  ,  "value":"Internal"  },    "value": {"type":"_", ssError}  } 
        /// </summary>
        public virtual string condition { get; set; }


        #region ssError


        SsError ssError;


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
 
        public int? errorCode
        {
            get => ssError?.errorCode;
            set
            {
                if (null == ssError) ssError = new SsError();
                ssError.errorCode = value;
            }
        }
        #endregion

      

    }
}
