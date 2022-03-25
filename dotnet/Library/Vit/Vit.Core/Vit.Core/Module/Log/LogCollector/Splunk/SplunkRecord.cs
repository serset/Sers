using Newtonsoft.Json;

using System;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector.Splunk
{
    public class SplunkRecord
    {/*
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
                    "app": {
                      "namespace": "mc.sers.cloud",
                      "appName": "mc",
                      "moduleName": "sers"
                      //,"...": {}
                    }
                }
             }
             */
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? time;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string index;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string host;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string source;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sourcetype;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object @event;




        public DateTime Time { set => time = ToEpoch(value); }
        

        public static double ToEpoch(DateTime value)
        {
            // From Splunk HTTP Collector Protocol
            // The default time format is epoch time format, in the format <sec>.<ms>. 
            // For example, 1433188255.500 indicates 1433188255 seconds and 500 milliseconds after epoch, 
            // or Monday, June 1, 2015, at 7:50:55 PM GMT.
            // See: http://dev.splunk.com/view/SP-CAAAE6P
            return value.ToTimeStamp() / 1000.0;
        }
    }
}
