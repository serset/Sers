using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Sers.Core.Module.Api.ApiDesc;
using Sers.Gover.Base;
using Sers.Gover.Base.Model;

using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Object_Serialize_Extensions;

namespace Sers.Gover.Persistence
{
    public class Persistence_ApiDesc
    {
        static string GetFilePath(string jsonFileName)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                jsonFileName = jsonFileName.Replace(invalidChar, '_');

            return CommonHelp.GetAbsPath(new[] { "Data", "Sers", "Gover", "ApiDesc", jsonFileName });            
        }

        /// <summary>
        /// 持久化ApiStation所有ApiDesc（demo: /Data/Sers/Gover/ApiDesc/_Sys_.json）
        /// </summary>
        /// <param name="data"></param>
        public static void ApiDesc_SaveApiStationToJsonFile(ApiStationData data)
        {
            try
            {
                var filePath = GetFilePath(data.stationName + ".json");

                var apiDescs = data.apiServices.Values.Select(m => m.apiDesc).ToList();

                if (apiDescs.Count == 0) 
                {
                    if(File.Exists(filePath))
                        File.Delete(filePath);
                }
                else
                {

                    var fileContent = apiDescs.SerializeToBytes();

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    File.WriteAllBytes(filePath, fileContent);
                }
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
                var foldPath = CommonHelp.GetAbsPath(new[] { "Data", "Sers", "Gover", "ApiDesc" });
                if (!Directory.Exists(foldPath)) return;
                string[] files = Directory.GetFiles(foldPath, "*.json");
                foreach (string filePath in files)
                {
                    try
                    {                
                        ArraySegment<byte> fileContent = File.ReadAllBytes(filePath).BytesToArraySegmentByte();

                        var apiDescs = fileContent.DeserializeFromArraySegmentByte<List<SsApiDesc>>();

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
