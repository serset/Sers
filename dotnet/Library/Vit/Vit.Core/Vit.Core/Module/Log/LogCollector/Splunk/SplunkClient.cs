using System;
using System.Net.Http.Headers;
using System.Net.Http;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector.Splunk
{
    public class SplunkClient
    {

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



       
        public void Init()
        {
            httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTH_SCHEME ?? "Splunk", authToken);
            if (!httpClient.DefaultRequestHeaders.Contains(SPLUNK_REQUEST_CHANNEL ?? "X-Splunk-Request-Channel"))
            {
                httpClient.DefaultRequestHeaders.Add(SPLUNK_REQUEST_CHANNEL ?? "X-Splunk-Request-Channel", Guid.NewGuid().ToString());
            }
        }

        private System.Net.Http.HttpClient httpClient = null;



        public void SendAsync(SplunkRecord record) 
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(record.Serialize(), Vit.Core.Module.Serialization.Serialization_Newtonsoft.defaultEncoding, "application/json");

            // TODO:    retry when fail. 
            //          batch:  batchIntervalInSeconds, batchSizeLimit, queueLimit
            httpClient.SendAsync(request);

            //var strMsg = msgBody.Serialize();
            //var response = httpClient.SendAsync(request).Result;
        }
    }
}
