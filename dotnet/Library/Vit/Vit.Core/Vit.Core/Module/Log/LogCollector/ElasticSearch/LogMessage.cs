using Newtonsoft.Json;

using System;

namespace Vit.Core.Module.Log.LogCollector.ElasticSearch
{
    internal class LogMessage
    {
        /* ElasticSearchMessage Format:
        // "index": "dev", "type": "_doc",
          {
              "@timestamp":1653468236619,
              "time": "2022-05-25T08:19:36.686Z", 
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
      */
        [JsonProperty("@timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public string timestamp;


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string time;


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string level;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string message;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object[] metadata;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object appInfo;


        public DateTime Time 
        {
            set
            {
                //timestamp = value.ToTimeStamp();
                timestamp = time = ToTimeString(value);
            }
        }


        public static string ToTimeString(DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }
}
