using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.ConfigurationManager;
using Sers.ServiceStation.Util.StaticFileTransmit;
using System;
using System.Collections.Generic;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Rpc;

namespace App.MicroControl.Station.Controllers
{ 
    public class UIController : IApiController
    {

        #region UseStaticFiles

        // wwwroot 路径从配置文件获取
        static StaticFileMap staticFileMap = new StaticFileMap(ConfigurationManager.Instance.GetByPath<string>("Host.wwwroot"));

        /// <summary>
        /// UseStaticFiles
        /// </summary>
        /// <returns></returns>
        [SsRoute("*")]
        public byte[] UseStaticFiles()
        {
            return staticFileMap.TransmitFile();
        }
        #endregion


        #region McData Mng
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        [SsRoute("mcMng/GetAll")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn<List<object>> GetAll()
        {

            var apiRet = ApiClient.CallRemoteApi<ApiReturn<List<JObject>>>("/_gover_/serviceStation/getAll", null);

            if (!apiRet.success) return apiRet.error;


            List<object> data = new List<object>();

            foreach (var item in apiRet.data)
            {
                var serviceStationInfo = item.SelectToken("serviceStationInfo.info.mcData");
                if (serviceStationInfo.IsJArray())
                {
                    foreach (JObject mcItem in serviceStationInfo.Value<JArray>())
                    {
                        data.Add(mcItem);
                    }
                }
            }
            return data;
        }



        #endregion


    }
}
