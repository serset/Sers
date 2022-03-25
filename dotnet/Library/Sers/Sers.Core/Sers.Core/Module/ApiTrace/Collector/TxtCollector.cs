using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;
using System.Collections.Generic;
using System.Text;
using Vit.Extensions;
using Vit.Core.Module.Log;

namespace Sers.Core.Module.ApiTrace.Collector
{
    public class TxtCollector : IApiTraceCollector
    {

        IDictionary<string, string> tagsTempleate;
        public void Init(JObject arg)
        {
            Logger.Info("[ApiTrace.TxtCollector]初始化中");

            tagsTempleate = arg?["tags"]?.Deserialize<IDictionary<string, string>>();
        }

        public void AppBeforeStart()
        {
            Logger.Info("[ApiTrace.TxtCollector]初始化成功");
        }

        public void AppBeforeStop()
        {

        }

        Vit.Core.Module.Log.LogCollector.TxtCollector logCollector = new Vit.Core.Module.Log.LogCollector.TxtCollector();

        public object TraceStart(RpcContextData rpcData)
        {
            return DateTime.Now;
        }
        public void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage)
        {
            var beginTime = (DateTime)traceData;

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
                                apiResponseMessage = GetApiReplyMessage();
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
                                apiResponseMessage = GetApiReplyMessage();
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



            StringBuilder msg = new StringBuilder(1024 * 4);

            msg.Append(Environment.NewLine).Append("┍------------ ---------┑");

            msg.Append(Environment.NewLine).Append("--BeginTime:").Append(beginTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--EndTime  :").Append(endTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--duration :").Append((endTime - beginTime).TotalMilliseconds).Append(" ms");


            tagsTempleate?.IEnumerable_ForEach(item =>
            {
                var key = GetTagValue(item.Key);
                var value = GetTagValue(item.Value);
                if (key != null)
                {
                    msg.Append(Environment.NewLine).Append("--" + key + ":").Append(value);
                }
            });


            msg.Append(Environment.NewLine).Append("┕------------ ---------┙").Append(Environment.NewLine);

            logCollector.Write(Level.ApiTrace, msg.ToString());
        }

    }
}
