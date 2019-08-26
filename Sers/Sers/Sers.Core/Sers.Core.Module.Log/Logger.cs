using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Sers.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public static class Logger
    {
        public static readonly LogMng log = new LogMng();



       
        public static Action<Level, string> OnLog { set => log.OnLog = value; get => log.OnLog; }

        /// <summary>
        /// DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            log.Debug(message);
        }

        /// <summary>
        /// INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            log.Info(message);
        }



        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            log.Error(message);
        }







 

    }
}
