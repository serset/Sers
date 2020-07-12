using System;
using System.Diagnostics;

namespace Sers.Hardware
{
    public class OsShell
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"> 如 reboot </param>
        /// <param name="arguments"></param>
        /// <param name="output"></param>
        /// <param name="millisecondsOfWait">等待时间，默认4000。若不指定则永久等待</param>
        /// <param name="WorkingDirectory">sets the working directory for the process to be started</param>
        /// <returns></returns>
        public static void Shell(string fileName, string arguments, out string output, int? millisecondsOfWait = 4000, string WorkingDirectory = null)
        {
            output = null;

            //(x.1)创建Process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(fileName, arguments)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                }
            };

            if (!string.IsNullOrEmpty(WorkingDirectory))
            {
                process.StartInfo.WorkingDirectory = WorkingDirectory;
            }

            #region (x.2)ProcessExit,确保程序退出时会关闭process          
            EventHandler stopProcess = null;
            stopProcess = (s, e) =>
            {
                lock (stopProcess)
                {
                    AppDomain.CurrentDomain.ProcessExit -= stopProcess;
                    try
                    {
                        if (process != null)
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                            }

                            process.Dispose();
                            process = null;
                        }
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            };
            AppDomain.CurrentDomain.ProcessExit += stopProcess;
            #endregion


            #region (x.3)启动Process           
            try
            {

                process.Start();
                //process.WaitForExit(millisecondsOfWait);
                //process.WaitForExit();
                //output = process.StandardOutput.ReadToEnd();
                var task = process.StandardOutput.ReadToEndAsync();
                if (millisecondsOfWait.HasValue)
                {
                    task.Wait(millisecondsOfWait.Value); 
                }
                else
                {
                    task.Wait();
                }
                if (task.IsCompleted)
                {
                    output = task.Result;
                }
            }
            finally
            {
                stopProcess(null, null);
            }
            #endregion


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"> 如 reboot </param>
        /// <param name="arguments"></param>
        /// <param name="millisecondsOfWait">等待时间，默认null。若不指定则永久等待</param>
        /// <param name="WorkingDirectory">sets the working directory for the process to be started</param>
        /// <returns></returns>
        public static void Shell(string fileName, string arguments, int? millisecondsOfWait = null, string WorkingDirectory = null)
        {

            //(x.1)创建Process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(fileName, arguments)
                {
                    //RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };


            if (!string.IsNullOrEmpty(WorkingDirectory))
            {
                process.StartInfo.WorkingDirectory = WorkingDirectory;
            }


            #region (x.2)ProcessExit,确保程序退出时会关闭process          
            EventHandler stopProcess = null;
            stopProcess = (s, e) =>
            {
                lock (stopProcess)
                {
                    AppDomain.CurrentDomain.ProcessExit -= stopProcess;
                    try
                    {
                        if (process != null)
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                            }

                            process.Dispose();
                            process = null;
                        }
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            };
            AppDomain.CurrentDomain.ProcessExit += stopProcess;
            #endregion


            #region (x.3)启动Process           
            try
            {
                process.Start();
                if (millisecondsOfWait.HasValue)
                    process.WaitForExit(millisecondsOfWait.Value);
                else
                    process.WaitForExit();
            }
            finally
            {
                stopProcess(null, null);
            }
            #endregion


        }
    }
}
