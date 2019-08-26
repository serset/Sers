
using Sers.Core.Module.Log;
using System;
using Sers.Core.Module.Mq.Mq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using ZeroMQ;
using Sers.Core.Extensions;
using System.Threading;

namespace Sers.Mq.Zmq.ClrZmq.ThreadWait
{
    public class ServerMq : IServerMq
    {

        public Action<IMqConn> Conn_OnDisconnected { get; set; }
        public Action<IMqConn> Conn_OnConnected { get; set; }




        /// <summary>
        /// 请勿处理耗时操作，需立即返回。收到数据事件          public delegate void Conn_OnGetFrame(IMqConn conn, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> Conn_OnGetFrame { set; get; }

        /// <summary>
        ///  connGuid -> MqConnect
        /// </summary>
        public readonly ConcurrentDictionary<long, MqConn> connMap = new ConcurrentDictionary<long, MqConn>();

        public IEnumerable<IMqConn> ConnectedList => connMap.Values.Select(conn => ((IMqConn)conn));



        /// <summary>
        /// Mq 地址。例如： "tcp://*:10346" 、 "ipc://10346"
        /// </summary>
        public string endpoint = "tcp://*:10346";


        SocketPoller poller = new SocketPoller();


        #region Start

        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            try
            {
                Logger.Info("[ServerMq] Zmq.ThreadWait,starting...   endpoint: \"" + endpoint + "\"");

                //(x.1) create zmq conn
                var socket = new ZSocket(ZSocketType.ROUTER);
                socket.Bind(endpoint);

                //(x.2) init poller
                poller.OnReceiveMessage = Zmq_OnReceiveMessage;
                poller.Start(socket);

                Logger.Info("[ServerMq] Zmq.ThreadWait,started.");
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
                        if (connMap.TryGetValue(connGuid, out var mqConn))
                        {
                            mqConn.Close();
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

            #region (x.2) get or create mqConn
            if (!connMap.TryGetValue(connGuid, out var conn))
            {
                //新连接
                conn = new MqConn() { zmqIdentity = connGuid.Int64ToBytes() };
                conn.OnSendFrameAsync = Zmq_SendFrameAsync;
                conn.Conn_OnDisconnected = MqConn_OnDisconnected;

                try
                {
                    if (connMap.TryAdd(connGuid, conn))
                        Conn_OnConnected?.Invoke(conn);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            #endregion

            #region (x.3)
            Conn_OnGetFrame(conn, new ArraySegment<byte>(msgFrame));
            #endregion

        }

        void Zmq_SendFrameAsync(MqConn conn, List<ArraySegment<byte>> data)
        {
            poller.SendMessageAsync(new ZMessage() { new ZFrame(conn.zmqIdentity), new ZFrame(data.ByteDataToBytes()) });
        }

        #endregion

        #region Stop

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Logger.Info("[ServerMq] Zmq.ThreadWait,stop...");

            //(x.1) stop mqConn
            ConnectedList.ToList().ForEach(MqConn_OnDisconnected);
            connMap.Clear();

            //(x.2)
            poller.Close();


            Logger.Info("[ServerMq] Zmq.ThreadWait,stoped");

        }
        #endregion


        #region MqConn_OnDisconnected

        private void MqConn_OnDisconnected(IMqConn mqConn)
        {
            var conn = (MqConn)mqConn;

            SendCloseSignal(conn.zmqIdentity);

            connMap.TryRemove(conn.zmqIdentity.BytesToInt64(), out _);

            try
            {
                Conn_OnDisconnected?.Invoke(mqConn);
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
                    Thread.Sleep(10);
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
