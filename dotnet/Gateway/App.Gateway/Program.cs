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

                #region init ServiceStation
                ServiceStation.Init();

                //ServiceStation.Discovery(typeof(Program).Assembly);
                if (!ServiceStation.Start())
                {
                    Logger.Error("can not connect to ServiceCenter. Closing station now.");
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
