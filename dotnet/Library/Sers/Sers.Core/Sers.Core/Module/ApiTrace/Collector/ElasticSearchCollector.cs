using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;
using System.Collections.Generic;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Vit.Core.Module.Log.LogCollector.ElasticSearch.Client;
using Newtonsoft.Json;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.Module.ApiTrace.Collector
{
    public class ElasticSearchCollector : IApiTraceCollector
    {
        // ElasticSearch ApiTrace Record Format:
        // "index": "dev", "type": "_doc"
        /* 
        {
            "@timestamp":1653468236619,
            "time": "2022-05-25T08:19:36.686Z", 

            "level": "ApiTrace",
            "apiTrace": {
                "beginTime":"2022-03-26 02:52:00.123456",
                "endTime":"2022-03-26 02:52:04.123456",
                "duration":"4000.0",

                //extTags
            },

            //custome object
            "appInfo": {
                "namespace": "mc.sers.cloud",
                "appName": "mc",
                "moduleName": "sers"
                //,"...": {}
            }
        }
        */
        public class ApiTraceRecord : ElasticSearchRecord
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string level;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public JObject apiTrace;
        }


        ElasticSearchClient client;
        JObject appInfo;

        IDictionary<string, string> tagsTemplate;

        public void Init(JObject arg)
        {
            Logger.Info("[ApiTrace.ElasticSearchCollector] init ...");
            client = arg["server"].Deserialize<ElasticSearchClient>();

            appInfo = arg?["appInfo"]?.Deserialize<JObject>();
            tagsTemplate = arg?["tags"]?.Deserialize<IDictionary<string, string>>();

            client?.Init();
        }
        

        public object TraceStart(RpcContextData rpcData)
        {
            return DateTime.Now;
        }
        public void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage)
        {
            JObject eventData = SplunkCollector.BuildEventData(traceData, rpcData, apiRequestMessage, GetApiReplyMessage, tagsTemplate);

            var record = new ApiTraceRecord
            {
                Time = DateTime.UtcNow,

                level = "ApiTrace",
                apiTrace = eventData,

                appInfo = appInfo
            };
            client.SendAsync(record);
        }
    }
}
