using Sers.Core.Module.Log;
using Sers.Gover.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Util.Common;

namespace Sers.Gover.Core.Persistence
{
    public class Persistence_ApiDesc
    {
        static string GetFilePath(string jsonFileName)
        {
            return CommonHelp.GetAbsPathByRealativePath(new[] { "Data", "Sers", "Gover", "ApiDesc", jsonFileName });            
        }

        /// <summary>
        /// 持久化ApiStation所有ApiDesc（demo: /Data/Sers/Gover/ApiDesc/_Sys_.json）
        /// </summary>
        /// <param name="data"></param>
        public static void ApiDesc_SaveApiStationToJsonFile(ApiStationData data)
        {
            try
            {
                var apiDescs = data?.apiServices.Values.Select(m => m.apiDesc).ToList();

                var filePath = GetFilePath(data.stationName + ".json");

                var fileContent = apiDescs.SerializeToBytes();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));


                File.WriteAllBytes(filePath, fileContent);          
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public static void ApiDesc_LoadAllFromJsonFile(ApiStationMng apiStationMng)
        {
            try
            {
                var foldPath = CommonHelp.GetAbsPathByRealativePath(new[] { "Data", "Sers", "Gover", "ApiDesc" });
                if (!Directory.Exists(foldPath)) return;
                string[] files = Directory.GetFiles(foldPath, "*.json");
                foreach (string filePath in files)
                {
                    try
                    {                
                        ArraySegment<byte> fileContent = File.ReadAllBytes(filePath).BytesToArraySegmentByte();

                        var apiDescs = fileContent.DeserializeFromBytes<List<SsApiDesc>>();

                        foreach (var apiDesc in apiDescs)
                        {
                            apiStationMng.ApiStation_GetOrAddByRoute(apiDesc.route)?.ApiService_GetOrAdd(apiDesc);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
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
