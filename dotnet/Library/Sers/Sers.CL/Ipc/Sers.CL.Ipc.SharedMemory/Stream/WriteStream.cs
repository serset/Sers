using System;
using System.Collections.Concurrent;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace Sers.CL.Ipc.SharedMemory.Stream
{
    public class WriteStream
    {

        //public Action  OnDisconnected;
        public WriteStream(int boundedCapacity= 10000000)
        {
            msgToSend = new BlockingCollection<byte[]>(boundedCapacity);
        }
        ~WriteStream()
        {
            Stop();
        }

        readonly BlockingCollection<byte[]> msgToSend;

        global::SharedMemory.CircularBuffer buffer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">共享内存名称</param>
        /// <param name="nodeCount">共享内存节点个数</param>
        /// <param name="nodeBufferSize">共享内存节点大小</param>
        /// <returns></returns>
        public bool SharedMemory_Malloc(string name = "", int nodeCount = 64, int nodeBufferSize = 10240)
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
            sendMsg_Thread.threadName = "CL-Ipc-SharedMemory-sendMsg-" + buffer.Name;
            sendMsg_Thread.threadCount = 1;
            sendMsg_Thread.action = SendMsg_Thread;
            sendMsg_Thread.Start();

            return true;
        }

     

       

        public void SendMessageAsync(byte[] msg)
        {
            msgToSend.Add(msg);
        }


        public void Stop()
        {
            if (buffer == null) return;

            //(x.1) stop thread
            sendMsg_Thread.Stop();

            //(x.2)write stop signal
            try
            {
                var baLen = (-1).Int32ToBytes();
                while (0 == buffer.Write(baLen)) ;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            //(x.3)close
            try
            {
                buffer.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            buffer = null;

            //(x.4)Invoke event
            //try
            //{
            //    OnDisconnected?.Invoke();
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex);
            //}
        }



        #region SendMsg_Thread

        LongTaskHelp sendMsg_Thread = new LongTaskHelp();

        void SendMsg_Thread()
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        //(x.1) get msg to send
                        byte[] msg = msgToSend.Take();

                        #region (x.2) send msg 

                        //(x.x.1) send msg len
                        int len = msg.Length;
                        var baLen = len.Int32ToBytes();
                        while (0 == buffer.Write(baLen)) ;

                        //(x.x.2) send msg body
                        int sendedLen = 0;
                        while (sendedLen < len)
                        {
                            sendedLen += buffer.Write(msg, sendedLen);
                        }
                        #endregion
                    }
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                    //Stop();
                    //return;
                }
            }
        }

        #endregion


    }
}
