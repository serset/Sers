using System;
using System.Threading;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Worker;
using Vit.Extensions.Json_Extensions;

namespace Sers.CL.Ipc.SharedMemory.Stream
{
    public class ReadStream
    {
        public IDeliveryConnection conn;

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件          public delegate void OnReceiveData(AsyncUserToken token, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnReceiveMessage { set; private get; }

        public Action<IDeliveryConnection> OnDisconnected { set; private get; }

        public ReadStream()
        {
        }
        ~ReadStream()
        {
            Stop();
        }



        global::SharedMemory.CircularBuffer buffer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">共享内存名称</param>
        /// <param name="nodeCount">共享内存节点个数</param>
        /// <param name="nodeBufferSize">共享内存节点大小</param>
        /// <returns></returns>
        public bool SharedMemory_Malloc(string name = "", int nodeCount = 64, int nodeBufferSize = 10)
        {
            try
            {
                if (buffer != null) return false;
                buffer = Util.SharedMemory_Malloc(name: name, nodeCount: nodeCount, nodeBufferSize: nodeBufferSize);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
           
        }

        public bool SharedMemory_Attach(string name = "")
        {
            if (buffer != null) return false;
            try
            {
                buffer = Util.SharedMemory_Attach(name);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }


        public bool Start()
        {
            if (buffer == null) return false;

            // start receiveMsg Thread
            receiveMsg_Thread.threadName = "CL-Ipc-SharedMemory-receiveMsg-"+ buffer.Name;
            receiveMsg_Thread.threadCount = 1;
            receiveMsg_Thread.Processor = ReceiveMsg_Thread;
            receiveMsg_Thread.Start();


            return true;
        }

 
      

        public void Stop()
        {
            if (buffer == null) return;

            receiveMsg_Thread.Stop();

            buffer.Close();

            buffer = null;

            try
            {
                OnDisconnected?.Invoke(conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }



        #region ReceiveMsg_Thread

        LongThread receiveMsg_Thread = new LongThread();

        void ReceiveMsg_Thread()
        {
            byte[] baLen = new byte[4];

            while (true)
            {
                try
                {
                    while (true)
                    {
                        //(x.1) get msg len
                        while (0 == buffer.Read(baLen)) ;
                        int len = baLen.BytesToInt32();
                        if (len < 0)
                        {
                            Stop();
                            return;
                        }

                        #region (x.2) receive msg                           
                        byte[] data = new byte[len];
                        int receivedLen = 0;
                        while (receivedLen < len)
                        {
                            receivedLen += buffer.Read(data, receivedLen);
                        }
                        #endregion

                        //(x.3) deal msg
                        OnReceiveMessage(conn, new ArraySegment<byte>(data));

                    }
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }

        #endregion

    }
}
