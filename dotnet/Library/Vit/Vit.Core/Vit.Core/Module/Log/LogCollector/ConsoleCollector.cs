using System;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector
{
    public class ConsoleCollector : ILogCollector
    { 
        public static Action<LogMessage> OnLog = (msg) =>
        {
            Console.WriteLine(TxtCollector.BuildMessageData(msg, prefix: "[" + msg.level + "]"));
        };

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(LogMessage msg)
        {
            OnLog?.Invoke(msg);
        }
    }
}
