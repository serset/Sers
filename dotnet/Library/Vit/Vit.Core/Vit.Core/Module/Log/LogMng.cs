using System;
using Vit.Core.Module.Log.LogCollector;
using System.Collections.Generic;

namespace Vit.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public partial class LogMng
    {
        public readonly List<ILogCollector> collectors = new List<ILogCollector>();


        public void AddCollector(ILogCollector collector)
        {
            collectors.Add(collector);
        }
        public void AddCollector(Action<LogMessage> OnLog)
        {
            collectors.Add(new LogCollector.Collector { OnLog= OnLog });
        }


        #region Log
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Log(LogMessage msg)
        {
            try
            {
                foreach (var collector in collectors)
                    collector.Write(msg);
            }
            catch { }
        }
        #endregion
 


        #region Log

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Log(Level level, string message, params object[] objs)
        {
            Log(new LogMessage { level = level, message = message, objs = objs });
        }
        #endregion



    }
}
