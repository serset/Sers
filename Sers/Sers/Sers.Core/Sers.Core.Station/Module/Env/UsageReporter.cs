using Sers.Core.Module.Log;
using Sers.Core.Module.PubSub.ShareEndpoint;
using Sers.Core.Util.Hardware;
using Sers.Core.Util.Threading;

namespace Sers.Core.Module.Env
{
    public class UsageReporter
    {        

        public const string Pubsub_UsageInfoReportTitle = "Sers_Sys_UsageInfo";

 
        public static void Publish()
        { 
            try
            {              
                var info = new EnvUsageInfo();
                info.usageStatus = DeviceManage.GetUsageInfo();
                info.deviceKey = Sers.Core.Module.Env.SersEnvironment.deviceKey;
                info.serviceStationKey = Sers.Core.Module.Env.SersEnvironment.serviceStationKey;

                Publisher.Publish(Pubsub_UsageInfoReportTitle, info);
            }
            catch (System.Exception ex)
            {
                Logger.log.Error(ex);
            }                   
        }

        static SersTimer timer = null;

        /// <summary>
        /// 开启自动上报Usage任务
        /// </summary>
        public static void StartReportTask()
        {
            if (null != timer) return;
            timer = new SersTimer { interval = 2, timerCallback =
                (object obj)=> 
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
                Logger.log.Error(ex);
            }

            try
            {
                DeviceManage.Dispose();
            }
            catch (System.Exception ex)
            {
                Logger.log.Error(ex);
            }
            
        }

    }
}
