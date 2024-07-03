using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using Vit.Core.Module.Serialization;

namespace Vit.Core.Module.Log.LogCollector.ElasticSearch.Client
{
    public class ElasticSearchClient
    {
        /// <summary>
        /// http://192.168.20.20:9200/dev/info/_bulk
        /// </summary>
        string bulkUrl;


        /// <summary>
        /// es address, example:"http://192.168.20.20:9200"
        /// </summary>
        public string url;


        /// <summary>
        /// es index, example:"dev"
        /// </summary>
        public string index;

        /// <summary>
        /// es type, example:"_doc"
        /// </summary>
        public string type;

        /// <summary>
        /// 若指定则在指定时间间隔统一推送数据，若不指定则立即推送。单位:ms
        /// </summary>
        public int? intervalMs;



        public void Init()
        {
            //信任所有的证书
            var HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
            httpClient = new System.Net.Http.HttpClient(HttpHandler);

            if (string.IsNullOrEmpty(type)) type = "_doc";
            bulkUrl = url + "/" + index + "/" + type + "/_bulk";

            InitTimer();
        }




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SendAsync(ElasticSearchRecord record)
        {
            if (recordList == null)
                SendToServer(new[] { record });
            else
                recordList.Add(record);
        }


        ~ElasticSearchClient()
        {
            if (time != null)
            {
                time?.Stop();
                time = null;
            }
        }


        #region Timer
        ConcurrentBag<ElasticSearchRecord> recordList;
        ConcurrentBag<ElasticSearchRecord> recordList_Swap;
        Util.Threading.Timer.VitTimer_SingleThread time;

        private void InitTimer()
        {
            if (intervalMs.HasValue && intervalMs.Value > 0)
            {
                StringBuilder buffer = new StringBuilder();
                recordList = new ConcurrentBag<ElasticSearchRecord>();
                recordList_Swap = new ConcurrentBag<ElasticSearchRecord>();
                time = new Util.Threading.Timer.VitTimer_SingleThread();
                time.intervalMs = intervalMs.Value;
                time.timerCallback = (e) =>
                {
                    (recordList_Swap, recordList) = (recordList, recordList_Swap);
                    if (recordList_Swap.Count > 0)
                    {
                        lock (buffer)
                            SendToServer(recordList_Swap, buffer);
                        while (recordList_Swap.TryTake(out _)) ;
                    }
                };
                time.Start();
            }
        }
        #endregion



        private System.Net.Http.HttpClient httpClient = null;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void SendToServer(IEnumerable<ElasticSearchRecord> records, StringBuilder buffer = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, bulkUrl);

            if (buffer == null) buffer = new StringBuilder();
            foreach (var record in records)
            {
                buffer.AppendLine("{\"create\":{}}").AppendLine(Json.Serialize(record));
            }
            request.Content = new StringContent(buffer.ToString(), Vit.Core.Module.Serialization.Serialization_Newtonsoft.defaultEncoding, "application/json");
            buffer.Clear();

            // TODO:    retry when fail. 
            //          batch:  batchIntervalInSeconds, batchSizeLimit, queueLimit
            httpClient.SendAsync(request);


            //var response = httpClient.SendAsync(request).Result;
            //var body = response.Content.ReadAsStringAsync().Result;
        }
    }
}
