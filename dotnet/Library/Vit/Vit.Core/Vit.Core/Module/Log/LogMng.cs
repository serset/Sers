using Vit.Extensions;
using System;
using System.IO;
using System.Linq;
using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public class LogMng
    {
        #region LogTxt
        public string NewLine = Environment.NewLine;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void LogTxt(Level level, string message)
        {
            //加锁 以避免多线程抢占文件错误
            // IOException: The process cannot access the file 'file path' because it is being used by another process

            string finalMsg = DateTime.Now.ToString("[HH:mm:ss.ffff]") + message + NewLine;
            string filePath = GetLogPath(level);
            lock (filePath)
                File.AppendAllText(filePath, finalMsg);
        }


        #region class LogFilePathCache
        class LogFilePathCache
        {

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void SetBasePath(string path)
            {
                BasePath = path;
                directoryDate = DateTime.MinValue;
            }
             

            static string AppPath => AppContext.BaseDirectory;
            //static string AppPath => Path.GetDirectoryName(typeof(Logger).Assembly.Location);

            /// <summary>
            /// /Logs
            /// </summary>
            string BasePath = Path.Combine(AppPath, "Logs");


            DateTime directoryDate = DateTime.MinValue;

            string[] paths;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public LogFilePathCache()
            {
                Level[] vs = (Level[])Enum.GetValues(typeof(Level));
                var max = vs.Max((m) => (int)m);
                paths = new string[max + 1];

            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public string GetPath(Level level)
            {
                if (DateTime.Now.Date != directoryDate)
                {
                    lock (this)
                    {
                        directoryDate = DateTime.Now.Date;

                        // path
                        //  /Logs/{yyyy-MM}/{yyyy-MM-dd}{level}.log         
                        var directoryPath = Path.Combine(BasePath,directoryDate.ToString("yyyy-MM"));
                        Directory.CreateDirectory(directoryPath);
                        foreach (Level l in Enum.GetValues(typeof(Level)))
                        {
                            paths[(int)l] = Path.Combine(directoryPath , directoryDate.ToString("[yyyy-MM-dd]")+l.ToString().ToLower() + ".log");
                        }
                    }
                }
                return paths[(int)level];
            }

        }
        #endregion

        #region Path

        /// <summary>
        /// log的BasePath
        /// </summary>
        public string BasePath { set => fileCache.SetBasePath(value); }



        LogFilePathCache fileCache = new LogFilePathCache();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private string GetLogPath(Level level)
        {
            return fileCache.GetPath(level);
        }
        #endregion
        




        #endregion


        #region Log
        /// <summary>
        ///  例如    (level, msg)=> { Console.WriteLine("[" + level + "]" + DateTime.Now.ToString("[HH:mm:ss.ffff]") + msg);   };
        /// </summary>
        public Action<Level, string> OnLog = null;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Log(Level level, string message)
        {
            try
            {              
                //LogTxt(level, message);
                OnLog?.Invoke(level, message);
            }
            catch { }
        }
        #endregion


        #region 对外 基础
        /// <summary>
        /// DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出
        /// </summary>
        /// <param name="message"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Debug(string message)
        {
            Log(Level.DEBUG, message);
        }

        /// <summary>
        /// INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Info( string message)
        {
            Log(Level.INFO, message);
        }


        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(string message)
        {
            Log(Level.ERROR, message);
        }



        #endregion

        #region 对外 扩展

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Info(object message)
        {
            Info(message.Serialize());
        }



        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="ex"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(Exception ex)
        {
            Error(null, ex);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary> 
        /// <param name="message"></param>
        /// <param name="ex"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(string message, Exception ex)
        {
            //if (string.IsNullOrWhiteSpace(message)) message = ex.Message;

            var strMsg = "";
            if (!string.IsNullOrWhiteSpace(message)) strMsg += " message:" + message;
            if (null != ex)
            {
                ex = ex.GetBaseException();
                var ssError = ex.ToSsError().Serialize();
                //if (!string.IsNullOrWhiteSpace(ssError))
                strMsg += NewLine + " ssError:" + ssError;
                strMsg += NewLine + " StackTrace:" + ex.StackTrace;
            }
            Error(strMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssError"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(SsError ssError)
        {
            Error(null, ssError);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ssError"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(string message, SsError ssError)
        {
            var strMsg = "";
            if (!string.IsNullOrWhiteSpace(message)) strMsg += " message:" + message + NewLine;
            if (null != ssError)
            {
                strMsg += " ssError:" + ssError.Serialize();
            }
            Error(strMsg);
        }
        #endregion

    }
}
