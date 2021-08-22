using System;
using System.Diagnostics;

using Vit.Core.Module.Log;

namespace Sers.Hardware.Process
{
    public class ProcessInfo
    {
        /// <summary>
        /// 总线程数
        /// </summary>
        public int ThreadCount;

        /// <summary>
        /// 活动的线程数
        /// </summary>
        public int RunningThreadCount;


        /// <summary>
        /// 占用总内存（单位：MB）
        /// </summary>
        public float WorkingSet;



        public static ProcessInfo GetCurrentProcessInfo() 
        {
            var processInfo=new ProcessInfo();
            try
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();

                //(x.x.1) ThreadCount
                processInfo.ThreadCount = process.Threads.Count;

                //(x.x.2) RunningThreadCount
                int n = 0;
                foreach (ProcessThread th in process.Threads)
                {
                    if (th.ThreadState == ThreadState.Running)
                        n++;
                }
                processInfo.RunningThreadCount = n;

                //(x.x.3) WorkingSet
                processInfo.WorkingSet = process.WorkingSet64 / 1024.0f / 1024;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return processInfo;
        }
    }
}
