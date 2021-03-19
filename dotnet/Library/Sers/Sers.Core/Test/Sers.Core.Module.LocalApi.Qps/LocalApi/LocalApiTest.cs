using System;
using System.Threading.Tasks;
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
            Task.Run(() =>
            {

                while (true)
                {
                    try
                    {
                        for (var t = 0; t < 1000; t++)
                        {                       

                            object returnValue = localApiService.CallLocalApi<string>("/a",null);

                            //string route = "/Test/api/GetDeviceGuidList";
                            //string arg = "asfsdf";
                            //object argValue = new { arg };

                            //object returnValue = localApiService.CallLocalApi<string>(route, argValue);
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
