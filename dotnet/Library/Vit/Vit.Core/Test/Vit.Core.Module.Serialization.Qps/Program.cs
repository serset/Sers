using Newtonsoft.Json.Linq;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.Rpc.Serialization;
using Sers.Core.Module.Rpc.Serialization.Fast;
using Statistics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vit.Extensions;

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

                var demo_data = new RpcContextData();

                demo_data.route = "/a";

                demo_data.caller.rid = "8320becee0d945e9ab93de6fdac7627a";
                demo_data.caller.source = "Internal";
                //data.caller.callStack = new List<string>() { "asdgsgsagddsg" };

                demo_data.http.url = "http://sers.internal/a";
                demo_data.http.method = "GET";
                //data.http.Headers()["Content-Type"]= "application/javascript";


                //byte[] demo_bytes_MessagePack = MessagePack_RpcContextData.SerializeToBytes(demo_data);
                //byte[] demo_bytes = Text_RpcContextData.SerializeToBytes(demo_data);



                byte[] bytes;
                JObject jo;
                string str;
                RpcContextData data2;

                while (true)
                {
                    try
                    {
                

                        for (var t = 0; t < 10000; t++)
                        {

                            //(x.1)MessagePack
                            //vr 1线程 
                            //序列化         253 万qps
                            //反序列化       157 万qps  
                            //序列化+序列化   85 万qps
                            //bytes = MessagePack_RpcContextData.Instance.SerializeToBytes(demo_data);
                            //data2 = MessagePack_RpcContextData.Instance.DeserializeFromBytes(bytes);


                            //(x.2)Text                       
                            //vr 1线程 
                            //序列化         130 万qps
                            //反序列化       103 万qps  
                            //序列化+序列化   50 万qps
                            //bytes = Text_RpcContextData.Instance.SerializeToBytes(demo_data);
                            //data2 = Text_RpcContextData.Instance.DeserializeFromBytes(bytes);


                            //(x.3)StringBuilder                       
                            //vr 1线程 
                            //序列化         387 万qps
                            //反序列化           万qps  
                            //序列化+序列化      万qps
                            //bytes = StringBuilder_RpcContextData.SerializeToBytes(demo_data);
                            //data2 = Text_RpcContextData.DeserializeFromBytes(demo_bytes);



                            //(x.3)Fast                       
                            //vr 1线程 
                            //序列化         480 万qps
                            //反序列化           万qps  
                            //序列化+序列化  180 万qps
                            bytes = BytePointor_RpcContextData.Instance.SerializeToBytes(demo_data);
                            //data2 = BytePointor_RpcContextData.Instance.DeserializeFromBytes(bytes);

                        }

                        qpsInfo.RequestCount++;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            });

        }



        public static void StartThread2()
        {
            QpsData qpsInfo = new QpsData(statisticsQps);
            Task.Run(() =>
            {
                
 
                var Instance = Vit.Core.Module.Serialization.Serialization_Text.Instance;

                byte[] bytes;
                JObject jo;
                string str;
                RpcContextData data, data2;


                data = new RpcContextData();

                data.route = "/a";

                data.http.url = "http://sers.internal/a";
                data.http.method = "GET";
                data.http.statusCode = 400;
                data.http.protocol = "HTTP/2.0";
                data.http.Headers()["Content-Type"] = "application/javascript";

                data.caller.rid = "8320becee0d945e9ab93de6fdac7627a";
                data.caller.callStack = new List<string>() { "asdgsgsagddsg" };
                data.caller.source = "Internal";

                jo = new JObject() { ["userInfo11"] = 11.545, ["userInfo12"] = 123456789012456 };
                jo["userInfo13"] = new JObject() { ["userInfo131"] = "131", ["userInfo132"] = "132" };
                jo["userInfo14"] = null;
                jo["userInfo15"] = new JArray { 1, true, 12.5, null, DateTime.Now };
                data.user = jo;


                IRpcSerialize RpcSerialize = BytePointor_RpcContextData.Instance;

                while (true)
                {
                    try
                    {                      

                        for (var t = 0; t < 10000; t++)
                        {           

                            bytes = RpcSerialize.SerializeToBytes(data);
                            //str = bytes.BytesToString();
                            data2 = RpcSerialize.DeserializeFromBytes(bytes);
                        }

                        qpsInfo.RequestCount++;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            });

        }




        public static void StartThread3()
        {
            QpsData qpsInfo = new QpsData(statisticsQps);
            Task.Run(() =>
            {
                // 4线程 72万(cpu 50%)
                // 8线程 79万(cpu 86%)
                //var Instance = Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance;


                //vr 1线程 17-19万
                // 4线程 87万(cpu 52%)
                // 8线程 103万(cpu 95%)
                var Instance = Vit.Core.Module.Serialization.Serialization_Text.Instance;


                //vr 1线程 58万
                // 4线程 200万(cpu 50%)
                //var Instance = Vit.Core.Module.Serialization.Serialization_MessagePack.Instance;
                //vr 1线程 48万
                //Instance.CompatibleWithNewtonsoft();


                //vr 1线程 69万
                //MessagePackFormatter_RpcContextData 全部序列化



                //vr 1线程 序列化  242万
                //MessagePackFormatter_RpcContextData

                //vr 1线程  序列化 391万
                //StringBuild SerializeToBytes

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

                            data.http.url = "http://sers.internal/a";
                            data.http.method = "GET";
                            //data.http.Headers()["Content-Type"]= "application/javascript";

                            data.caller.rid = "8320becee0d945e9ab93de6fdac7627a";
                            //data.caller.callStack = new List<string>() { "asdgsgsagddsg" };

                            //bytes = data.ToBytes();

                            //bytes = Text_RpcContextData.SerializeToBytes(data);
                            //data2 = Text_RpcContextData.DeserializeFromBytes(bytes);


                            //bytes = Instance.SerializeToBytes(data);
                            //data2 = Instance.DeserializeFromBytes<RpcContextData>(bytes);



                            //data = new RpcContextData { route = "/a" };
                            //jo = new JObject() { ["userInfo11"] = 11.545, ["userInfo12"] = 123456789012456 };
                            //jo["userInfo13"] = new JObject() { ["userInfo131"] = "131", ["userInfo132"] = "132" };
                            //jo["userInfo14"] = null;
                            //jo["userInfo15"] = new JArray { 1, true, 12.5, null, DateTime.Now };

                            //data.user = jo;


                            //str = Instance.SerializeToString(data);

                            //bytes = Instance.SerializeToBytes(data);

                            //data2 = Instance.DeserializeFromBytes<RpcContextData>(bytes);
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
