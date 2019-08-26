using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Mq.Mq
{
    /// <summary>
    /// (0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
    /// </summary>
    public class MqConnState
    {
        public const byte waitForCertify = 0;
        public const byte certified = 2;

        public const byte waitForClose = 4;
        public const byte closed = 8;
    }
}
