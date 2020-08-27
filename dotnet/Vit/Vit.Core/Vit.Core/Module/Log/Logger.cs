using System;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;

namespace Vit.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public static class Logger
    {

        public static readonly LogMng log = new LogMng();

        #region static Logger
        static Logger()
        {
            log.InitByConfigurationManager(); 
        }
        #endregion



        /// <summary>
        ///  例如    (level, msg)=> { Console.WriteLine("[" + level + "]" + DateTime.Now.ToString("[HH:mm:ss.ffff]") + msg);   };
        /// </summary>
        public static Action<Level, string> OnLog { set => log.OnLog = value; get => log.OnLog; }

        /// <summary>
        /// DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            log.Debug(message);
        }

        #region Info

        /// <summary>
        /// INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            log.Info(message);
        }

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
        public static void Error(string message)
        {
            log.Error(message);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(Exception ex)
        {
            log.Error(null, ex);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary> 
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Error(string message, Exception ex)
        {        
            log.Error(message,ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssError"></param>
        public static void Error(SsError ssError)
        {
            log.Error(ssError);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ssError"></param>
        public static void Error(string message, SsError ssError)
        {
            log.Error(message,ssError);
        }
        #endregion








    }
}
