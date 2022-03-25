using System;
using Vit.Core.Module.Log.LogCollector;
using System.Collections.Generic;
using Vit.Core.Util.ConfigurationManager;

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
            collectors.Add(new LogCollector.Collector { OnLog = OnLog });
        }


        #region Log

        static bool? PrintLogErrorToConsole = ConfigurationManager.Instance.GetByPath<bool?>("Vit.Logger.PrintLogErrorToConsole");

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Log(LogMessage msg)
        {
            try
            {
                foreach (var collector in collectors)
                    collector.Write(msg);
            }
            catch(Exception e)
            {
                if (PrintLogErrorToConsole == true)
                    Console.WriteLine("[Vit.Core.Module.Log] error: " + e.Message);
            }
        }
        #endregion



        #region Log

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Log(Level level, string message, params object[] metadata)
        {
            Log(new LogMessage { level = level, message = message, metadata = metadata });
        }
        #endregion



    }
}
