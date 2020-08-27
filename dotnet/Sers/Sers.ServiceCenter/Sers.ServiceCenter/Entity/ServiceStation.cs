using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Counter;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Sers.Hardware.Env;
using Sers.Hardware.Usage;
using Vit.Core.Util.Extensible;
using Vit.Extensions;

namespace Sers.ServiceCenter.Entity
{
    /// <summary>
    /// 对应一个部署的服务站点
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceStation: Extensible
    {
        /// <summary>
        /// 服务站点开启时间
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? startTime;

        /// <summary>
        /// CL通信层连接对象
        /// </summary>
        [JsonIgnore]
        public IOrganizeConnection  connection { get; set; }

        /// <summary>
        /// 站点软硬件环境信息
        /// </summary>
        [JsonProperty]
        public DeviceInfo deviceInfo;

        /// <summary>
        /// 站点信息。同一硬件可以部署多个站点，它们软硬件环境是一样的，但站点信息不一样。
        /// </summary>
        [JsonProperty]
        public ServiceStationInfo serviceStationInfo;
     
 
        [JsonProperty]
        public List<ApiNode> apiNodes;

        /// <summary>
        /// 系统当前占用率
        /// </summary>
        //[JsonProperty]
        public UsageStatus usageStatus;

        [JsonIgnore]
        public string serviceStationKey=> serviceStationInfo?.serviceStationKey;


        #region counter    
        [JsonIgnore]
        private Counter _counter;
        //[JsonProperty]
        public Counter counter { get => (_counter ?? (_counter = new Counter())); set => _counter = value; }
        #endregion



        #region QpsCacl
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty]
        public float qps { get; private set; } = 0;

        private DateTime? qps_TimeLast = null;
        private int qps_SumCountLast = 0;
        public void QpsCalc()
        {
            var curSumCount = counter.sumCount;
            var curTime = DateTime.Now;
            if (qps_TimeLast != null)
            {
                qps = ((int)(100000.0f * (curSumCount - qps_SumCountLast) / (curTime - qps_TimeLast.Value).TotalMilliseconds)) / 100f;
            }
            qps_TimeLast = curTime;
            qps_SumCountLast = curSumCount;
        }
        #endregion



        public string GetApiStationNames()
        {
            var stationNames = apiNodes.Select(m => m.apiDesc.ApiStationNameGet());
            return String.Join(",", stationNames); 
        }

     
        public void SendRequestAsync(Object sender, ApiMessage apiReqMessage, Action<object, List<ArraySegment<byte>>> callback)
        {
            connection.SendRequestAsync(sender, apiReqMessage.Package(), callback);            
        }
    }
}
