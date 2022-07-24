using Newtonsoft.Json;

using System;

namespace Vit.Core.Module.Log.LogCollector.Splunk
{

    /* Splunk LogRecord Format:
    {
       "time": 1426279439.123,  

       "index": "dev",

       "host": "localhost",
       "source": "random-data-generator",
       "sourcetype": "my_sample_data",

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

    public class LogEvent
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string level;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string message;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object[] metadata;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object appInfo;
    }
}
