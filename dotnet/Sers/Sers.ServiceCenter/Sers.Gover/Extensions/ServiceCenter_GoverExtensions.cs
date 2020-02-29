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

            serviceCenter.apiCenterService = GoverManage.Instance;

            serviceCenter.LoadSsApi(typeof(ServiceCenter_GoverExtensions).Assembly);

            //注册站点关闭回调
            SersApplication.onStop += (GoverManage.SaveToFile);


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
                intervalMs = 2000,
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
