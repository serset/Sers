
using System;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using ZeroMQ;
using Sers.Core.Extensions;
using System.Collections.Generic;
using Sers.Core.Util.Common;
using System.Threading;

namespace Sers.Mq.Zmq.ClrZmq.ThreadWait
{
    public class ClientMq : IClientMq
    {
        /// <summary>
        /// 连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        /// </summary>
        public string secretKey { get; set; }


        MqConn _mqConn = new MqConn() { zmqIdentity = CommonHelp.NewGuidLong().Int64ToBytes() };

    



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> OnGetFrame { get; set; }

        public Action<IMqConn> Conn_OnDisconnected { get => _mqConn.Conn_OnDisconnected; set => _mqConn.Conn_OnDisconnected = value; }


        public IMqConn mqConn => _mqConn;


        SocketPoller poller = new SocketPoller();


        /// <summary>
        /// Mq 地址。例如： "tcp://127.0.0.1:10346" 、 "ipc://10346"
        /// </summary>
        public string endpoint = "tcp://127.0.0.1:10346";

        public bool Connect()
        {         

            try
            {

                Logger.Info("[ClientMq] Zmq.ThreadWait,connecting...  endpoint: \"" + endpoint + "\"");

                //(x.1)
                _mqConn.OnSendFrameAsync = Zmq_SendMessageAsync;

                //(x.2) create zmq conn
                var socket = new ZSocket(ZSocketType.DEALER);
                socket.Identity = _mqConn.zmqIdentity;
                socket.Connect(endpoint);


                //(x.3) init poller
                poller.OnReceiveMessage = Zmq_OnReceiveMessage;
                poller.Start(socket);
                Logger.Info("[ClientMq] Zmq.ThreadWait,connected.");
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

            OnGetFrame(_mqConn, msgFrame.BytesToArraySegmentByte());
        }

        void Zmq_SendMessageAsync(MqConn conn,List<ArraySegment<byte>> data)
        {
            poller.SendMessageAsync(new ZMessage() { new ZFrame(data.ByteDataToBytes()) });            
        }

       


        public void Close()
        {
            SendCloseSignal();

            try
            {
                poller.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                _mqConn.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            _mqConn = null;

        }

        private void SendCloseSignal()
        {
            try
            {
                if (poller != null)
                {
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
