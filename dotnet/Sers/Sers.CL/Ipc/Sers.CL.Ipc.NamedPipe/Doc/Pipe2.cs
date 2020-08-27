using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Vit.Core.Module.Log;

namespace DeliveryTest
{
    /// <summary>
    /// https://www.cnblogs.com/kasimlz/p/8619471.html
    /// </summary>
    class Pipe2
    {

   
        static void Main2(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };


            Task.Run(StartServer);
            //Task.Run(StartServer);
            //Task.Run(StartServer);
            //Task.Run(StartServer);


            Task.Run(StartClient);
            //Task.Run(StartClient);
            //Task.Run(StartClient);
            //Task.Run(StartClient);



            while (true)
            {
                Thread.Sleep(5000);
            }

        }
        static string pipeName = "demo2345";


       static int serverId = 1;
        static void StartServer()
        {
            string id = "server" + (serverId++);

            while (1 == 1)
            {

                using (NamedPipeServerStream server = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                {
                    Console.WriteLine($"[{id}]服务器管道已创建。");

                    // 等待客户端的连接
                    server.WaitForConnection();
                    Console.WriteLine($"[{id}]正在等待客户端连接......");


                  
                    using (StreamReader reader = new StreamReader(server))
                    using (StreamWriter writer = new StreamWriter(server))
                    {
                    
                        writer.AutoFlush = true;

                        Task.Run(() => {
                            while (true)
                            {
                                var msg = reader.ReadLine();
                                //var buff1 = new byte[4];
                                //var len = server.Read(buff1, 0, 4);

                            }

                        });

                        var buff = new byte[4] { 1, 2, 3, 4 };
                        while (true)
                        {
                            //var len = server.Read(buff, 0, 4);
                            //server.Write(buff,0,4);
                            Thread.Sleep(1000);
                        }



                        while (true) 
                        {
                            var msg = reader.ReadLine();
                            writer.WriteLine("[]" + msg);
                            //writer.Flush();
                            Thread.Sleep(1000);
                        }
                      

                        //

                        //while (true)
                        //{


                        //string msg = "你好from-" + id;
                        //writer.WriteLine(msg);
                        //Thread.Sleep(1000);
                        //}
                    }

                    server.Close();
                }
            }

        }


        static int clientId = 1;
        static void StartClient()
        {
            string id = "client" + (clientId++);

            try
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None,TokenImpersonationLevel.None))
                {
                    Console.WriteLine($"[{id}]即将连接服务器。");
                    client.Connect();
                    Console.WriteLine($"[{id}]连接成功。");

                  


                    //var buff = new byte[4] { 4,3,2,1};
                    //while (true)
                    //{
                    //    client.Write(buff, 0, 4);

                    //    var buff2 = new byte[4] ;
                    //    var len = client.Read(buff2, 0, 4);
                    //    Thread.Sleep(1000);
                    //}

                    using (StreamWriter writer = new StreamWriter(client))
                    using (StreamReader reader = new StreamReader(client))
                    {
                        writer.AutoFlush = true;
                        //Task.Run(() => {
                        //    while (true)
                        //    {

                        //        var ss = reader.ReadLine();

                        //    }

                        //});

                        var buff = new byte[4] { 2, 2, 3, 4 };
                        while (true)
                        {
                            //var len = server.Read(buff, 0, 4);
                            string s = "hello world!";
                            writer.WriteLine(s);
                            Thread.Sleep(1000);
                        }
                        while (true)
                        {
                            string s = "hello world!";
                            writer.WriteLine(s);

                            var ss =   reader.ReadLine();
                            Thread.Sleep(1000);

                        }
                        //string msg = null;
                        //while ((msg = reader.ReadLine()) != null)
                        //{
                        //    Console.WriteLine($"[{id}]get message：{msg}");
                        //}
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生了异常：{ex}");
            }

        }

    }
}
