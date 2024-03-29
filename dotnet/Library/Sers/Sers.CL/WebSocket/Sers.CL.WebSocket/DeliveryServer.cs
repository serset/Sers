﻿// https://blog.csdn.net/ZslLoveMiwa/article/details/80247739

// https://www.oschina.net/translate/websocket-libraries-comparison-2

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Fleck;

using Newtonsoft.Json;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;

namespace Sers.CL.WebSocket
{
    public class DeliveryServer : IDeliveryServer
    {
        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager;

        /// <summary>
        /// server address(default: "ws://0.0.0.0:4503")
        /// </summary>
        public string host = "ws://0.0.0.0:4503";

        /// <summary>
        /// 
        /// </summary>
        public CertificateInfo certificate;

        #region CertificateInfo
        [JsonObject(MemberSerialization.OptIn)]
        public class CertificateInfo
        {
            /// <summary>
            /// Data/ssl.pfx
            /// </summary>
            [JsonProperty]
            public string filePath { get; set; }

            [JsonProperty]
            public string password { get; set; }

        }
        #endregion

        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; }



        public bool Start()
        {
            Stop();

            try
            {
                Logger.Info("[CL.DeliveryServer] WebSocket,starting", new { host });

                connMap.Clear();

                listenSocket = new WebSocketServer(host);

                if (certificate != null)
                {
                    listenSocket.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.filePath, certificate.password,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                }

                //出错后进行重启
                listenSocket.RestartAfterListenError = true;

                #region Start
                //开始监听
                listenSocket.Start(socket =>
                {
                    //获取客户端网页的url
                    //string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;   

                    var conn = new DeliveryServer_Connection();
                    conn.securityManager = securityManager;
                    conn.Init(socket);

                    socket.OnError = (ex) =>
                    {
                        Logger.Error(ex);
                        conn.Close();
                    };
                    socket.OnOpen = () =>
                    {
                        Delivery_OnConnected(conn);
                    };
                    socket.OnClose = () =>
                    {
                        Delivery_OnDisconnected(conn);
                    };
                    socket.OnBinary = bytes =>
                    {
                        conn.AppendData(new ArraySegment<byte>(bytes, 0, bytes.Length));
                    };
                    //socket.OnMessage = message =>   
                    //{                        
                    //};
                });
                #endregion

                Logger.Info("[CL.DeliveryServer] WebSocket,started");
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

            //(x.1) stop conn
            ConnectedList.ToList().ForEach(Delivery_OnDisconnected);
            connMap.Clear();

            //(x.2) close Socket
            try
            {
                listenSocket_.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }







        public DeliveryServer()
        {
        }




        WebSocketServer listenSocket;



        /// <summary>
        ///  connHashCode -> DeliveryConnection
        /// </summary>
        readonly ConcurrentDictionary<int, DeliveryServer_Connection> connMap = new ConcurrentDictionary<int, DeliveryServer_Connection>();




        public IEnumerable<IDeliveryConnection> ConnectedList => connMap.Values.Select(conn => ((IDeliveryConnection)conn));



        #region Delivery_Event

        private DeliveryServer_Connection Delivery_OnConnected(DeliveryServer_Connection conn)
        {

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

        private void Delivery_OnDisconnected(IDeliveryConnection _conn)
        {
            var conn = (DeliveryServer_Connection)_conn;

            connMap.TryRemove(conn.GetHashCode(), out _);

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
