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
                int treadCount = 1;
                if (args?.Length == 1)
                {
                    int.TryParse(args[0], out treadCount);
                }

                for (var t = 0; t < treadCount; t++)
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
