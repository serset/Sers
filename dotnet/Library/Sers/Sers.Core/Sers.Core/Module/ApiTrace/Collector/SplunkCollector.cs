using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;
using System.Collections.Generic;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Vit.Core.Module.Log.LogCollector.Splunk.Client;

namespace Sers.Core.Module.ApiTrace.Collector
{
    public class SplunkCollector : IApiTraceCollector
    {
        // Splunk ApiTrace Record Format:
        /* 
        {
            "time": 1426279439.123,

            "index": "dev",

            "host": "localhost",
            "source": "random-data-generator",
            "sourcetype": "my_sample_data",

            "event": { 
                "level": "ApiTrace",

                "appInfo": { //custome object
                    "namespace": "mc.sers.cloud",
                    "appName": "mc",
                    "moduleName": "sers"
                    //,"...": {}
                },

                "beginTime":"2022-03-26 02:52:00.123456",
                "endTime":"2022-03-26 02:52:04.123456",
                "duration":"4000.0",

                //extTags
            }
        }
        */



        SplunkClient client;
        SplunkRecord hostInfo;
        JObject appInfo;

        IDictionary<string, string> tagsTemplate;

        public void Init(JObject arg)
        {
            Logger.Info("[ApiTrace.SplunkCollector]初始化中");
            client = arg["server"].Deserialize<SplunkClient>();

            hostInfo = arg?["hostInfo"]?.Deserialize<SplunkRecord>();
            appInfo = arg?["appInfo"]?.Deserialize<JObject>();
            tagsTemplate = arg?["tags"]?.Deserialize<IDictionary<string, string>>();

            client?.Init();
        }

        public void AppBeforeStart()
        {
            Logger.Info("[ApiTrace.SplunkCollector]初始化成功");
        }

        public void AppBeforeStop()
        {

        }

        public object TraceStart(RpcContextData rpcData)
        {
            return DateTime.Now;
        }

        public void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage)
        {
            JObject eventData = BuildEventData(traceData, rpcData, apiRequestMessage, GetApiReplyMessage, tagsTemplate);
            if (appInfo != null) eventData["appInfo"] = appInfo;

            var record = new SplunkRecord
            {
                Time = DateTime.UtcNow,
                index = hostInfo?.index,

                host = hostInfo?.host ?? Environment.MachineName,
                source = hostInfo?.source,
                sourcetype = hostInfo?.sourcetype,

                @event = eventData
            };
            client.SendAsync(record);
        }



        #region BuildEventData    
        public static JObject BuildEventData(object beginTime, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage, IDictionary<string, string> tagsTemplate)
        {

            JObject eventData = new JObject();
            eventData["level"] = "ApiTrace";

            if (beginTime is DateTime _beginTime) 
            {
                var endTime = DateTime.Now;

                eventData["beginTime"] = _beginTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                eventData["endTime"] = endTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                eventData["duration"] = (endTime - _beginTime).TotalMilliseconds;
            }


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

            JToken GetTagValue(string valueString)
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
                            if (requestRpc_json == null)
                            {
                                requestRpc_json = requestRpc_oriString.Deserialize<JObject>();
                            }
                            if (string.IsNullOrEmpty(path))
                            {
                                return requestRpc_json;
                            }
                            return requestRpc_json?.SelectToken(path);

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
                            return requestData_json?.SelectToken(path);

                        case "responseRpc":
                            if (apiResponseMessage == null)
                            {
                                apiResponseMessage = GetApiReplyMessage();
                            }
                            if (responseRpc_oriString == null)
                            {
                                responseRpc_oriString = apiResponseMessage.rpcContextData_OriData.ArraySegmentByteToString();
                            }
                            if (responseRpc_json == null)
                            {
                                responseRpc_json = responseRpc_oriString.Deserialize<JObject>();
                            }
                            if (string.IsNullOrEmpty(path))
                            {
                                return responseRpc_json;
                            }
                            return responseRpc_json?.SelectToken(path);

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
                            return responseData_json?.SelectToken(path);
                    }
                }
                catch
                {
                }
                return null;
            }
            #endregion



            tagsTemplate?.IEnumerable_ForEach(item =>
            {
                var key = GetTagValue(item.Key)?.ConvertToString();
                if (string.IsNullOrEmpty(key)) return;

                var value = GetTagValue(item.Value);
                eventData[key] = value;

            });

            return eventData;
        }

        #endregion

    }
}
