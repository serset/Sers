using System;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector
{
    public class ConsoleCollector : ILogCollector
    {

        public static Action<LogMessage> OnLog = (msg) =>
        {
            #region build log string
            string logString = msg.message;

            if (msg.objs != null)
            {
                foreach (var obj in msg.objs)
                {
                    logString += Environment.NewLine + obj.Serialize();
                }
            }
            #endregion

            Console.WriteLine("[" + msg.level + "]" + DateTime.Now.ToString("[HH:mm:ss.ffff]") + logString);
        };




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(LogMessage msg)
        {
            OnLog?.Invoke(msg);
        }
    }
}
