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
    ///    BeginRead  https://blog.csdn.net/xutingzhou/article/details/8184266
    /// </summary>
    class Pipe3
    {

        class StreamReader 
        {
            public PipeStream stream;


            public Action<byte[]> OnReadData;


            //定义异步读取状态类
            class AsyncState
            {
                public PipeStream FS { get; set; }
                public byte[] Buffer { get; set; }
                public ManualResetEvent EvtHandle { get; set; }
            }
            static int bufferSize = 512; 

            public void Start() 
            {
                var buffer = new byte[bufferSize];
                //构造BeginRead需要传递的状态
                var asyncState = new AsyncState { FS = stream, Buffer = buffer, EvtHandle = new ManualResetEvent(false) };
                //异步读取
                var asyncResult = stream.BeginRead(buffer, 0, bufferSize, new AsyncCallback(AsyncReadCallback), asyncState);                
            }
 

            //异步读取回调处理方法
            void AsyncReadCallback(IAsyncResult asyncResult)
            {
                var asyncState = (AsyncState)asyncResult.AsyncState;
                int readCn = asyncState.FS.EndRead(asyncResult);
                //判断是否读到内容
                if (readCn > 0)
                {
                    byte[] buffer;
                    if (readCn == bufferSize) buffer = asyncState.Buffer;
                    else
                    {
                        buffer = new byte[readCn];
                        Array.Copy(asyncState.Buffer, 0, buffer, 0, readCn);
                    }
                    //输出读取内容值
                    OnReadData(buffer);
                }

                asyncState.Buffer = new byte[bufferSize];
                //再次执行异步读取操作
                asyncResult = asyncState.FS.BeginRead(asyncState.Buffer, 0, bufferSize, new AsyncCallback(AsyncReadCallback), asyncState);
            }
     


        }


        class StreamWriter
        {
            public PipeStream stream;


            public Action<byte[]> OnReadData;


            //定义异步读取状态类
            class AsyncState
            {
                public PipeStream FS { get; set; }
                public byte[] Buffer { get; set; }
                public ManualResetEvent EvtHandle { get; set; }
            }
       

            public void Write(byte[] buffer)
            {
             
                //构造BeginRead需要传递的状态
                var asyncState = new AsyncState { FS = stream, Buffer = buffer, EvtHandle = new ManualResetEvent(false) };
                //异步读取
                var asyncResult = stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(AsyncReadCallback), asyncState);
            }


            //异步读取回调处理方法
            void AsyncReadCallback(IAsyncResult asyncResult)
            {
                var asyncState = (AsyncState)asyncResult.AsyncState;
                asyncState.FS.EndWrite(asyncResult);
            }



        }



        static void Main1(string[] args)
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


                    new StreamReader { stream = server, OnReadData = (byte[] data) => {

                        Console.WriteLine("getdata");

                    } }.Start();


                    var writer = new StreamWriter
                    {
                        stream = server

                    };


                    var buff = new byte[4] { 2, 2, 3, 4 };
                    while (true)
                    {
                        writer.Write(buff);
                        //var len = server.Read(buff, 0, 4);
                        //client.Write(buff, 0, 4);
                        //client.Flush();
                        Thread.Sleep(1000);
                    }



                    //var buff = new byte[4] { 1,2,3,4};
                    //while (true)
                    //{
                    //    //var len = server.Read(buff, 0, 4);
                    //    //server.Write(buff,0,4);
                    //    Thread.Sleep(1000);
                    //}


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
                using (NamedPipeClientStream client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    Console.WriteLine($"[{id}]即将连接服务器。");
                    client.Connect();
                    Console.WriteLine($"[{id}]连接成功。");

                    //Task.Run(() => {
                    //    while (true)
                    //    {

                    //        var buff1 = new byte[4];
                    //        var len = client.Read(buff1, 0, 4);

                    //    }

                    //});


                    new StreamReader
                    {
                        stream = client,
                        OnReadData = (byte[] data) => {

                            Console.WriteLine("getdata from server");

                        }
                    }.Start();





                    var writer=new StreamWriter
                    {
                        stream = client
                         
                    } ;


                    var buff = new byte[4] { 2, 2, 3, 4 };
                    while (true)
                    {
                        writer.Write(buff);
                        //var len = server.Read(buff, 0, 4);
                        //client.Write(buff, 0, 4);
                        //client.Flush();
                        Thread.Sleep(1000);
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
