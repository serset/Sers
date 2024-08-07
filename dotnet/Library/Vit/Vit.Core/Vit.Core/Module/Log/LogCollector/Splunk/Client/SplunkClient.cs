﻿using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;

using Vit.Core.Module.Serialization;

namespace Vit.Core.Module.Log.LogCollector.Splunk.Client
{
    public class SplunkClient
    {

        /// <summary>
        /// 接口地址，如 "http://192.168.20.20:8088/services/collector"
        /// </summary>
        public string url;

        public string authToken;

        public string index;


        /// <summary>
        /// 若指定则在指定时间间隔统一推送数据，若不指定则立即推送。单位:ms
        /// </summary>
        public int? intervalMs;

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
            //信任所有的证书
            var HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
            httpClient = new System.Net.Http.HttpClient(HttpHandler);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTH_SCHEME ?? "Splunk", authToken);
            if (!httpClient.DefaultRequestHeaders.Contains(SPLUNK_REQUEST_CHANNEL ?? "X-Splunk-Request-Channel"))
            {
                httpClient.DefaultRequestHeaders.Add(SPLUNK_REQUEST_CHANNEL ?? "X-Splunk-Request-Channel", Guid.NewGuid().ToString());
            }

            InitTimer();
        }




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SendAsync(SplunkRecord record)
        {
            record.index = index;

            if (recordList == null)
                SendToServer(record);
            else
                recordList.Add(record);
        }


        ~SplunkClient()
        {
            if (time != null)
            {
                time?.Stop();
                time = null;
            }
        }


        #region Timer
        ConcurrentBag<SplunkRecord> recordList;
        ConcurrentBag<SplunkRecord> recordList_Swap;
        Util.Threading.Timer.VitTimer_SingleThread time;

        private void InitTimer()
        {
            if (intervalMs.HasValue && intervalMs.Value > 0)
            {
                recordList = new ConcurrentBag<SplunkRecord>();
                recordList_Swap = new ConcurrentBag<SplunkRecord>();
                time = new Util.Threading.Timer.VitTimer_SingleThread();
                time.intervalMs = intervalMs.Value;
                time.timerCallback = (e) =>
                {
                    (recordList_Swap, recordList) = (recordList, recordList_Swap);
                    if (recordList_Swap.Count > 0)
                    {
                        SendToServer(recordList_Swap);
                        while (recordList_Swap.TryTake(out _)) ;
                    }
                };
                time.Start();
            }
        }
        #endregion





        private System.Net.Http.HttpClient httpClient = null;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void SendToServer(object record)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(Json.Serialize(record), Vit.Core.Module.Serialization.Serialization_Newtonsoft.defaultEncoding, "application/json");

            // TODO:    retry when fail. 
            //          batch:  batchIntervalInSeconds, batchSizeLimit, queueLimit
            httpClient.SendAsync(request);

            //var strMsg = record.Serialize();
            //var response = httpClient.SendAsync(request).Result;
            //var body = response.Content.ReadAsStringAsync().Result;
        }
    }
}
