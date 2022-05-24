using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector.Splunk
{
    public class SplunkCollector : ILogCollector
    {
        public JObject config;
        public void Init(JObject config)
        {
            if (config == null) return;

            this.config = config;


            client = config["client"]?.Deserialize<LogClient>();
            message = config["message"]?.Deserialize<LogMessage>();
            appInfo = config["appInfo"]?.Deserialize<object>();
            client?.Init();
        }




        /* SplunkMessage Format:
            {
               "time": 1426279439.123,  
               "host": "localhost",
               "source": "random-data-generator",
               "sourcetype": "my_sample_data",
               "index": "dev",
               "event": { 
                   "level": "info",
                   "message": "Something happened",
                   "metadata": [],
                    //custome object
                   "appInfo": {
                     "namespace": "mc.sers.cloud",
                     "appName": "mc",
                     "moduleName": "sers"
                     //,"...": {}
                   }
               }
            }
            */


        internal LogClient client;
        internal LogMessage message;
        public object appInfo;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(Log.LogMessage msg)
        {
            if (msg.metadata != null && msg.metadata.Length == 0) msg.metadata = null;

            var record = new LogMessage
            {
                Time = DateTime.UtcNow,
                index = message?.index,
                host = message?.host ?? Environment.MachineName,
                source = message?.source,
                sourcetype = message?.sourcetype,

                @event = new Event
                {
                    level = msg.level.ToString(),
                    message = msg.message,
                    metadata = msg.metadata,
                    appInfo = appInfo
                }
            };

            client.SendAsync(record);
        }


        internal class Event
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string level;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string message;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public object metadata;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public object appInfo;
        }






    }
}
