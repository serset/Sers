using Sers.Core.Module.LocalApi.MsTest.LocalApi;
using System;
using System.Threading;
using Vit.Core.Module.Log;

namespace DeliveryTest
{
    class Program
    {
 
        static void Main(string[] args)
        {
            try
            {
                int requestTreadCount = 4;
                LocalApiTest.workThreadCount = 4;

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
