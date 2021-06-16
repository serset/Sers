using System;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.Module.Counter;
using Sers.Gover.Base;
using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;

namespace Sers.Gover.Persistence
{
    public class Persistence_Counter
    {
  
        static readonly string[] filePath = new [] {"Data", "Sers", "Gover", "Counter.json" };


        /*
         Counter.json


            {
              "ApiStations":{
                        "_sys_":{ counter}            
                },

               "ApiServices":{
                        "/_sys/api1":{counter},
                        "/_sys/api2":{counter}
                }            
            }             
             
             
             */


        /// <summary>
        /// 持久化ApiStation所有Counter（demo: /Data/Sers/Gover/Counter.json）
        /// </summary>
        /// <param name="apiStationMng"></param>
        public static void SaveCounterToJsonFile(ApiStationMng apiStationMng)
        {
            try
            {
                #region (x.1)构建文件内容

                var apiStations= apiStationMng.ApiStation_GetAll();


                var counter_ApiStations = apiStations.ToDictionary(m => m.stationName, m => m.counter);
                var counter_ApiServices = apiStations.SelectMany(m=>m.apiServices.Values).ToDictionary(m => m.apiDesc?.ServiceKeyGet(), m => m.counter);                

                var fileContent = new { ApiStations= counter_ApiStations, ApiServices= counter_ApiServices };
                #endregion

                //(x.2) 保存到文件
                JsonFile.SetToFile(fileContent, filePath);
 
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public static void LoadCounterFromJsonFile(ApiStationMng apiStationMng)
        {
            try
            {
                //(x.1)载入文件
                var counterData = JsonFile.GetFromFile<Dictionary<string, Dictionary<string, Counter>>>(filePath);

                //(x.2)加载数据
                if (counterData.TryGetValue("ApiStations", out var counter_ApiStations))
                {
                    foreach (var item in counter_ApiStations)
                    {
                        apiStationMng.ApiStation_GetOrAddByName(item.Key).counter.CopyDataFrom(item.Value);
                    }
                }

                if (counterData.TryGetValue("ApiServices", out var counter_ApiServices))
                {
                    foreach (var item in counter_ApiServices)
                    {
                        apiStationMng.ApiStation_GetOrAddByRoute(item.Key)?.ApiService_Get(item.Key)?.counter.CopyDataFrom(item.Value);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }





    }
}
