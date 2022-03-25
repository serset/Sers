using System;

using Vit.Core.Util.ComponentModel.SsError;
using Vit.Core.Util.ConfigurationManager;
using System.Linq;
using Vit.Core.Module.Log.LogCollector;
using Vit.Extensions;

namespace Vit.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public static class Logger
    {

        public static   LogMng log { get; set; } = new LogMng();

        #region static Logger
        static Logger()
        {
            if (false != ConfigurationManager.Instance.GetByPath<bool?>("Vit.Logger.PrintToTxt"))
            {
                PrintToTxt = true;
            }

            if (false != ConfigurationManager.Instance.GetByPath<bool?>("Vit.Logger.PrintToConsole"))
            {
                PrintToConsole = true;
            }

            //splunk
            var splunk = ConfigurationManager.Instance.GetByPath<SplunkCollector>("Vit.Logger.Splunk");
            if (splunk!=null)
            {
                log.collectors.Add(splunk);
            }
        }
        #endregion

        #region PrintToTxt  
        static bool _PrintToTxt = false;
        public static bool PrintToTxt
        {
            get { return _PrintToTxt; }
            set
            {
                _PrintToTxt = value;
                if (value)
                {
                    if (!log.collectors.Any(c => c is TxtCollector))
                    {
                        log.collectors.Add(new LogCollector.TxtCollector());
                    }
                }
                else
                {
                    log.collectors.RemoveAll(c => c is TxtCollector);
                }
            }
        }
        #endregion

        #region PrintToConsole  
        static bool _PrintToConsole = false;
        public static bool PrintToConsole
        {
            get { return _PrintToConsole; }
            set
            {
                _PrintToConsole = value;
                if (value)
                {
                    if (!log.collectors.Any(c => c is ConsoleCollector))
                    {
                        log.collectors.Add(new ConsoleCollector());
                    }
                }
                else
                {
                    log.collectors.RemoveAll(c => c is ConsoleCollector);
                }
            }
        }
        #endregion

        #region Log
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Log(LogMessage msg)
        {
            log.Log(msg);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Log(Level level, string message, params object[] metadata)
        {
            log.Log(level, message, metadata);
        }
        #endregion




        /// <summary>
        /// DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="metadata"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Debug(string message, params object[] metadata)
        {
            log.Debug(message, metadata);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message, params object[] metadata)
        {
            log.Warn(message, metadata);
        }


        #region Info

        /// <summary>
        /// INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="metadata"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Info(string message, params object[] metadata)
        {
            log.Info(message, metadata);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Info(object message)
        {
            log.Info(message);
        }
        #endregion



        #region Error

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="metadata"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, params object[] metadata)
        {
            log.Error(message, metadata);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="ex"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(Exception ex)
        {
            log.Error(ex);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary> 
        /// <param name="message"></param>
        /// <param name="ex"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, Exception ex)
        {
            log.Error(message, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssError"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(SsError ssError)
        {
            log.Error(ssError);
        }

        #endregion



    }
}
