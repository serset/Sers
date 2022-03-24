using System;
using System.Collections.Generic;

namespace Vit.Core.Module.Log
{
    public class LogMessage
        //: Extensible
    {
        public Level level;
        public string message;
        public Object[] metadata;
    }
}
