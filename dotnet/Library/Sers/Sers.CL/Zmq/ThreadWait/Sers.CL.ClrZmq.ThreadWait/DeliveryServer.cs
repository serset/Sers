using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Extensions;
using ZeroMQ;

namespace Sers.CL.ClrZmq.ThreadWait
{
    public class DeliveryServer : IDeliveryServer
    {
        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager;

        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; }
 

        /// <summary>
        ///  connGuid -> conn
        /// </summary>
        public readonly ConcurrentDictionary<long, DeliveryConnection> connMap = new ConcurrentDictionary<long, DeliveryConnection>();

        public IEnumerable<IDeliveryConnection> ConnectedList => connMap.Values.Select(conn => ((IDeliveryConnection)conn));



        /// <summary>
        /// 地址。例如： "tcp://*:4502" 、 "ipc://4502"
        /// </summary>
        public string endpoint = "tcp://*:4502";


        SocketPoller poller = new SocketPoller();


        #region Start

        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            try
            {
                Logger.Info("[CL.DeliveryServer] Zmq.ThreadWait,starting", new { endpoint });

                //(x.1) create zmq conn
                var socket = new ZSocket(ZSocketType.ROUTER);
                socket.Bind(endpoint);

                //(x.2) init poller
                poller.OnReceiveMessage = Zmq_OnReceiveMessage;
                poller.Start(socket);

                Logger.Info("[CL.DeliveryServer] Zmq.ThreadWait,started");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        void Zmq_OnReceiveMessage(ZMessage msg)
        {
            long connGuid;
            byte[] msgFrame;

            #region (x.1) get identity and msgFrame
            using (msg)
            {
                if (null == msg || msg.Count < 2) return;
                connGuid = msg[0].ReadInt64();
                msgFrame = msg[1].Read();

                #region 检测是否为 关闭命令               
                if (msg.Count > 2)
                {
                    bool getedCloseSignal = false;
                    try
                    {
                        var data = msg[2].Read();
                        if (data.Length > 0 && data[0] == 0xff)
                        {
                            getedCloseSignal = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        getedCloseSignal = true;
                    }
                    if (getedCloseSignal)
                    {
                        if (connMap.TryGetValue(connGuid, out var _conn))
                        {
                            _conn.Close();
                        }
                        else
                        {
                            SendCloseSignal(connGuid.Int64ToBytes());
                        }
                        return;
                    }
                }
                #endregion
            }
            #endregion

            #region (x.2) get or create conn
            if (!connMap.TryGetValue(connGuid, out var conn))
            {
                //新连接
                conn=Delivery_OnConnected(connGuid);
            }
            #endregion

            #region (x.3)
            conn.OnGetFrame(conn, new ArraySegment<byte>(msgFrame));
            #endregion

        }

        void Zmq_SendFrameAsync(DeliveryConnection conn, byte[] data)
        {
            poller.SendMessageAsync(new ZMessage() { new ZFrame(conn.zmqIdentity), new ZFrame(data) });
        }

        #endregion

        #region Stop

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Logger.Info("[ServerMq] Zmq.ThreadWait,stoping");

            //(x.1) stop conn
            ConnectedList.ToList().ForEach(Delivery_OnDisconnected);
            connMap.Clear();

            //(x.2)
            poller.Close();


            Logger.Info("[ServerMq] Zmq.ThreadWait,stoped");

        }
        #endregion


        #region Delivery_Event

        private DeliveryConnection Delivery_OnConnected(long connGuid)
        {
            var conn = new DeliveryConnection() { zmqIdentity = connGuid.Int64ToBytes() };

            conn.securityManager = securityManager;

            conn.OnSendFrameAsync = Zmq_SendFrameAsync;
            conn.Conn_OnDisconnected = Delivery_OnDisconnected;
            try
            {
                if (connMap.TryAdd(connGuid, conn))
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
            var conn = (DeliveryConnection)_conn;

            SendCloseSignal(conn.zmqIdentity);

            connMap.TryRemove(conn.zmqIdentity.BytesToInt64(), out _);

            try
            {
                Conn_OnDisconnected?.Invoke(_conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        private void SendCloseSignal(byte[] zmqIdentity)
        {
            try
            {
                if (poller != null)
                {
                    poller.SendMessageAsync(new ZMessage() { new ZFrame(zmqIdentity), new ZFrame(new byte[1]), new ZFrame(new byte[] { (byte)0xff }) });
                    poller.SendMessageAsync(new ZMessage() { new ZFrame(zmqIdentity), new ZFrame(new byte[1]), new ZFrame(new byte[] { (byte)0xff }) });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion
    }
}
