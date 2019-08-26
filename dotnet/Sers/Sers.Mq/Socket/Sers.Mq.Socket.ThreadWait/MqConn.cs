using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Util.Pool;
using Sers.Core.Util.Threading;

namespace Sers.Mq.Socket.ThreadWait
{
    public class MqConn: IMqConn
    {
 
        ~MqConn()
        {
            Close();
        }

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = MqConnState.waitForCertify;

        public string connTag { get; set; }


        public Action<IMqConn> Conn_OnDisconnected { get; set; }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> OnGetFrame;

 
        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {

            try
            {          
                Int32 len = data.ByteDataCount();
                data.Insert(0, len.Int32ToArraySegmentByte());
                socket.SendAsync(data, SocketFlags.None);
            }
            catch (Exception ex)
            {               
                Logger.Error(ex);
                Close();
            }            
        }
 


        public void Close()
        {
            if (socket == null) return;

            state = MqConnState.closed;

            var socket_ = socket;
            socket = null;
            try
            {
                taskToReceiveMsg.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                socket_.Close();
                socket_.Dispose();

                //socket_.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (client != null)
            {
                try
                {
                    client.Close();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                client = null;
            }

            try
            {
                Conn_OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public void Init(TcpClient client)
        {
            this.client = client;
            socket = client.Client;
 
            connectTime = DateTime.Now;  
        }

        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();
        public void StartBackThreadToReceiveMsg()
        {
            taskToReceiveMsg.Stop();
             
            taskToReceiveMsg.threadName = "Sers.Mq.Socket.ThreadWait-taskToReceiveMsg";
            taskToReceiveMsg.threadCount = 1;
            taskToReceiveMsg.action = TaskToReceiveMsg;
            taskToReceiveMsg.Start();          
        }

        void TaskToReceiveMsg()
        {
            while (socket != null)
            {
                try
                {
                    while (socket!=null)
                    {       
                        OnGetFrame(this, ReadMsg());
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }

 
        /// <summary>
        /// 通信SOCKET
        /// </summary>
        public global::System.Net.Sockets.Socket socket { get;private set; }

        TcpClient client;

        /// <summary>
        /// 连接时间
        /// </summary>
        private DateTime connectTime { get; set; }




     

        #region (x.x) socket层 封装 ReadMsg 
        //线程不安全

        /*
            消息块格式：
	            第一部分(len)    数据长度，4字节 Int32类型
	            第二部分(data)   原始数据，长度由第二部分指定 
             
        */


        internal ArraySegment<byte> ReadMsg()
        {
            #region Method Receive
            void Receive(ArraySegment<byte> data)
            {
                int readedCount = 0;
                int curCount;             
                do
                {
                    //curCount = socket.Receive(data.Slice(readedCount, data.Count - readedCount));
                    curCount = socket.Receive(data.Array, data.Offset + readedCount, data.Count - readedCount, SocketFlags.None);
                    if (curCount == 0)
                    {
                        Logger.Error("[lith_190807_002]socket is closed. connTag:" + connTag);
                        throw new Exception("[lith_190418_002]socket is closed. connTag:" + connTag);
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
                    Logger.Error("[lith_190807_003]socket read error. connTag:" + connTag);
                    throw new Exception("[lith_190505_001]socket read error. connTag:" + connTag);
                }
                if (len == 0)
                {
                    return ArraySegmentByteExtensions.Null;
                }
                var data = DataPool.ArraySegmentByteGet(len);
                Receive(data);
                return data;
            }
            catch (Exception ex) when (!(ex is ThreadInterruptedException))
            {
                //连接断开
                Task.Run((Action)Close);
                throw;
            }
        }


        #endregion




    }
}
