using Sers.Core.Module.LocalApi.MsTest.LocalApi;
using System;
using System.Threading;
using Vit.Core.Module.Log;

namespace DeliveryTest
{
    class Program
    {
        /// <summary>
        ///             qps
        ///   16 16   25-26万 （2021-03-23）
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                int requestTreadCount = 16;
                LocalApiTest.workThreadCount = 16;

                if (args != null)
                {
                    if (args.Length >=1)
                    {
                        int.TryParse(args[0], out requestTreadCount);
                    }

                    if (args.Length >= 2)
                    {
                        int.TryParse(args[1], out LocalApiTest.workThreadCount);
                    }
                }
                for (var t = 0; t < requestTreadCount; t++)
                {
                    LocalApiTest.StartThread();
                }


                while (true)
                {
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

         




    }
}
