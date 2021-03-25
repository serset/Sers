using Newtonsoft.Json.Linq;

using Sers.Core.Module.Rpc;
using Statistics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        public static int workThreadCount = 1;


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
                var Instance = Vit.Core.Module.Serialization.Serialization_Text.Instance;

                // 4线程 200万(cpu 50%)
                // var Instance = Vit.Core.Module.Serialization.Serialization_MessagePack.Instance;

                while (true)
                {
                    try
                    {
                        byte[] bytes;
                        JObject jo;
                        string str;
                        RpcContextData data, data2;

                        for (var t = 0; t < 10000; t++)
                        {
                            data = new RpcContextData { route = "/a" };
                            data.http.method = "GET";

                            bytes = Instance.SerializeToBytes(data);
                            data2 = Instance.DeserializeFromBytes<RpcContextData>(bytes);



                            data = new RpcContextData { route = "/a" };
                            jo = new JObject() { ["userInfo11"] = 11.545, ["userInfo12"] = 123456789012456 };
                            jo["userInfo13"] = new JObject() { ["userInfo131"] = "131", ["userInfo132"] = "132" };
                            jo["userInfo14"] = null;
                            jo["userInfo15"] = new JArray { 1, true, 12.5, null, DateTime.Now };

                            data.user = jo;
                            data.joUser = jo;

                            str = Instance.SerializeToString(data);

                            bytes = Instance.SerializeToBytes(data);

                            data2 = Instance.DeserializeFromBytes<RpcContextData>(bytes);
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
