using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Mq.Mq
{
    public interface IMqConn
    {

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        byte state { get; set; }
 
        String connTag { get; set; }

     
        void SendFrameAsync(List<ArraySegment<byte>> data);

        void Close();
    }
}
