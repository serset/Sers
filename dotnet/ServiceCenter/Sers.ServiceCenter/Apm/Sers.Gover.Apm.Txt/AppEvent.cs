using Newtonsoft.Json.Linq;
using Sers.Core.Module.App.AppEvent;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Gover.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Extensions.IEnumerable;

namespace Sers.Gover.Apm.Txt
{

    public class AppEvent : IAppEvent
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        Action<Object, Vit.Core.Util.Pipelines.ByteData> ApiScopeEvent(RpcContextData rpcData, ApiMessage apiRequestMessage)
        {
            //记录请求数据

            var beginTime = DateTime.Now;       


            return (s, apiReplyMessage) => {

                var endTime = DateTime.Now;


                #region method getTagValue

                string requestRpc_oriString = null;
                JObject requestRpc_json = null;

                string requestData_oriString = null;
                JObject requestData_json = null;

                ApiMessage apiResponseMessage = null;

                string responseRpc_oriString = null;
                JObject responseRpc_json = null;

                string responseData_oriString = null;
                JObject responseData_json = null;

                string GetTagValue(string valueString)
                {
                    if (string.IsNullOrEmpty(valueString)) return null;
                    if (!valueString.StartsWith("{{") || !valueString.EndsWith("}}")) return valueString;

                    try
                    {

                        valueString = valueString.Substring(2, valueString.Length - 4);

                        string dataType;
                        string path;

                        var splitIndex = valueString.IndexOf('.');
                        if (splitIndex < 0)
                        {
                            dataType = valueString;
                            path = "";
                        }
                        else
                        {
                            dataType = valueString.Substring(0, splitIndex);
                            path = valueString.Substring(splitIndex + 1);
                        }

                        switch (dataType)
                        {
                            case "requestRpc":
                               
                                if (requestRpc_oriString == null)
                                {
                                    requestRpc_oriString = apiRequestMessage.rpcContextData_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return requestRpc_oriString;
                                }
                                if (requestRpc_json == null)
                                {
                                    requestRpc_json = requestRpc_oriString.Deserialize<JObject>();
                                }
                                return requestRpc_json?.SelectToken(path).ConvertToString();

                            case "requestData":
                                if (requestData_oriString == null)
                                {
                                    requestData_oriString = apiRequestMessage.value_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return requestData_oriString;
                                }
                                if (requestData_json == null)
                                {
                                    requestData_json = requestData_oriString.Deserialize<JObject>();
                                }
                                return requestData_json?.SelectToken(path).ConvertToString();

                            case "responseRpc":
                                if (apiResponseMessage == null)
                                {
                                    apiResponseMessage = new ApiMessage();
                                    apiResponseMessage.Unpack(apiReplyMessage.ToArraySegment());
                                }
                                if (responseRpc_oriString == null)
                                {
                                    responseRpc_oriString = apiResponseMessage.rpcContextData_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return responseRpc_oriString;
                                }
                                if (responseRpc_json == null)
                                {
                                    responseRpc_json = responseRpc_oriString.Deserialize<JObject>();
                                }
                                return responseRpc_json?.SelectToken(path).ConvertToString();

                            case "responseData":
                                if (apiResponseMessage == null)
                                {
                                    apiResponseMessage = new ApiMessage();
                                    apiResponseMessage.Unpack(apiReplyMessage.ToArraySegment());
                                }
                                if (responseData_oriString == null)
                                {
                                    responseData_oriString = apiResponseMessage.value_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return responseData_oriString;
                                }
                                if (responseData_json == null)
                                {
                                    responseData_json = responseData_oriString.Deserialize<JObject>();
                                }
                                return responseData_json?.SelectToken(path).ConvertToString();
                        }
                    }
                    catch
                    {
                    }
                    return null;
                }
                #endregion



                StringBuilder msg = new StringBuilder();

                msg.Append(Environment.NewLine).Append("┍------------ ---------┑");

                msg.Append(Environment.NewLine).Append("--BeginTime:").Append(beginTime.ToString("[HH:mm:ss.ffffff]"));
                msg.Append(Environment.NewLine).Append("--EndTime  :").Append(endTime.ToString("[HH:mm:ss.ffffff]"));
                msg.Append(Environment.NewLine).Append("--duration :").Append((endTime - beginTime).TotalMilliseconds).Append(" ms");

         
                config.tags?.ForEach(item =>
                {
                    //try
                    //{
                        var key = GetTagValue(item.Key);
                        var value = GetTagValue(item.Value);
                        if (key != null)
                        {
                            msg.Append(Environment.NewLine).Append("--" + key + ":").Append(value);
                        }
                    //}
                    //catch
                    //{
                    //}
                });                
                 

                msg.Append(Environment.NewLine).Append("┕------------ ---------┙").Append(Environment.NewLine);

                Logger.log.LogTxt(Level.ApiTrace, msg.ToString()); 
            };
        }


        public void InitEvent(JObject arg)
        {
            config = arg.Deserialize<Config>();

            Logger.Info("[Sers.Gover.Apm.Txt]初始化中...  ");
        }


        Config config;

        public void BeforeStart()
        {
            if (config == null) return;

            GoverApiCenterService.Instance.AddApiScopeEvent(ApiScopeEvent);

            Logger.Info("[Sers.Gover.Apm.Txt]初始化成功");
        }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void OnStart()
        { 
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void AfterStart()
        {         
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void BeforeStop()
        {
          
           
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void AfterStop()
        {
            
        }
    }


    #region Config Model
    class Config
    {  
        public IDictionary<string, string> tags;
    }
    #endregion

}
