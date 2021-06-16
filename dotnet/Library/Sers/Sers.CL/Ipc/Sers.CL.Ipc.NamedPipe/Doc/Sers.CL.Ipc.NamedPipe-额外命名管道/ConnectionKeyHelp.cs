using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Ipc.NamedPipe
{
    public static class ConnectionKeyHelp
    {




        public static void Publish(Func<string> beforeConnect, string pipeName = "Sers.Ipc")
        {

            using (NamedPipeServerStream server = new NamedPipeServerStream(pipeName, PipeDirection.Out))
            {
                //Console.WriteLine($"[{id}]服务器管道已创建。");

                // 等待客户端的连接
                server.WaitForConnection();

                //Console.WriteLine($"[{id}]正在等待客户端连接......");

                var connKey = beforeConnect();
                Logger.Info("[Ipc]服务端已创建，connKey：" + connKey);

                using (StreamWriter writer = new StreamWriter(server))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine(connKey);
                }
                server.Close();
            }

        }







        /// <summary>
        ///  
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="serverName"></param>
        /// <param name="msTimeout"></param>
        /// <returns> connectionKey </returns>
        public static string Subscribe(string pipeName,string serverName=".",int msTimeout=10000) 
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream(serverName, pipeName, PipeDirection.In))
                {
                    //Console.WriteLine($"[{id}]即将连接服务器。");

                    var task=client.ConnectAsync();

                    task.Wait(msTimeout);

                    if (!task.IsCompleted) 
                    {
                        return null;
                    }
              

                    using (StreamReader reader = new StreamReader(client))
                    {
                        string connectionKey = reader.ReadLine();
                        return connectionKey;

                    }
                    //client.Close();
                    return null;
                }

            }
        
       


    }
}
