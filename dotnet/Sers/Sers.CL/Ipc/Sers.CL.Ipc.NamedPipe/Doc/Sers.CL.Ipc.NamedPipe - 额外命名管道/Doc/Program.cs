using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Vit.Core.Module.Log;

namespace DeliveryTest
{
    /// <summary>
    /// https://www.liujiajia.me/2020/3/27/dotnet-core-named-pipe
    /// </summary>
    class Program
    {

   
        static void Main(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };


            Task.Run(StartServer);
            //Task.Run(StartServer);
            //Task.Run(StartServer);
            //Task.Run(StartServer);


            Task.Run(StartClient);
            Thread.Sleep(5000);
            Task.Run(StartClient);
            //Task.Run(StartClient);
            //Task.Run(StartClient);



            while (true)
            {
                Thread.Sleep(5000);
            }

        }



       static int serverId = 1;
        static void StartServer()
        {
            string id = "server" + (serverId++);

            while (1 == 1)
            {

                using (NamedPipeServerStream server = new NamedPipeServerStream("demo", PipeDirection.Out))
                {
                    Console.WriteLine($"[{id}]服务器管道已创建。");

                    // 等待客户端的连接
                    server.WaitForConnection();
                    Console.WriteLine($"[{id}]正在等待客户端连接......");

                    using (StreamWriter writer = new StreamWriter(server))
                    {
                        writer.AutoFlush = true;
                        int t = 0;
                        while (++t<10)
                        {

                            string msg = "你好from-" + id;
                        writer.WriteLine(msg);
                        Thread.Sleep(1000);
                        }
                    }

                    //server.Close();
                }
            }

        }


        static int clientId = 1;
        static void StartClient()
        {
            string id = "client" + (clientId++);

            try
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream(".", "demo", PipeDirection.In))
                {
                    Console.WriteLine($"[{id}]即将连接服务器。");


                    client.Connect();
                    Console.WriteLine($"[{id}]连接成功。");


                    using (StreamReader reader = new StreamReader(client))
                    {
                        string msg = null;
                        while ((msg = reader.ReadLine()) != null)
                        {
                            Console.WriteLine($"[{id}]get message：{msg}");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生了异常：{ex}");
            }
            Console.WriteLine($"[{id}]closed");

        }

    }
}
