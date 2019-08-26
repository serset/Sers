using Sers.Core.Module.Env;
using Sers.Core.Module.Log;
using Sers.Hardware.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sers.Core.Module.App
{
    public class SersApplication
    {
        static AutoResetEvent stopEvent = new AutoResetEvent(false);

        public static bool IsRunning = false;

        public static void Stop()
        {
            IsRunning = false;


            stopEvent.Set();

            try
            {
                System.Environment.Exit(0);//退出全部线程                   
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }

            try
            {
                //System.Environment.Exit(0);//退出全部线程  
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }

           

        }

        

        /// <summary>
        /// 强制控制台不退出，除非执行Stop()
        /// </summary>
        public static void RunAwait()
        {
            if(!IsRunning) return;
            stopEvent.Reset();
            stopEvent.WaitOne();           
        }

        /// <summary>
        /// 注册Ctrl+C 关闭事件
        /// </summary>
        public static void ResistConsoleCancelKey(Action onStop=null)
        {
            Console.WriteLine("Press Ctrl+C to shut down.");
            Console.CancelKeyPress += new ConsoleCancelEventHandler((object sender, ConsoleCancelEventArgs args) =>
                {
                    //args.Cancel = true;
                    try
                    {
                        onStop?.Invoke();
                    }
                    catch
                    { 
                    }        
                });
        }


        #region ServiceStationInfo
        /// <summary>
        /// 服务站点信息
        /// </summary>
        public static readonly ServiceStationInfo serviceStationInfo = new ServiceStationInfo();


        public static ServiceStationInfo GetServiceStationInfo()
        {

            #region stationName
            if (string.IsNullOrEmpty(serviceStationInfo.stationName))
            {
                serviceStationInfo.stationName = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Sers.ServiceStation.StationInfo.StationName");
            }
            #endregion


            #region stationVersion
            if (string.IsNullOrEmpty(serviceStationInfo.stationVersion))
            {
                serviceStationInfo.stationVersion = SersEnvironment.GetEntryAssemblyVersion();
            }
            #endregion

            serviceStationInfo.serviceStationKey = SersEnvironment.serviceStationKey;

            return serviceStationInfo;
        }
        #endregion


        #region GetDeviceInfo
        public static DeviceInfo GetDeviceInfo()
        {
            var info = Core.Util.Hardware.DeviceManage.GetDeviceInfo();
            info.deviceKey = SersEnvironment.deviceKey;
            return info;
        }
        #endregion

    }
}
