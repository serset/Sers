using System;
using System.Collections.Generic;
using Sers.Core.Module.Api.Rpc;
using Sers.SersLoader.ApiDesc.Attribute.RpcVerify;
using Vit.Extensions;

namespace Sers.SersLoader.ApiDesc.Attribute.Valid
{
    /// <summary>
    /// 调用来源限制(内部调用 外部调用) demo:
    /// [SsCallerSource(ECallerSource.Internal)]
    /// [SsCallerSource(ECallerSource.OutSide, errorMessage = "只可外部调用")]
    /// [SsCallerSource(callerSourceString = "Internal,OutSide")]
    /// </summary>
    public class SsCallerSourceAttribute : SsRpcVerifyAttribute
    {

        public SsCallerSourceAttribute()
        {
        }


        public SsCallerSourceAttribute(ECallerSource  callerSource)
        {      
                this.callerSource = callerSource;
        }
      

        #region callerSourceString


        string _callerSourceString;
        /// <summary>
        /// 用逗号隔开的多个。例如 "Internal,OutSide"
        /// </summary>
        public string callerSourceString
        {
            get => _callerSourceString;
            set
            {
                _callerSourceString = value;

                //(x.1) get values
                var values = _callerSourceString.Split(',');


                //(x.2) build condition
                if (values.Length == 0)
                {
                    condition = null;
                }
                else if (values.Length == 1)
                {
                    // { "condition":{ "type":"==","path":"caller.source"  ,  "value":"Internal"  }  }
                    condition = "{\"type\":\"==\",\"path\":\"caller.source\",\"value\":\"" + values[0] + "\"}";
                }
                else
                {
                    // { "condition":{ "type":"in","path":"caller.source"  ,  "value":["Internal"]  }  }
                    condition = "{\"type\":\"in\",\"path\":\"caller.source\",\"value\":[\"" + String.Join("\",\"", values) + "\"]}";
                }
            }
        }
        #endregion


        #region callerSource

        ECallerSource _callerSource;
        /// <summary>
        /// 可多个。例如：  ECallerSource.Internal| ECallerSource.OutSide
        /// </summary>
        public ECallerSource callerSource
        {
            get => _callerSource;
            set
            {
                _callerSource = value;

                //(x.1) get values
                List<string> values = new List<string>();
                foreach (ECallerSource item in Enum.GetValues(typeof(ECallerSource)))
                {
                    if ((_callerSource & item) != 0)
                        values.Add(item.EnumToString());
                }

                //(x.2)
                callerSourceString = String.Join(",", values);                
            }
        }

        #endregion

    }
}
