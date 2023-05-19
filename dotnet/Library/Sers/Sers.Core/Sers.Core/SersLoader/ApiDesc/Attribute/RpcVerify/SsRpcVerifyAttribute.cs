using System;

using Newtonsoft.Json.Linq;

using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions.Json_Extensions;

namespace Sers.SersLoader.ApiDesc.Attribute.RpcVerify
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public  class SsRpcVerifyAttribute : SsRpcVerifyBaseAttribute
    {
        //{"condition":{"type":"!=","path":"caller.source"  ,  "value":"Internal"  },    "value": {"type":"_", ssError}  } 

      


        public override void GetApiRpcVerify(JArray rpcVerifySwitchBody)
        {            
            //(x.1)
            if (string.IsNullOrEmpty(condition)) return;

            //(x.2)
            var item = new JObject();
            rpcVerifySwitchBody.Add(item);

            //(x.3)ssError
            var errorValue = (ssError ?? SsError.Err_NotAllowed).ConvertBySerialize<JObject>();
            errorValue["type"] = "_";
            item["value"] = errorValue;

            #region (x.4)condition
          
            //(x.x.1)原始条件
            var joCon = JObject.Parse(condition);
            if (verifiedWhenNull) 
            {
                joCon["resultWhenNull"] = true;
            }



            //(x.x.2)取反
            // {"type":"Not",  "value":SsExp    }
            item["condition"] = new JObject
            {
                ["type"]="Not",
                ["value"] = joCon
            };
            #endregion




        }


        /// <summary>
        /// 当出现空值时，是否通过验证（默认不通过，false）
        /// </summary>
        public bool verifiedWhenNull { get; set; } = false;



        /// <summary>
        /// 例如：{"type":"!=","path":"caller.source"  ,  "value":"Internal"  }
        /// </summary>
        public virtual string condition { get; set; }


        #region ssError


        SsError ssError;


        /// <summary>
        /// 校验不通过时的提示消息，若不指定则使用默认提示消息
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

        /// <summary>
        /// 校验不通过时的errorCode, 如 1000。可不指定
        /// </summary>
        public int errorCode
        {
            get => ssError?.errorCode??0;
            set
            {
                if (null == ssError) ssError = new SsError();
                ssError.errorCode = value;
            }
        }
        #endregion

      

    }
}
