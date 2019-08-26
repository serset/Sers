using Sers.Core.Module.App;
using Sers.Core.Module.Log;
using Sers.Core.Util.Threading;
using Sers.ServiceCenter.ApiCenter;
using Sers.ServiceCenter.ApiCenter.Gover.Core;

namespace Sers.Core.Extensions
{
    public static partial class ApiCenterExtensions
    {
        /// <summary>
        /// 使用 Gover 服务治理 模块
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serviceCenter"></param>
        public static void UseGover(this ApiCenterService data, Sers.ServiceCenter.ServiceCenter serviceCenter)
        {
            if (null == data) return;

            serviceCenter.DiscoveryApi(typeof(global::Sers.Gover.Controller.ApiControllers.ServiceStationController).Assembly);

            data.SetApiCenterManage(GoverManage.Instance);

            //注册站点关闭回调
            SersApplication.onStop+=(GoverManage.SaveToFile);


            #region ApiStation Qps 定时计算
            SersApplication.onStart += (QpsTimer_Start);
            SersApplication.onStop += (QpsTimer_Stop);

            #endregion
        }

        #region QpsTimer

        static int SaveToFile_tick = 0;
        static void QpsTimer_Calc()
        {
            try
            {
                foreach (var item in GoverManage.Instance.ApiStation_GetAll())
                {
                    item.QpsCalc();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }

          
            try
            {
                SaveToFile_tick++;
                if (SaveToFile_tick > 10)
                {
                    SaveToFile_tick = 0;
                    GoverManage.SaveToFile();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }

        }
        static SersTimer timer = null;
        static void QpsTimer_Start()
        {
            if (null != timer) return;
            timer = new SersTimer
            {
                interval = 2,
                timerCallback =
                (object obj) =>
                {
                    QpsTimer_Calc();
                }
            };
            timer.Start();           
        }

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
