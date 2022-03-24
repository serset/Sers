using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Net.Http.Headers;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector
{
    public class SplunkCollector : ILogCollector
    {
        private static DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public static double ToEpoch(DateTime value)
        {
            // From Splunk HTTP Collector Protocol
            // The default time format is epoch time format, in the format <sec>.<ms>. 
            // For example, 1433188255.500 indicates 1433188255 seconds and 500 milliseconds after epoch, 
            // or Monday, June 1, 2015, at 7:50:55 PM GMT.
            // See: http://dev.splunk.com/view/SP-CAAAE6P
            return value.ToTimeStamp() / 1000.0;
        }





        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(LogMessage msg)
        {
            if (msg.metadata != null && msg.metadata.Length == 0) msg.metadata = null;

            var msgBody = new Message
            {
                time = ToEpoch(DateTime.UtcNow),
                index = message?.index,
                host = message?.host ?? Environment.MachineName,
                source = message?.source,
                sourcetype = message?.sourcetype,

                @event = new Message.Event
                {
                    level = msg.level.ToString(),
                    message = msg.message,
                    metadata = msg.metadata,

                    @namespace = message?.@event?.@namespace,
                    app = message?.@event?.app
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(msgBody.Serialize(), Vit.Core.Module.Serialization.Serialization_Newtonsoft.defaultEncoding, "application/json");

            // TODO:    retry when fail. 
            //          batch:  batchIntervalInSeconds, batchSizeLimit, queueLimit
            httpClient.SendAsync(request);

            //var strMsg = msgBody.Serialize();
            //var response = httpClient.SendAsync(request).Result;

        }



        public class Message 
        {     
            public double time;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string index;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string host;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string source;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string sourcetype;

            [JsonProperty("event")]
            public Event @event;

            public class Event
            {

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string level;

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string message;

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public object metadata;


                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string @namespace;

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string app;
            }

            /*
             {
                "time": 1426279439,  
                "host": "localhost",
                "source": "random-data-generator",
                "sourcetype": "my_sample_data",
                "index": "index_prod",
                "event": { 
                    "level": "info",
                    "message": "Something happened",
                    "metadata": [],
                    "namespace": "mc.sers.cloud",
                    "app":"mc"
                }
             }
             */
        }
        public Message message;

        /// <summary>
        /// 接口地址，如 "http://192.168.20.20:8088/services/collector"
        /// </summary>
        public string url;

        public string authToken;

        /// <summary>
        /// 默认 "Splunk"
        /// </summary>
        public string AUTH_SCHEME;
        /// <summary>
        /// 默认 "X-Splunk-Request-Channel"
        /// </summary>
        public string SPLUNK_REQUEST_CHANNEL;

        #region HttpClient

        HttpClient _httpClient = null;


        public HttpClient httpClient 
        {
            get 
            {
                if (_httpClient == null) 
                {
                    _httpClient = new HttpClient();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTH_SCHEME ?? "Splunk", authToken);
                    if (!_httpClient.DefaultRequestHeaders.Contains(SPLUNK_REQUEST_CHANNEL ?? "X-Splunk-Request-Channel"))
                    {
                        _httpClient.DefaultRequestHeaders.Add(SPLUNK_REQUEST_CHANNEL ?? "X-Splunk-Request-Channel", Guid.NewGuid().ToString());
                    }
                }
                return _httpClient;
            }
        }

        #endregion






    }
}
