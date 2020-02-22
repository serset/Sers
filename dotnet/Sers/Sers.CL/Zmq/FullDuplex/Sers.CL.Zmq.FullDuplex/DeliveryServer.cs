using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sers.CL.Zmq.FullDuplex.Zmq;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;
using Vit.Extensions;
 
namespace Sers.CL.Zmq.FullDuplex
{
    public class DeliveryServer : IDeliveryServer
    {

        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; } 

        /// <summary>
        ///  connGuid -> conn
        /// </summary>
        public readonly ConcurrentDictionary<long, DeliveryConnection> connMap = new ConcurrentDictionary<long, DeliveryConnection>();

        public IEnumerable<IDeliveryConnection> ConnectedList => connMap.Values.Select(conn => ((IDeliveryConnection)conn));



        /// <summary>
        /// 地址。例如： "tcp://*:4504" 、 "ipc://4504"
        /// </summary>
        public string endpoint = "tcp://*:4504";
        ZSocket socketRouter;
        SocketStream stream = new SocketStream();
 
        #region Start

        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            try
            {
                Logger.Info("[CL.DeliveryServer] Zmq.FullDuplex,starting...   endpoint: \"" + endpoint + "\"");

                //(x.1) create zmq conn
                socketRouter = new ZSocket(ZSocketType.ROUTER);
                socketRouter.Bind(endpoint);


                //(x.2) create zmq socketWriter
                var socketWriter = new ZSocket(ZSocketType.DEALER);
                socketWriter.Identity = ((long) 0).Int64ToBytes();
                socketWriter.Connect(endpoint.Replace("*", "127.0.0.1"));


                //(x.3) Start stream
                stream.OnReceiveMessage = OnReceiveMessage;
                stream.BeforeStop = () => 
                {
                    Logger.Info("[CL.DeliveryServer] Zmq.FullDuplex,stop...");

                    //stop conn
                    ConnectedList.ToList().ForEach(conn => conn.Close());
                    connMap.Clear();

                    //以防 关闭命令 没有发送出去
                    Thread.Sleep(100);
                };
                stream.AfterStop = () => {            Logger.Info("[CL.DeliveryServer] Zmq.FullDuplex,stoped");    };
                stream.Start(socketRouter, socketWriter);
                Logger.Info("[CL.DeliveryServer] Zmq.FullDuplex,started.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        void OnReceiveMessage(List<byte[]> msg)
        {
            if (null == msg || msg.Count < 2) return;

            long identityOfWriter  = msg[0].BytesToInt64();

            if (identityOfWriter != 0)
            {
                #region (x.1)reader

                byte[] msgFrame = msg[1];

                #region (x.1)检测是否为 关闭命令               
                if (msg.Count > 2)
                {
                    bool getedCloseSignal = false;
                    try
                    {
                        var data = msg[2];
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
                        if (connMap.TryGetValue(identityOfWriter, out var _conn))
                        {
                            _conn.Close();
                        }
                        return;
                    }
                }
                #endregion

                #region (x.2) get or create conn
                if (!connMap.TryGetValue(identityOfWriter, out var conn))
                {
                    //新连接
                    conn = new DeliveryConnection();
                    conn.SetIdentity(identityOfWriter>>1);
                    conn.OnSendFrameAsync = SendMessageAsync;
                    conn.Conn_OnDisconnected = Delivery_OnDisconnected;
                    try
                    {
                        if (connMap.TryAdd(identityOfWriter, conn))
                            Conn_OnConnected?.Invoke(conn);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                #endregion

                #region (x.3)
                conn.OnGetFrame(conn, new ArraySegment<byte>(msgFrame));
                #endregion

                #endregion
            }
            else
            {
                #region (x.2)writer
                msg.RemoveAt(0);
                socketRouter.SendMessage(msg.ToArray());
                #endregion
            }   

        }

        void SendMessageAsync(DeliveryConnection conn, List<ArraySegment<byte>> data)
        {
            stream.SendMessageAsync( conn.identityOfReader, data.ByteDataToBytes()  );
        }

        void SendCloseSignal(DeliveryConnection conn)
        {
            stream.SendMessageAsync(conn.identityOfReader, new byte[1], new byte[] { (byte)0xff });
            stream.SendMessageAsync(conn.identityOfReader, new byte[1], new byte[] { (byte)0xff });
        }

        #endregion

        #region Stop

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            stream.Stop();
        }


        #endregion


        #region Delivery_OnDisconnected

        private void Delivery_OnDisconnected(IDeliveryConnection _conn)
        {
            var conn = (DeliveryConnection)_conn;

            SendCloseSignal(conn);

            connMap.TryRemove(conn.identityOfWriter.BytesToInt64(), out _);

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
