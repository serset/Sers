using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Extensions;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.Log
{
    public static partial class LoggerExtensions
    {
      


       
        public static void Info(this LogMng log,object message)
        {
            log.Info(message.Serialize());
        }



        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="log"></param>
        /// <param name="ex"></param>
        public static void Error(this LogMng log,Exception ex)
        {
            Error(log,null,ex);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Error(this LogMng log, string message, Exception ex = null)
        {
            //if (string.IsNullOrWhiteSpace(message)) message = ex.Message;
           
            var strMsg = "";
            if (!string.IsNullOrWhiteSpace(message)) strMsg += " message:" + message;
            if (null != ex)
            {
                ex = ex.GetBaseException();
                var ssError = ex.SsError_Get().Serialize();
                //if (!string.IsNullOrWhiteSpace(ssError))
                strMsg += Environment.NewLine + " ssError:" + ssError;
                strMsg += Environment.NewLine + " StackTrace:" + ex.StackTrace;
            }
            log.Error(strMsg);
        }

        public static void Error(this LogMng log, SsError ssError)
        {
            Error(log, null, ssError);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <param name="ssError"></param>
        public static void Error(this LogMng log, string message, SsError ssError)
        {        

            var strMsg = "";
            if (!string.IsNullOrWhiteSpace(message)) strMsg += " message:" + message+ Environment.NewLine;
            if (null != ssError)
            {
                strMsg += " ssError:" + ssError.Serialize();
            }
            log.Error(strMsg);
        }
    }
}
