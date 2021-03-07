using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pool;
using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace Sers.CL.Socket.ThreadWait
{
    public class DeliveryConnection: IDeliveryConnection
    {
 
        ~DeliveryConnection()
        {
            Close();
        }

        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _securityManager = value; }
        Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;


        public Action<IDeliveryConnection > Conn_OnDisconnected { get; set; }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { private get; set; }


        public void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            if (data == null || socket == null) return;
            try
            {
                Int32 len = data.Count();
                data.Insert(0, len.Int32ToArraySegmentByte());

                var bytes = data.ToBytes();

                _securityManager?.Encryption(new ArraySegment<byte>(bytes, 4, bytes.Length - 4));       

                socket.SendAsync(bytes.BytesToArraySegmentByte(), SocketFlags.None);
                //socket.SendAsync(data, SocketFlags.None);
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

            state = DeliveryConnState.closed;

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



        #region taskToReceiveMsg       

        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();
        public void StartBackThreadToReceiveMsg()
        {
            taskToReceiveMsg.Stop();
             
            taskToReceiveMsg.threadName = "Sers.CL.Socket.ThreadWait-taskToReceiveMsg";
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
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion



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
                        Logger.Error("[lith_190807_002]socket is closed.");
                        throw new Exception("[lith_190418_002]socket is closed." );
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
                    Logger.Error("[lith_190807_003]socket read error.");
                    throw new Exception("[lith_190505_001]socket read error.");
                }
                if (len == 0)
                {
                    return ArraySegmentByteExtensions.Null;
                }
                var data = DataPool.ArraySegmentByteGet(len);
                Receive(data);
                _securityManager?.Decryption(data);
                return data;
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                //连接断开
                //Task.Run((Action)Close);
                Close();
                throw;
            }
        }


        #endregion




    }
}
