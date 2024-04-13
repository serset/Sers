using System;
using System.Diagnostics;
using System.IO;

namespace Vit.Core.Util.Shell
{
    public class ShellStream: IDisposable
    {

        public StreamReader StandardError => process.StandardError;
        public StreamReader StandardOutput => process.StandardOutput;

        public StreamWriter StandardInput => process.StandardInput;

        public void Dispose()
        {
            stopProcess(null, null);
        }

        public Process process { get; private set; }



        EventHandler stopProcess = null;
        public ShellStream(string fileName, string arguments, string WorkingDirectory = null)
        {

            #region EventHandler           
            stopProcess = (s, e) =>
                {
                    lock (stopProcess)
                    {
                        try
                        {
                            AppDomain.CurrentDomain.ProcessExit -= stopProcess;


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
                        catch { }
                    }
                };
            #endregion

            //(x.1)创建Process
            process = new Process
            {
                StartInfo = new ProcessStartInfo(fileName, arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput=true,
                    UseShellExecute = false,
                }
            };

            if (!string.IsNullOrEmpty(WorkingDirectory))
            {
                process.StartInfo.WorkingDirectory = WorkingDirectory;
            }

            #region (x.2)ProcessExit,确保程序退出时会关闭process          

            AppDomain.CurrentDomain.ProcessExit += stopProcess;
            #endregion


            #region (x.3)启动Process
            process.Start();
            #endregion


        }

    }
}
