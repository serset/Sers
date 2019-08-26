using Sers.Core.Extensions;
using Sers.Core.Util.SsError;
using System;
using System.IO;
using System.Linq;

namespace Sers.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public class LogMng
    {
        #region private



        #region class LogFilePathCache
        class LogFilePathCache
        {

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

            public LogFilePathCache()
            {
                Level[] vs = (Level[])Enum.GetValues(typeof(Level));
                var max = vs.Max((m) => (int)m);
                paths = new string[max + 1];

            }

            public string GetPath(Level level)
            {
                if (DateTime.Now.Date != directoryDate)
                {
                    lock (this)
                    {
                        directoryDate = DateTime.Now.Date;

                        //path
                        //// old: Logs/{yyyy-MM-dd}/{level}.log

                        //cur:     /Logs/{yyyy-MM}/{yyyy-MM-dd}{level}.log         
                        var directoryPath = Path.Combine(BasePath,directoryDate.ToString("yyyy-MM"));
                        Directory.CreateDirectory(directoryPath);
                        foreach (Level l in Enum.GetValues(typeof(Level)))
                        {
                            //paths[(int)l] = directoryPath + Path.DirectorySeparatorChar + l + ".log";
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

        private string GetLogPath(Level level)
        {
            return fileCache.GetPath(level);
        }
        #endregion




        public void LogTxt(Level level, string message)
        {
            File.AppendAllText(GetLogPath(level), message);
        }


        #endregion


        #region Log
        public Action<Level, string> OnLog = null;
        public void Log(Level level, string message)
        {
            try
            {
                string finalMsg = DateTime.Now.ToString("[HH:mm:ss.ffff]") + message+ Environment.NewLine;
                LogTxt(level, finalMsg);
                OnLog?.Invoke(level, finalMsg);
            }
            catch { }
        }
        #endregion


        #region 对外 基础
        /// <summary>
        /// DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            Log(Level.DEBUG, message);
        }

        /// <summary>
        /// INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message"></param>
        public  void Info( string message)
        {
            Log(Level.INFO, message);
        }


        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Log(Level.ERROR, message);
        }



        #endregion

        #region 对外 扩展

        public void Info(object message)
        {
            Info(message.Serialize());
        }



        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="ex"></param>
        public void Error(Exception ex)
        {
            Error(null, ex);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary> 
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception ex)
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
            Error(strMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssError"></param>
        public void Error(SsError ssError)
        {
            Error(null, ssError);
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ssError"></param>
        public void Error(string message, SsError ssError)
        {
            var strMsg = "";
            if (!string.IsNullOrWhiteSpace(message)) strMsg += " message:" + message + Environment.NewLine;
            if (null != ssError)
            {
                strMsg += " ssError:" + ssError.Serialize();
            }
            Error(strMsg);
        }
        #endregion

    }
}
