using System;
using System.Collections.Generic;
using Vit.Extensions;
using Sers.Core.Module.Api.Rpc;


namespace Sers.ApiLoader.Sers.Attribute
{
    /// <summary>
    /// 调用来源限制(内部调用 外部调用)
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
        /// 用逗号隔开的多个。例如 "Internal,Gateway"
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
                    // { "condition":{ "type":"!=","path":"caller.source"  ,  "value":"Internal"  },    "value": { "type":"_", ssError} }
                    condition = "{\"type\":\"!=\",\"path\":\"caller.source\",\"value\":\"" + values[0] + "\"}";
                }
                else
                {
                    // { "condition":{ "type":"not in","path":"caller.source"  ,  "value":["Internal"]  },    "value": { "type":"_", ssError} }
                    condition = "{\"type\":\"not in\",\"path\":\"caller.source\",\"value\":[\"" + String.Join("\",\"", values) + "\"]}";
                }
            }
        }
        #endregion


        #region callerSource

        ECallerSource _callerSource;
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
