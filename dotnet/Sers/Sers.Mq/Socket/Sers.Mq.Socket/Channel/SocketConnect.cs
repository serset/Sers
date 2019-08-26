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
using System.Net.Sockets;
using System.Threading;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Util.Pool;
using Sers.Core.Util.Threading;

namespace Sers.Mq.Socket.Channel.Socket
{
    /// <summary>
    /// 
    /// </summary>
    public class SocketConnect : IDisposable
    {

        #region (x.1) 成员

        
        public SocketConnect()
        {     
        }

        ~SocketConnect()
        {
            Dispose();
        }



        #region TcpClientIsConnected
        public static bool TcpClientIsConnected(TcpClient c, int microSeconds = 500)
        {
            return null != c && c.Client.Connected && !(c.Client.Poll(microSeconds, SelectMode.SelectRead) && (c.Client.Available == 0));
        }
        #endregion

        #region TestIsConnectedByPoll
        public bool TestIsConnectedByPoll(int microSeconds = 500)
        {
            TcpClient c = client;
            return null != c && c.Client.Connected && !(c.Client.Poll(microSeconds, SelectMode.SelectRead) && (c.Client.Available == 0));
        }
        #endregion


        TcpClient client;

        System.Net.Sockets.Socket socket;
        public bool IsConnected => (null != socket); 

 
 

        public Action<SocketConnect> OnDisconnected { get; set; }

        #endregion




        #region (x.2)Init

        /// <summary>
        /// 初始化并开启后台线程
        /// </summary>
        /// <param name="client"></param>
        /// <param name="workThreadCount">后台处理消息的线程个数</param>
        /// <returns></returns>
        public bool Init(TcpClient client, int workThreadCount)
        {
            try
            {
                this.client = client;
                socket = client.Client;

                taskToReceiveMsg.action = TaskToReceiveMsg;
                taskToReceiveMsg.Start();

                taskToDealMsg.threadCount = workThreadCount;
                taskToDealMsg.action = TaskToDealMsg;
                taskToDealMsg.Start();
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
           
        }
        #endregion




        #region (x.3)Close Dispose

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

            //(x.1)
            taskToReceiveMsg.Stop();
            taskToDealMsg.Stop();


            #region (x.2)清理释放连接资源


            if (null != socket)
            {
                try
                {
                    socket.Close();
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
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
                Logger.Error(ex);
            }
            #endregion


            //(x.3)
            try
            {
                OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion       



        #region (x.4)收到消息 和 发送消息

        public Action<ArraySegment<byte>> OnGetMsg
        {
            get;
            set;
        }


        public void SendMsg(List<ArraySegment<byte>> data)
        {
            if (!IsConnected)
            {
                //TODO:
                throw new Exception("[lith_190418_001]socket is closed");
            }

            WriteMsg(data);
        }
        #endregion


        #region (x.5)后台消息线程      

        #region (x.x.1)后台处理接收到的消息线程 TaskToDealMsg

        LongTaskHelp taskToDealMsg = new LongTaskHelp();
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

        void CacheReceivedMsg(ArraySegment<byte> msg)
        {
            msgToDeal.Enqueue(msg);
            autoResetEvent_OnReveiveMsg.Set();
        }

        #endregion




        #region (x.x.2)后台接收消息线程 TaskToReceiveMsg
        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();


        void TaskToReceiveMsg()
        {
            while (IsConnected)
            {
                try
                {
                    while (IsConnected)
                    {
                        CacheReceivedMsg(ReadMsg());
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }

        }

        #endregion




        #endregion


        #region (x.6)第一层封装 ReadMsg WriteMsg 
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
                int curCount;
                do
                {
                    //curCount = socket.Receive(data.Slice(readedCount, data.Count - readedCount));
                    curCount = socket.Receive(data.Array, data.Offset + readedCount, data.Count - readedCount,SocketFlags.None);
                    if (curCount == 0)
                    {
                        throw new Exception("[lith_190418_002]socket is closed.");
                    }
                    readedCount += curCount;
                    //var t = socket.ReceiveAsync(data.Slice(readedCount, data.Count - readedCount), SocketFlags.None);
                    //t.Wait();
                    //readedCount += t.Result;

                } while (readedCount < data.Count);
            }

            #endregion


            try
            {

                var bLen = DataPool.ArraySegmentByteGet(4);

                //(x.1)获取 第一部分(len)  
                Receive(bLen);
                Int32 len = bLen.ArraySegmentByteToInt32();

                //(x.2)获取第二部分(data)  
                if (len < 0)
                {
                    throw new Exception("[lith_190505_001]socket read error.");
                }
                if (len==0)
                {
                    return ArraySegmentByteExtensions.Null;
                }
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

        void WriteMsg(List<ArraySegment<byte>> data)
        {
            try
            {
                Int32 len = data.ByteDataCount();

                data.Insert(0, len.Int32ToArraySegmentByte());

                socket.SendAsync(data, SocketFlags.None);
            }
            catch (Exception ex)
            {
                //连接断开
                Close();
                throw;
            }
        }
        #endregion

    }
}
