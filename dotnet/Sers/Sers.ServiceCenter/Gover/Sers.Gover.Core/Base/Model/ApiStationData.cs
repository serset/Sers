using Newtonsoft.Json;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Util.Counter;
using Sers.Core.Util.Extensible;
using Sers.ServiceCenter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sers.Gover.Core.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiStationData : Extensible
    {
        [JsonProperty]
        public string stationName;
        [JsonProperty]
        public int apiServiceCount => apiServices.Count;
        [JsonProperty]
        public int apiNodeCount => apiServices.Values.Sum((apiService=>apiService.apiNodeCount));


        [JsonProperty]
        public int activeApiNodeCount =>
            apiServices.Values.SelectMany(m => m.apiNodes).Count((apiNode) => apiNode.Status_Get() == EServiceStationStatus.正常);

        [JsonProperty]
        public EServiceStationStatus eStatus
        {
            get { return this.GetDataByConvert<EServiceStationStatus?>("status")??EServiceStationStatus.正常; }
            set { this.SetData("status", value); }
        }

       
        [JsonProperty]
        public string status => eStatus.ToString();



        public bool IsActive()
        {
            return eStatus == EServiceStationStatus.正常;
        }


        /// <summary>
        /// route ApiService
        /// </summary>
        [JsonProperty]
        public ConcurrentDictionary<string, ApiService> apiServices = new ConcurrentDictionary<string, ApiService>();



        #region counter    
        [JsonIgnore]
        private Counter _counter;
        [JsonProperty]
        public Counter counter { get => (_counter ?? (_counter = new Counter())); set => _counter = value; }
        #endregion


        #region QpsCacl
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty]
        public float qps { get; private set; } = -1;

        private DateTime? qps_TimeLast = null;
        private int qps_SumCountLast = 0;
        public void QpsCalc()
        {
            var curSumCount = counter.sumCount;
            var curTime = DateTime.Now;
            if (qps_TimeLast != null)
            {
                qps = ((int)(100000.0f * (curSumCount - qps_SumCountLast) / (curTime - qps_TimeLast.Value).TotalMilliseconds))/100f;
            }
            qps_TimeLast = curTime;
            qps_SumCountLast = curSumCount;
        }

        #endregion


        ApiService InitApiService(ApiService apiService)
        {
            apiService.counter.ReportTo(counter);
            return apiService;
        }
         

        public ApiService ApiService_Get(string route)
        {
            if (apiServices.TryGetValue(route, out var apiService)) return apiService;
            return null; 
        }
        public ApiService ApiService_GetOrAdd(SsApiDesc  apiDesc)
        {
            var apiService = apiServices.GetOrAdd(apiDesc.route, (n) => { return InitApiService(new ApiService { apiDesc= apiDesc }); });
            return apiService;
        }

        #region ApiNode            
        public void ApiNode_Add(ApiNode apiNode)
        {
            var apiService = ApiService_GetOrAdd(apiNode.apiDesc);
            apiService.AddApiNode(apiNode);
        }
        public void ApiNode_Remove(string route, ApiNode apiNode)
        {
            var apiService = apiNode.apiService;
            //if (!apiServices.TryGetValue(route, out var apiService))
            //{
            //    return;
            //}
            if (null == apiService) return;
            apiService.RemoveApiNode(apiNode);
            //if (apiService.apiNodeCount == 0)
            //{
            //    apiServices.TryRemove(route, out _);
            //}
        }
        #endregion

    }
}
