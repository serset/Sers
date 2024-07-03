using System;
using System.Collections.Generic;
using System.Threading;

using Sers.CL.Zmq.FullDuplex.Zmq;
using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using Vit.Extensions.Json_Extensions;

namespace Sers.CL.Zmq.FullDuplex
{
    public class DeliveryClient : IDeliveryClient
    {

        public DeliveryClient()
        {
            _conn.SetIdentity(CommonHelp.NewFastGuidLong());
        }

        DeliveryConnection _conn = new DeliveryConnection();

        public IDeliveryConnection conn => _conn;




        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set => _conn.OnGetFrame = value; }

        public Action<IDeliveryConnection> Conn_OnDisconnected { set => _conn.Conn_OnDisconnected = value; }



        /// <summary>
        /// 地址。例如： "tcp://127.0.0.1:4504" 、 "ipc://4504"
        /// </summary>
        public string endpoint = "tcp://127.0.0.1:4504";

        SocketStream stream = new SocketStream();

        public bool Connect()
        {

            try
            {

                Logger.Info("[CL.DeliveryClient] Zmq.FullDuplex,connecting", new { endpoint });

                //(x.1)
                _conn.OnSendFrameAsync = SendMessageAsync;

                //(x.2) create zmq socketWriter
                var socketWriter = new ZSocket(ZSocketType.DEALER);
                socketWriter.Identity = _conn.identityOfWriter;
                socketWriter.Connect(endpoint);

                //(x.3) create zmq socketReader
                var socketReader = new ZSocket(ZSocketType.DEALER);
                socketReader.Identity = _conn.identityOfReader;
                socketReader.Connect(endpoint);


                //(x.4) Start stream               
                stream.OnReceiveMessage = OnReceiveMessage;
                stream.BeforeStop = () =>
                {
                    SendCloseSignal();
                    _conn.Close();
                    //以防 关闭命令 没有发送出去
                    Thread.Sleep(100);
                };
                //stream.AfterStop = () => {  };
                stream.Start(socketReader, socketWriter);
                Logger.Info("[CL.DeliveryClient] Zmq.FullDuplex,connected");
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
            if (null == msg || msg.Count == 0) return;

            byte[] msgFrame = msg[0];

            #region 检测是否为 关闭命令               
            if (msg.Count > 1)
            {
                bool getedCloseSignal = false;
                try
                {
                    var data = msg[1];
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

            _conn.OnGetFrame(_conn, msgFrame.BytesToArraySegmentByte());
        }

        void SendMessageAsync(DeliveryConnection conn, byte[] data)
        {
            stream.SendMessageAsync(data);
        }

        private void SendCloseSignal()
        {
            stream.SendMessageAsync(new byte[1], new byte[] { (byte)0xff });
            stream.SendMessageAsync(new byte[1], new byte[] { (byte)0xff });
        }


        public void Close()
        {
            stream.Stop();
        }

    }
}
