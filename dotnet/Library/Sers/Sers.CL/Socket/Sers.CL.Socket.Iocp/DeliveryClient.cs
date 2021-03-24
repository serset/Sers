// https://freshflower.iteye.com/blog/2285286

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Net;
using Vit.Core.Util.Pool;

namespace Sers.CL.Socket.Iocp
{
    public class DeliveryClient: IDeliveryClient
    {


        DeliveryConnection _conn = new DeliveryConnection();
        public IDeliveryConnection conn => _conn;

        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set { _conn.OnGetFrame = value; }  }


        public Action<IDeliveryConnection> Conn_OnDisconnected { set => _conn.Conn_OnDisconnected = value; }





        readonly SocketAsyncEventArgs receiveEventArgs ;

        /// <summary>
        /// 缓存区大小
        /// </summary>
        public int receiveBufferSize = 8*1024;


        public DeliveryClient()
        {
       
            _conn.receiveEventArgs= receiveEventArgs = new SocketAsyncEventArgs();

            receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
 

        }








        /// <summary>
        ///  服务端 host地址（默认 "127.0.0.1" ）。例如： "127.0.0.1"、"sers.cloud"。
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// 服务端 监听端口号（默认4501）。例如： 4501。
        /// </summary>
        public int port = 4501;
        public bool Connect()
        {
            try
            {
                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connecting... host:" + host + " port:" + port);

                //(x.1) Instantiates the endpoint and socket.
                var hostEndPoint = new IPEndPoint(NetHelp.ParseToIPAddress(host), port);
                socket = new global::System.Net.Sockets.Socket(hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


                _conn.Init(socket);

                var buff= DataPool.BytesGet(receiveBufferSize);
                _conn.receiveEventArgs.SetBuffer(buff, 0, buff.Length);


                //(x.2)
                SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
                connectArgs.RemoteEndPoint = hostEndPoint;
                connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);

                autoResetEvent_OnConnected.Reset();
                socket.ConnectAsync(connectArgs);


                //(x.3) 阻塞. 让程序在这里等待,直到连接响应后再返回连接结果
                if (!autoResetEvent_OnConnected.WaitOne(10000))
                    return false;

                if (connectArgs.SocketError == SocketError.Success)
                {
                    Logger.Info("[CL.DeliveryClient] Socket.Iocp,connected.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

    
        public void Close()
        {
            if (null == _conn) return;
            var conn = _conn;
            _conn = null;
            conn.Close();
        }
 

 
        private global::System.Net.Sockets.Socket socket=null;
 

        // Signals a connection.
        private AutoResetEvent autoResetEvent_OnConnected = new AutoResetEvent(false);
      

        // Calback for connect operation
        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            // Signals the end of connection.
            autoResetEvent_OnConnected.Set(); //释放阻塞.

            //如果连接成功,则初始化socketAsyncEventArgs
            if (e.SocketError == SocketError.Success)
            {
                //启动接收,不管有没有,一定得启动.否则有数据来了也不知道.
                if (!socket.ReceiveAsync(receiveEventArgs))
                    ProcessReceive(receiveEventArgs);
            }
            else
            {
                Close();
            }

        }
 
 

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        { 
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    Logger.Info("[Iocp]IO_Completed Send");
                    return;

                    //ProcessSend(e);
                    //break;
                default:
                    Logger.Info("[Iocp]IO_Completed default");
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        // This method is invoked when an asynchronous receive operation completes. 
        // If the remote host closed the connection, then the socket is closed.  
        // If data was received then the data is echoed back to the client.
        //
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection 
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据                  
                    _conn.AppendData(new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred));

                    byte[] buffData = DataPool.BytesGet(receiveBufferSize);
                    e.SetBuffer(buffData, 0, buffData.Length);

                    // start loop
                    if (!socket.ReceiveAsync(e))
                        ProcessReceive(e);
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Close();
        }




 




    }
}
