using System;
using Statistics;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions;


namespace Sers.Core.Module.LocalApi.MsTest.LocalApi
{
    public class LocalApiTest
    {        
    
        public static int workThreadCount = 4;

        static StatisticsQpsAsync statisticsQps = new StatisticsQpsAsync();
        static LocalApiService localApiService;
        static LocalApiTest()
        {
            //(x.1)构建
            localApiService = new LocalApiService() { workThreadCount = workThreadCount };
            localApiService.LoadSersApi(typeof(LocalApiTest).Assembly);

            localApiService.Start();


            statisticsQps.Start("Msg");

        }



        public static void StartThread()
        {
            QpsData qpsInfo = new QpsData(statisticsQps);

            int t = 0;

            Action<string> callApi = null;

            callApi = reply =>{

                t++;
                if (t >= 1000)
                {
                    t = 0;
                    qpsInfo.RequestCount++;
                }
                
                //string route = "/Test/api/GetDeviceGuidList";
                //string arg = "asfsdf";
                //object argValue = new { arg };

                localApiService.CallLocalApi<string>("/a", null, callApi);
            };

            callApi(null);
        }

    }
}
