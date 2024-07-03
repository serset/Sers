// https://freshflower.iteye.com/blog/2285286

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Core.Util.Net;
using Vit.Core.Util.Pool;

namespace Sers.CL.Socket.Iocp.Base
{
    public class DeliveryClient_Base<DeliveryConnection> : IDeliveryClient
        where DeliveryConnection : DeliveryConnection_Base, new()
    {
        public virtual DeliveryConnection NewConnection()
        {
            var conn = _conn;
            conn.securityManager = securityManager;
            return conn;
        }


        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager;

        protected DeliveryConnection _conn = new DeliveryConnection();
        public IDeliveryConnection conn => _conn;


        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set { _conn.OnGetFrame = value; } }


        public Action<IDeliveryConnection> Conn_OnDisconnected { set => _conn.Conn_OnDisconnected = value; }





        /// <summary>
        ///  服务端 host地址（默认 "127.0.0.1" ）。例如： "127.0.0.1"、"sers.cloud"。
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// 服务端 监听端口号（默认4501）。例如： 4501。
        /// </summary>
        public int port = 4501;


        /// <summary>
        /// 接收缓存区大小（单位:byte,默认：8192）
        /// </summary>
        public int receiveBufferSize = 8 * 1024;


        public DeliveryClient_Base()
        {
            receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
        }






        #region Connect Close



        public virtual bool Connect()
        {
            try
            {

                //(x.1) Instantiates the endpoint and socket.
                var hostEndPoint = new IPEndPoint(NetHelp.ParseToIPAddress(host), port);
                socket = new global::System.Net.Sockets.Socket(hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


                var conn = NewConnection();
                _conn.receiveEventArgs = receiveEventArgs;
                _conn.Init(socket);

                var buff = DataPool.BytesGet(receiveBufferSize);
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

                if (connectArgs.SocketError != SocketError.Success)
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }


        public virtual void Close()
        {
            if (null == _conn) return;
            var conn = _conn;
            _conn = null;
            conn.Close();
        }
        #endregion



        #region Iocp


        readonly SocketAsyncEventArgs receiveEventArgs;


        private global::System.Net.Sockets.Socket socket = null;


        // Signals a connection.
        private AutoResetEvent autoResetEvent_OnConnected = new AutoResetEvent(false);


        // Calback for connect operation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                    {
                        Task.Run(() =>
                        {
                            ProcessReceive(e);
                        });
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Close();
        }

        #endregion









    }
}
