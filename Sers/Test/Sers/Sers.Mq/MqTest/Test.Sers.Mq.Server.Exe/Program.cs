using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Mq.Mng;
using System;
using System.Collections.Generic;

namespace Test.Sers.Mq.Client
{
    class Program
    {
       


        static void Main(string[] args)
        {
            var test = new ServerTest();

            test.Init();

            test.Connect();

            //test.SendRequest(10, 1000);

            Console.ReadLine();
        }
    }
}
