using Sers.Core.Module.Rpc;
using Statistics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        public static int workThreadCount = 4;


        static void Main(string[] args)
        {

            if (args != null)
            {
                if (args.Length >= 1)
                {
                    int.TryParse(args[0], out workThreadCount);
                }
            }
            statisticsQps.Start("Msg");

            for (var t = 0; t < workThreadCount; t++)
            {
                StartThread();
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }




        static StatisticsQpsAsync statisticsQps = new StatisticsQpsAsync();
  
   


        public static void StartThread()
        {
            QpsData qpsInfo = new QpsData(statisticsQps);
            Task.Run(() =>
            {
                // 4线程 72万(cpu 50%)
                // 8线程 79万(cpu 86%)
                //var Instance = Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance;


                // 4线程 87万(cpu 52%)
                // 8线程 103万(cpu 95%)
                // var Instance = Vit.Core.Module.Serialization.Serialization_Text.Instance;

                // 4线程 200万(cpu 50%)
                var Instance = Vit.Core.Module.Serialization.Serialization_MessagePack.Instance;

                while (true)
                {
                    try
                    {
                        for (var t = 0; t < 10000; t++)
                        {
                            var data = new RpcContextData { route = "/a" };
                            //data.http.method = "GET";
                            //var str = Instance.SerializeToString(data);

                            var bytes = Instance.SerializeToBytes(data);

                            //var ss = bytes.BytesToString();

                            var data2 = Instance.DeserializeFromBytes<RpcContextData>(bytes);
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
