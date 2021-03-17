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
                for (var t = 0; t < 4; t++)
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
