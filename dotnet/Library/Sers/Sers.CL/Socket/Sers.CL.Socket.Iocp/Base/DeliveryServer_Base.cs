//  https://freshflower.iteye.com/blog/2285272 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class DeliveryServer_Base<DeliveryConnection> : IDeliveryServer
        where DeliveryConnection : DeliveryConnection_Base, new()
    {


        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager;

        /// <summary>
        /// 服务端 监听地址。若不指定则监听所有网卡。例如： "127.0.0.1"、"sers.cloud"。
        /// </summary>
        public string host = null;

        /// <summary>
        /// 服务端 监听端口号(默认4501)。例如： 4501。
        /// </summary>
        public int port = 4501;



        /// <summary>
        /// 接收缓存区大小
        /// </summary>
        public int receiveBufferSize = 8 * 1024;


        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; }



        /// <summary>
        /// 最大连接数
        /// </summary>
        private int maxConnectCount;


        public int MaxConnCount
        {
            get { return maxConnectCount; }
            set
            {
                maxConnectCount = value;
                m_maxNumberAcceptedClients = new Semaphore(maxConnectCount, maxConnectCount);
                //pool_ReceiveEventArgs.Capacity = maxConnectCount;
            }
        }



        /// <summary>
        ///  connHashCode -> DeliveryConnection
        /// </summary>
        protected readonly ConcurrentDictionary<int, DeliveryConnection> connMap = new ConcurrentDictionary<int, DeliveryConnection>();

        public IEnumerable<IDeliveryConnection> ConnectedList => connMap.Values.Select(conn => ((IDeliveryConnection)conn));


        public DeliveryServer_Base()
        {
            MaxConnCount = 20000;
        }




        #region Start Stop


        public virtual bool Start()
        {
            Stop();

            try
            {
                //Logger.Info("[CL.DeliveryServer] Socket.Iocp,starting... host:" + host + " port:" + port);

                //(x.1)
                connMap.Clear();

                //(x.2)
                IPEndPoint localEndPoint = new IPEndPoint(String.IsNullOrEmpty(host) ? IPAddress.Any : NetHelp.ParseToIPAddress(host), port);
                listenSocket = new global::System.Net.Sockets.Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(localEndPoint);

                //(x.3)
                // start the server with a listen backlog of 100 connections
                listenSocket.Listen(maxConnectCount);
                // post accepts on the listening socket
                StartAccept(null);

                //Logger.Info("[CL.DeliveryServer] Socket.Iocp,started.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }


        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Stop()
        {     

            if (listenSocket == null) return;

            var listenSocket_ = listenSocket;
            listenSocket = null;

            //(x.1) stop conn
            ConnectedList.ToList().ForEach(Delivery_OnDisconnected);
            connMap.Clear();

            //(x.2) close Socket
            try
            {
                listenSocket_.Close();
                listenSocket_.Dispose();
                //listenSocket_.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        #endregion


        #region Iocp


        global::System.Net.Sockets.Socket listenSocket;

        Semaphore m_maxNumberAcceptedClients;


        #region ReceiveEventArgs

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        SocketAsyncEventArgs ReceiveEventArgs_Create(global::System.Net.Sockets.Socket socket)
        {
            var conn = Delivery_OnConnected(socket);

            SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);


            var buff = DataPool.BytesGet(receiveBufferSize);
            receiveEventArgs.SetBuffer(buff, 0, buff.Length);

            receiveEventArgs.UserToken = conn;
            conn.receiveEventArgs = receiveEventArgs;

            return receiveEventArgs;
        }

        //ObjectPool<SocketAsyncEventArgs> pool_ReceiveEventArgs = new ObjectPool<SocketAsyncEventArgs>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ReceiveEventArgs_Release(SocketAsyncEventArgs receiveEventArgs)
        {
            receiveEventArgs.UserToken = null;
            //pool_ReceiveEventArgs.Push(receiveEventArgs);
        }
        #endregion




        // Begins an operation to accept a connection request from the client 
        //
        // <param name="acceptEventArg">The context object to use when issuing 
        // the accept operation on the server's listening socket</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArgs.AcceptSocket = null;
            }

            m_maxNumberAcceptedClients.WaitOne();
            if (!listenSocket.AcceptAsync(acceptEventArgs))
            {
                AcceptEventArg_Completed(null, acceptEventArgs);
            }
        }

        // This method is the callback method associated with Socket.AcceptAsync 
        // operations and is invoked when an accept operation is complete
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                // Get the socket for the accepted client connection and put it into the 
                //ReadEventArg object user token
                SocketAsyncEventArgs receiveEventArgs = ReceiveEventArgs_Create(acceptEventArgs.AcceptSocket);

                if (!acceptEventArgs.AcceptSocket.ReceiveAsync(receiveEventArgs))
                {
                    ProcessReceive(receiveEventArgs);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            // Accept the next connection request
            if (acceptEventArgs.SocketError == SocketError.OperationAborted) return;
            StartAccept(acceptEventArgs);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IO_Completed(object sender, SocketAsyncEventArgs e)
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
                //    ProcessSend(e);
                //    break;
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
            //读取数据
            DeliveryConnection conn = e.UserToken as DeliveryConnection;
            if (conn == null) return;

            try
            {

                // check if the remote host closed the connection               
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据
                    conn.AppendData(new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred));

                    byte[] buffData = DataPool.BytesGet(receiveBufferSize);
                    e.SetBuffer(buffData, 0, buffData.Length);



                    // start loop
                    //继续接收. 为什么要这么写,请看Socket.ReceiveAsync方法的说明
                    if (!conn.socket.ReceiveAsync(e))
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
            conn.Close();
        }



        #endregion


    


        #region Delivery_Event

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DeliveryConnection Delivery_OnConnected(global::System.Net.Sockets.Socket socket)
        {
            var conn = new DeliveryConnection();
            conn.securityManager = securityManager;
            conn.Init(socket);

            conn.Conn_OnDisconnected = Delivery_OnDisconnected;

            connMap[conn.GetHashCode()] = conn;
            try
            {
                Conn_OnConnected?.Invoke(conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            return conn;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Delivery_OnDisconnected(IDeliveryConnection _conn)
        {
            // decrement the counter keeping track of the total number of clients connected to the server
            m_maxNumberAcceptedClients.Release();


            var conn = (DeliveryConnection)_conn;

            ReceiveEventArgs_Release(conn.receiveEventArgs);

            connMap.TryRemove(_conn.GetHashCode(), out _);

            try
            {
                Conn_OnDisconnected?.Invoke(_conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        #endregion


    }
}
