using Newtonsoft.Json.Linq;

using System;

namespace Vit.Core.Module.Log.LogCollector
{
    public class Collector : ILogCollector
    {
        public JObject config;
        public void Init(JObject config)
        {
            this.config = config;
        }

        public Action<LogMessage> OnLog;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(LogMessage msg)
        {
            OnLog?.Invoke(msg);
        }

    }
}
