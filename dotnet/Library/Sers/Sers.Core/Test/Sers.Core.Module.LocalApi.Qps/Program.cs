﻿using System;
using System.Threading;

using Sers.Core.Module.LocalApi.MsTest.LocalApi;

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

                LocalApiTest.threadCount = 16;
                int requestTreadCount = 16;
                if (args != null)
                {
                    if (args.Length >= 1)
                    {
                        int.TryParse(args[0], out LocalApiTest.threadCount);
                    }

                    if (args.Length >= 2)
                    {
                        int.TryParse(args[1], out requestTreadCount);
                    }
                }
                for (var t = 0; t < requestTreadCount; t++)
                {
                    //LocalApiTest.StartThread();
                    LocalApiTest.StartThread_Async();

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
