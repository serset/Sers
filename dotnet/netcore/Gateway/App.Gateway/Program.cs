using System;
using Sers.Gateway;
using Sers.ServiceStation;
using Vit.Core.Module.Log;

namespace App.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {

                #region (x.1)初始化ServiceStation                
                ServiceStation.Init();

                //ServiceStation.Discovery(typeof(Program).Assembly);
                if (!ServiceStation.Start())
                {
                    Logger.Info("无法连接服务中心。站点关闭...");
                    return;
                }

                #endregion

                GatewayHelp.Bridge();


                ServiceStation.RunAwait();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Exception:" + ex.Message);
            }


        }
    }
}
