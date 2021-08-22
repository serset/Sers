﻿using Sers.Core.Module.App;
using Vit.Core.Module.Log;
using Sers.Core.Module.PubSub;
using Vit.Core.Util.ConfigurationManager;
using Sers.Hardware.Usage;
using Vit.Core.Util.Threading.Timer;
using Sers.Hardware.Process;

namespace Sers.Core.Module.Env
{
    public class UsageReporter
    {        

        public const string Pubsub_UsageInfoReportTitle = "Sers_Sys_UsageInfo";


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Publish()
        {
            try
            {
                var info = new EnvUsageInfo();
                info.usageStatus = UsageHelp.GetUsageInfo();
                info.Process = ProcessInfo.GetCurrentProcessInfo();

                info.deviceKey = Sers.Core.Module.Env.SersEnvironment.deviceKey;
                info.serviceStationKey = Sers.Core.Module.Env.SersEnvironment.serviceStationKey;

                MessageClient.Publish(Pubsub_UsageInfoReportTitle, info);
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
        }

        static SersTimer timer = null;
        /// <summary>
        /// 开启自动上报Usage任务
        /// </summary>
        public static void StartReportTask(double intervalSecond)
        {
            if (null != timer) return;
            timer = new SersTimer { 
                intervalMs = (int)(intervalSecond * 1000),
                timerCallback =
                (object obj) =>
                {
                    Publish();
                }
            };
            timer.Start();
        }

        /// <summary>
        /// 关闭自动上报Usage任务
        /// </summary>
        public static void StopReportTask()
        {
            try
            {
                if (null != timer)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                UsageHelp.Dispose();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
        }



        #region UseUsageReporter


        /// <summary>
        /// 自动上报Usage任务
        /// </summary>
        public static void UseUsageReporter()
        {
            var intervalSecond = ConfigurationManager.Instance.GetByPath<double?>("Sers.ServiceStation.UsageReporter.intervalSecond");
            if (!intervalSecond.HasValue || intervalSecond.Value <= 0)
            {
                return;
            }

            SersApplication.onStart += (() => Sers.Core.Module.Env.UsageReporter.StartReportTask(intervalSecond.Value));
            SersApplication.onStop += (Sers.Core.Module.Env.UsageReporter.StopReportTask);
        }
        #endregion

    }
}
