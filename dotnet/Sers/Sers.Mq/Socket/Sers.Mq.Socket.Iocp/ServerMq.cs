//  https://freshflower.iteye.com/blog/2285272 


using Sers.Core.Module.Log;
using Sers.Core.Util.Pool;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using Sers.Core.Module.Mq.Mq;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.Util.Net;

namespace Sers.Mq.Socket.Iocp
{
    public class ServerMq: IServerMq
    {
        /// <summary>
        /// Mq 服务端 监听地址。若不指定则监听所有网卡。例如： "127.0.0.1"、"sersms.com"。(appsettings.json :: Sers.Mq.Config.ServerMqBuilder[x].host)
        /// </summary>
        public string host = null;
        /// <summary>
        /// Mq 服务端 监听端口号。例如： 10345。(appsettings.json :: Sers.Mq.Config.ServerMqBuilder[x].port)
        /// </summary>
        public int port = 10345;



        /// <summary>
        /// 缓存区大小
        /// </summary>
        public int receiveBufferSize = 8 * 1024; 


        public Action<IMqConn> Conn_OnDisconnected { get; set; }
        public Action<IMqConn> Conn_OnConnected { get; set; }


        /// <summary>
        /// 请勿处理耗时操作，需立即返回。收到数据事件          public delegate void Conn_OnGetFrame(IMqConn token, ArraySegment&lt;byte&gt; mqMessage);
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> Conn_OnGetFrame { set; get; }
 
 
    
        public bool Start()
        {
            Stop();

            try
            {
                Logger.Info("[ServerMq] Socket.Iocp,starting... host:" + host + " port:" + port);

                connMap.Clear();

                IPEndPoint localEndPoint = new IPEndPoint(String.IsNullOrEmpty(host)?IPAddress.Any: NetHelp.ParseToIPAddress(host), port);
                listenSocket = new global::System.Net.Sockets.Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(localEndPoint);

                 
                // start the server with a listen backlog of 100 connections
                listenSocket.Listen(maxConnectCount);
                // post accepts on the listening socket
                StartAccept(null);

                Logger.Info("[ServerMq] Socket.Iocp,started.");
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
        public void Stop()
        {
            if (listenSocket == null) return;

            var listenSocket_ = listenSocket;
            listenSocket = null;

            //(x.1) stop mqConn
            ConnectedList.ToList().ForEach(MqConn_OnDisconnected);
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
       
 


  


        public ServerMq()
        {
            MaxConnCount = 20000;
        }


        /// <summary>
        /// 最大连接数
        /// </summary>
        private int maxConnectCount;


        public int MaxConnCount { get { return maxConnectCount; }
            set {
                maxConnectCount = value;
                m_maxNumberAcceptedClients = new Semaphore(maxConnectCount, maxConnectCount);
                pool_ReceiveEventArgs.Capacity = maxConnectCount;
            }
        }


        global::System.Net.Sockets.Socket listenSocket;
 
        Semaphore m_maxNumberAcceptedClients;

        /// <summary>
        ///  connGuid -> MqConnect
        /// </summary>
        public readonly ConcurrentDictionary<int, MqConn> connMap = new ConcurrentDictionary<int, MqConn>();

        public IEnumerable<IMqConn> ConnectedList => connMap.Values.Select(conn=>((IMqConn)conn));





        #region ReceiveEventArgs


        SocketAsyncEventArgs ReceiveEventArgs_Create(global::System.Net.Sockets.Socket socket)
        {
            var conn = new MqConn();
            conn.Init(socket);
            conn.OnGetFrame = Conn_OnGetFrame;   
            conn.Conn_OnDisconnected = MqConn_OnDisconnected;

            var receiveEventArgs = pool_ReceiveEventArgs.PopOrNull();
            if (receiveEventArgs == null)
            {
                receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            }

            var buff = DataPool.BytesGet(receiveBufferSize);
            receiveEventArgs.SetBuffer(buff, 0, buff.Length);
 
            receiveEventArgs.UserToken = conn;
            conn.receiveEventArgs = receiveEventArgs;

            return receiveEventArgs;
        }

        ObjectPool<SocketAsyncEventArgs> pool_ReceiveEventArgs = new ObjectPool<SocketAsyncEventArgs>();

        void ReceiveEventArgs_Release(SocketAsyncEventArgs receiveEventArgs)
        {
            receiveEventArgs.UserToken = null;
            pool_ReceiveEventArgs.Push(receiveEventArgs);
        }
        #endregion



        // Begins an operation to accept a connection request from the client 
        //
        // <param name="acceptEventArg">The context object to use when issuing 
        // the accept operation on the server's listening socket</param>
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
                AcceptEventArg_Completed(null,acceptEventArgs);
            }
        }

        // This method is the callback method associated with Socket.AcceptAsync 
        // operations and is invoked when an accept operation is complete
        //
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {          
                // Get the socket for the accepted client connection and put it into the 
                //ReadEventArg object user token
                SocketAsyncEventArgs receiveEventArgs = ReceiveEventArgs_Create(acceptEventArgs.AcceptSocket);

                MqConn mqConn = (MqConn)receiveEventArgs.UserToken;

                //if (mqConn != null)
                {
                    connMap[mqConn.GetHashCode()] = mqConn;

                    try
                    {
                        Conn_OnConnected?.Invoke(mqConn);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

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
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                //读取数据
                MqConn mqConn = (MqConn)e.UserToken;
                if (mqConn != null)
                {
                    // check if the remote host closed the connection               
                    if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                    {
                        //读取数据
                        mqConn.AppendData(new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred));

                        byte[] buffData = DataPool.BytesGet(receiveBufferSize);
                        e.SetBuffer(buffData, 0, buffData.Length);

                        // start loop
                        //继续接收. 为什么要这么写,请看Socket.ReceiveAsync方法的说明
                        if (!mqConn.socket.ReceiveAsync(e))
                            ProcessReceive(e);
                    }
                    else
                    {
                        mqConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

       

  
        private void MqConn_OnDisconnected(IMqConn mqConn)
        {
            // decrement the counter keeping track of the total number of clients connected to the server
            m_maxNumberAcceptedClients.Release();


            var conn = (MqConn)mqConn;

            ReceiveEventArgs_Release(conn.receiveEventArgs);

            connMap.TryRemove(mqConn.GetHashCode(),out _);

            try
            {
                Conn_OnDisconnected?.Invoke(mqConn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }



    }
}
