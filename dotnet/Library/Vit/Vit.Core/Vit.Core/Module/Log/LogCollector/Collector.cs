using System;

namespace Vit.Core.Module.Log.LogCollector
{
    public class Collector : ILogCollector
    {

        public Action<LogMessage> OnLog;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(LogMessage msg)
        {
            OnLog?.Invoke(msg);
        }

    }
}
