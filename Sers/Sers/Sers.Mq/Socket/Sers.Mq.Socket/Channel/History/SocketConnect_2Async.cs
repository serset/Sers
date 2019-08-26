#region << 版本注释 - v1 >>
/*
 * ========================================================================
 * 版本：v1
 * 时间：181204
 * 作者：Lith   
 * Q  Q：755944120
 * 邮箱：litsoft@126.com
 * 
 * ========================================================================
*/
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Util.Pool;
using Sers.Core.Util.Threading;

namespace Sers.Mq.Socket.Channel.Socket._2Async
{
    /// <summary>
    /// 
    /// </summary>
    public class SocketConnect : IDisposable
    {

        public SocketConnect()
        {
            taskToSendMsg = new TaskToSendMsg(this);
        }

        ~SocketConnect()
        {
            Dispose();
        }




        TcpClient client;
        System.Net.Sockets.Socket socket; 

        public bool IsConnected => (null != socket);

        public Action<Exception> OnException { get; set; }

        public Action<SocketConnect> OnDisconnected { get; set; }






        #region Init

        /// <summary>
        /// 初始化并开启后台线程
        /// </summary>
        /// <param name="client"></param>
        /// <param name="workThreadCount">后台处理消息的线程个数</param>
        /// <returns></returns>
        public bool Init(TcpClient client,int workThreadCount)
        {
            try
            {
                this.client = client;
                socket = client.Client; 
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
                return false;
            }

            taskToReceiveMsg.action = TaskToReceiveMsg;
            taskToReceiveMsg.Start();

            taskToSendMsg.Start();

            taskToDealMsg.threadCount = workThreadCount;
            taskToDealMsg.action = TaskToDealMsg;
            taskToDealMsg.Start();



            return true;
        }
        #endregion
        



        #region Close Dispose

        /// <summary>
        /// 释放使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 释放使用的所有资源
        /// </summary>
        public void Close()
        {
            if (!IsConnected) { return; }            
       
          

            #region 清理释放连接资源

            if (null != socket)
            {
                try
                {
                    socket.Close();
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);
                }
                socket = null;
            }


            try
            {
                if (null != client)
                {
                    client.Close();
                    client.Dispose();
                    client = null;
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
            #endregion


            try
            {
                OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }

            Task.Run(()=>{
                taskToReceiveMsg.Stop();
                taskToDealMsg.Stop();
                taskToSendMsg.Stop();
            });
        }
        #endregion       



        #region 收到消息 和 发送消息

        public Action<ArraySegment<byte>> OnGetMsg
        {
            get;
            set;
        }


        public void SendMsg(List<ArraySegment<byte>> data)
        {
            if(IsConnected)
                taskToSendMsg.SendMsg(data);
            //TODO:
        }
        #endregion


        #region 后台消息线程      

        #region 后台处理接收到的消息线程 TaskToDealMsg

        LongTasksHelp taskToDealMsg = new LongTasksHelp();
        ConcurrentQueue<ArraySegment<byte>> msgToDeal = new ConcurrentQueue<ArraySegment<byte>>();
        AutoResetEvent autoResetEvent_OnReveiveMsg = new AutoResetEvent(false);

        void TaskToDealMsg()
        {
            while (true)
            {
                try
                {
                    #region ThreadToDealMsg                        
                    while (true)
                    {

                        if (msgToDeal.TryDequeue(out var msg))
                        {
                            OnGetMsg(msg);
                        }
                        else
                        {
                            autoResetEvent_OnReveiveMsg.WaitOne(100);
                        }
                    }
                    #endregion
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                }
            }
        }
 
        void CahceReceivedMsg(ArraySegment<byte> msg)
        {
            msgToDeal.Enqueue(msg);
            autoResetEvent_OnReveiveMsg.Set();
        }

        #endregion




        #region 后台接收消息线程 TaskToReceiveMsg
        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();


        void TaskToReceiveMsg()
        {
            while (IsConnected)
            {
                try
                {
                    while (IsConnected)
                    {
                        CahceReceivedMsg(ReadMsg());
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    OnException?.Invoke(ex);
                }
            }

        }

        #endregion




        #region 后台发送消息线程 TaskToSendMsg

        TaskToSendMsg taskToSendMsg;
        

        class TaskToSendMsg : LongTaskHelp
        {

            AutoResetEvent mEvent = new AutoResetEvent(false);
            SocketConnect socketConnect;
            ConcurrentQueue<List<ArraySegment<byte>>> msgToSendQueue = new ConcurrentQueue<List<ArraySegment<byte>>>();

            public TaskToSendMsg(SocketConnect socketConnect)
            {
                this.socketConnect = socketConnect;
                action = Handle;
            }
         
            public void SendMsg(List<ArraySegment<byte>> data)
            {
                msgToSendQueue.Enqueue(data);
                mEvent.Set();
            }
           

            void Handle()
            {
                while (true)
                {
                    try
                    {
                        while (true)
                        {
                            while (msgToSendQueue.TryDequeue(out var data))
                            {
                                socketConnect.WriteMsg(data);                                
                            }
                            int millisecondsTimeout = 10000;
                            mEvent.WaitOne(millisecondsTimeout);
                            //mEvent.Reset();
                        }
                    }
                    catch (Exception ex) when (!(ex is ThreadInterruptedException))
                    {
                        Logger.log.Error(ex);
                    }
                }
            }
           
        }


        #endregion

        #endregion


        #region 第一层封装 ReadMsg WriteMsg 
        //线程不安全

        /*
            消息块格式：
	            第一部分(len)    数据长度，4字节 Int32类型
	            第二部分(data)   原始数据，长度由第二部分指定 
             
        */



        ArraySegment<byte> ReadMsg()
        {
            #region Method Receive
            void Receive(ArraySegment<byte> data)
            {                 
                int readedCount = 0;
                do {              
                    var t = socket.ReceiveAsync(data.Slice(readedCount, data.Count - readedCount), SocketFlags.None);
                    t.Wait();
                    readedCount+=t.Result;

                } while (readedCount < data.Count);
            }
            #endregion


            try
            {                

                var  bLen= DataPool.ArraySegmentByteGet(4);

                //(x.1)获取 第一部分(len)  
                Receive(bLen);
                Int32 len=bLen.ArraySegmentByteToInt32();

                //(x.2)获取第二部分(data)  
                var data = DataPool.ArraySegmentByteGet(len);
                Receive(data);
                return data;
            }
            catch (Exception ex)
            {
                //连接断开
                Close();
                throw;
            }
        }


        public void WriteMsg(List<ArraySegment<byte>> data)
        {
            try
            {
                Int32 len = data.ByteDataCount();

                data.Insert(0, len.Int32ToBytes());

                socket.SendAsync(data, SocketFlags.None);

            }
            catch (Exception ex)
            {
                //连接断开
                Close();
                throw;
            }
            //finally
            //{
            //    data.ReturnToPool();
            //}
        }
        #endregion

    }
}
