using System;
using System.Net.Http;
using System.Net.Http.Headers;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector
{
    public class SplunkCollector : ILogCollector
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(LogMessage msg)
        {
            var msgBody = new
            {
                index = message?.index,
                level = msg.level,
                message = msg.message,
                metadata = msg.metadata,
                time = DateTime.Now,
                source = message?.source
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(msgBody.Serialize(), Vit.Core.Module.Serialization.Serialization_Newtonsoft.defaultEncoding, "application/json");

            // TODO:    retry when fail. 
            //          batch:  batchIntervalInSeconds, batchSizeLimit, queueLimit
            var response = httpClient.SendAsync(request);
        }

        public class Message 
        {
            /// <summary>
            /// 
            /// </summary>
            public string index;

            public object source;

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
