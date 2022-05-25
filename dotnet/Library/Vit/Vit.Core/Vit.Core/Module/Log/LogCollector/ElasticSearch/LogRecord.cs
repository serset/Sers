using Newtonsoft.Json;

using System;

using Vit.Core.Module.Log.LogCollector.ElasticSearch.Client;

namespace Vit.Core.Module.Log.LogCollector.ElasticSearch
{
    // ElasticSearch LogRecord Format:
    // "index": "dev", "type": "_doc"
    /* 
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
    public class LogRecord : ElasticSearchRecord
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string level;


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string message;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object[] metadata;
    }
}
