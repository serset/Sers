using Sers.Core.Module.Mq.Mq;
using Sers.Mq.Ipc.SharedMemory.Stream;
using System;
using System.Collections.Generic;
using Sers.Core.Extensions;

namespace Sers.Mq.Ipc.SharedMemory.Mq
{
    public class MqConn : IMqConn
    {
 

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = MqConnState.waitForCertify;

        public string connTag { get; set; }

   

        internal WriteStream writeStream;


        public void Close()
        {
            state = MqConnState.closed;
        }

        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            writeStream.SendMessageAsync(data.ByteDataToBytes());
        }
    }
}
