using System;
using System.Collections.Generic;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;

namespace Sers.Mq.Zmq.ClrZmq.ThreadWait
{
    public class MqConn: IMqConn
    {       
        ~MqConn()
        {
            Close();
        }

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = MqConnState.waitForCertify;

        public string connTag { get; set; }

        public byte[] zmqIdentity { get; set; }


        public Action<MqConn,List<ArraySegment<byte>>> OnSendFrameAsync { get; set; }
         
        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            OnSendFrameAsync(this,data);
        }
        

        public Action<IMqConn> Conn_OnDisconnected { get; set; }

        public void Close()
        {             
            state = MqConnState.closed;             
            try
            {
                Conn_OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
