using System;
using System.Threading;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using Vit.Extensions.Json_Extensions;

using ZeroMQ;

namespace Sers.CL.ClrZmq.ThreadWait
{
    public class DeliveryClient : IDeliveryClient
    {


        DeliveryConnection _conn = new DeliveryConnection() { zmqIdentity = CommonHelp.NewFastGuidLong().Int64ToBytes() };

        public IDeliveryConnection conn => _conn;




        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set => _conn.OnGetFrame = value; }

        public Action<IDeliveryConnection> Conn_OnDisconnected { set => _conn.Conn_OnDisconnected = value; }





        SocketPoller poller = new SocketPoller();


        /// <summary>
        /// 地址。例如： "tcp://127.0.0.1:4502" 、 "ipc://4502"
        /// </summary>
        public string endpoint = "tcp://127.0.0.1:4502";

        public bool Connect()
        {

            try
            {

                Logger.Info("[CL.DeliveryClient] Zmq.ThreadWait,connecting", new { endpoint });

                //(x.1)
                _conn.OnSendFrameAsync = Zmq_SendMessageAsync;

                //(x.2) create zmq conn
                var socket = new ZSocket(ZSocketType.DEALER);
                socket.Identity = _conn.zmqIdentity;
                socket.Connect(endpoint);


                //(x.3) init poller
                poller.OnReceiveMessage = Zmq_OnReceiveMessage;
                poller.Start(socket);
                Logger.Info("[CL.DeliveryClient] Zmq.ThreadWait,connected");
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
            byte[] msgFrame;
            using (msg)
            {
                if (null == msg || msg.Count == 0) return;

                msgFrame = msg[0].Read();

                #region 检测是否为 关闭命令               
                if (msg.Count > 1)
                {
                    bool getedCloseSignal = false;
                    try
                    {
                        var data = msg[1].Read();
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
                        Close();
                        return;
                    }
                }
                #endregion               
            }

            _conn.OnGetFrame(_conn, msgFrame.BytesToArraySegmentByte());
        }

        void Zmq_SendMessageAsync(DeliveryConnection conn, byte[] data)
        {
            poller.SendMessageAsync(new ZMessage() { new ZFrame(data) });
        }




        public void Close()
        {
            if (_conn == null) return;

            SendCloseSignal();

            var conn = _conn;
            _conn = null;

            try
            {
                poller.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            poller = null;

            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        private void SendCloseSignal()
        {
            try
            {
                if (poller != null)
                {
                    poller.SendMessageAsync(new ZMessage() { new ZFrame(new byte[1]), new ZFrame(new byte[] { (byte)0xff }) });
                    poller.SendMessageAsync(new ZMessage() { new ZFrame(new byte[1]), new ZFrame(new byte[] { (byte)0xff }) });
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

















    }
}
