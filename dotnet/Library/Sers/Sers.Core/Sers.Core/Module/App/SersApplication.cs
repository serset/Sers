using System;
using System.Threading;

using Sers.Core.Module.Env;
using Sers.Hardware.Env;

using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Threading.Timer;

namespace Sers.Core.Module.App
{
    public class SersApplication
    {
        static AutoResetEvent stopEvent = new AutoResetEvent(false);

        public static bool IsRunning { get; private set; } = false;


        #region Action OnStart OnStop
        public static Action onStart;
        public static Action onStop;
        #endregion



        #region timer for System.GC.Collect();
        static VitTimer gcTimer = new VitTimer()
        {
            intervalMs = 10000,
            timerCallback = (e) =>
              {
                  System.GC.Collect();
              }
        };
        #endregion


        #region Start
        public static void OnStart()
        {

            IsRunning = true;
            try
            {
                onStart?.Invoke();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                gcTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion


        public static void OnStop()
        {
            try
            {
                gcTimer.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            IsRunning = false;

            try
            {
                onStop?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            stopEvent.Set();



            try
            {
                //退出当前进程
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                //退出当前进程以及当前进程开启的所有进程
                System.Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }



        /// <summary>
        /// 强制控制台不退出，除非执行Stop()
        /// </summary>
        public static void RunAwait()
        {
            if (!IsRunning) return;
            stopEvent.Reset();
            stopEvent.WaitOne();
        }




        /// <summary>
        /// 注册Ctrl+C 关闭事件
        /// </summary>
        public static void ResistConsoleCancelKey(Action onStop = null)
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
        public static readonly ServiceStationInfo serviceStationInfo;

        /// <summary>
        /// 服务站点硬件设备信息
        /// </summary>
        public static readonly DeviceInfo deviceInfo;


        public static void CalculateUniquekey()
        {
            if (string.IsNullOrEmpty(deviceInfo.deviceKey))
            {
                deviceInfo.deviceKey = SersEnvironment.deviceKey;
            }

            if (string.IsNullOrEmpty(serviceStationInfo.serviceStationKey))
            {
                serviceStationInfo.serviceStationKey = SersEnvironment.serviceStationKey;
            }
        }

        static SersApplication()
        {
            #region #1 serviceStationInfo
            {
                serviceStationInfo = Appsettings.json.GetByPath<ServiceStationInfo>("Sers.ServiceStation.serviceStationInfo")
                    ?? new ServiceStationInfo();

                // ##1 stationVersion
                if (string.IsNullOrEmpty(serviceStationInfo.stationVersion))
                {
                    serviceStationInfo.stationVersion = SersEnvironment.GetEntryAssemblyVersion();
                }

                // ##2 serviceStationKey
                serviceStationInfo.serviceStationKey = null;
                //if (string.IsNullOrEmpty(serviceStationInfo.serviceStationKey))
                //{
                //    serviceStationInfo.serviceStationKey = SersEnvironment.serviceStationKey;
                //}
            }
            #endregion

            #region #2 deviceInfo
            {
                deviceInfo = EnvHelp.GetDeviceInfo();

                deviceInfo.deviceKey = null;
                //deviceInfo.deviceKey = ConfigurationManager.Instance.GetStringByPath("Sers.ServiceStation.deviceInfo.deviceKey")
                //    ?? SersEnvironment.deviceKey;

            }
            #endregion
        }

        #endregion

    }
}
