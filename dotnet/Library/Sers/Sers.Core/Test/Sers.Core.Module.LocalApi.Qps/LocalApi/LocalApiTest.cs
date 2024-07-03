using System;
using System.Threading.Tasks;

using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions;

using Statistics;

using Vit.Extensions;

namespace Sers.Core.Module.LocalApi.MsTest.LocalApi
{
    public class LocalApiTest
    {

        public static int threadCount = 4;

        static StatisticsQpsAsync statisticsQps = new StatisticsQpsAsync();
        static LocalApiService localApiService;
        static LocalApiTest()
        {
            //(x.1)构建
            localApiService = LocalApiServiceFactory.CreateLocalApiService() as LocalApiService;
            localApiService.threadCount = threadCount;
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
                            //string route = "/Test/api/GetDeviceGuidList";
                            //string arg = "asfsdf";
                            //object argValue = new { arg };

                            var apiReplyMessage = localApiService.CallLocalApi("/a", null);
                        }

                        qpsInfo.RequestCount++;
                    }
                    catch { }
                }

            });

        }

        public static void StartThread_Async()
        {
            QpsData qpsInfo = new QpsData(statisticsQps);

            int t = 0;

            Action<string> callApi = null;

            callApi = reply =>
            {

                t++;
                if (t >= 10000)
                {
                    t = 0;
                    qpsInfo.RequestCount++;
                }

                //string route = "/Test/api/GetDeviceGuidList";
                //string arg = "asfsdf";
                //object argValue = new { arg };

                localApiService.CallLocalApiAsync<string>("/a", null, callApi);
            };

            callApi(null);
        }

    }
}
