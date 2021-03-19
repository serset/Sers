using System;
using System.Threading.Tasks;
using Statistics;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions;
 

namespace Sers.Core.Module.LocalApi.MsTest.LocalApi
{

    public class LocalApiTest
    {
        static StatisticsQpsAsync statisticsQps = new StatisticsQpsAsync();
        static LocalApiService localApiService;
        static LocalApiTest()
        {
            //(x.1)构建
            localApiService = new LocalApiService() { workThreadCount = 4 };
            localApiService.LoadSersApi(typeof(LocalApiTest).Assembly);

            localApiService.Start();


            statisticsQps.Start("Msg");

        }



        public static void StartThread()
        {
            QpsData qpsInfo = new QpsData(statisticsQps);
            Task.Run(() =>
            {

                while (true)
                {
                    try
                    {
                        for (var t = 0; t < 10000; t++)
                        {
                              
                            string route = "/Test/api/GetDeviceGuidList";
                            string arg = "asfsdf";
                            object argValue = new { arg };

                            object returnValue = localApiService.CallLocalApi<string>(route, argValue);
                        }

                        qpsInfo.RequestCount++;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            });

        }

    }
}
