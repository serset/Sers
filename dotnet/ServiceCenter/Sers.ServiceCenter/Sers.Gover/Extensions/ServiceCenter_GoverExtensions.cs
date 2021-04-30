using Sers.Core.Module.App;
using Sers.Gover.Base;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Vit.Extensions
{
    public static partial class ServiceCenter_GoverExtensions
    {
        /// <summary>
        /// 使用 Gover 服务治理 模块
        /// </summary>       
        /// <param name="serviceCenter"></param>
        public static void UseGover(this Sers.ServiceCenter.ServiceCenter serviceCenter)
        {
            if (null == serviceCenter) return;

            serviceCenter.apiCenterService = GoverApiCenterService.Instance;

            serviceCenter.LoadSsApi(typeof(ServiceCenter_GoverExtensions).Assembly);

            //注册站点关闭回调
            SersApplication.onStop += (GoverApiCenterService.SaveToFile);


            #region ApiStation Qps 定时计算
            SersApplication.onStart += (QpsTimer_Start);
            SersApplication.onStop += (QpsTimer_Stop);

            #endregion
        }

        #region QpsTimer

        static int SaveToFile_tick = 0;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static void QpsTimer_Calc()
        {
            //(x.1)计算ApiStation qps
            try
            {
                foreach (var item in GoverApiCenterService.Instance.ApiStation_GetAll())
                {
                    item.QpsCalc();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }

            //(x.2)计算ServiceStation qps
            try
            {
                foreach (var item in GoverApiCenterService.Instance.serviceStationMng.serviceStationCollection)
                {
                    item.QpsCalc();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }

            //(x.3)按需保存计数信息到文件
            try
            {
                SaveToFile_tick++;
                if (SaveToFile_tick > 10)
                {
                    SaveToFile_tick = 0;
                    GoverApiCenterService.SaveToFile();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }

        }

        static SersTimer timer = null;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static void QpsTimer_Start()
        {
            if (null != timer) return;
            timer = new SersTimer
            {
                intervalMs = 2000,
                timerCallback =
                (object obj) =>
                {
                    QpsTimer_Calc();
                }
            };
            timer.Start();           
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static void QpsTimer_Stop()
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

        }
        #endregion
    }
}
