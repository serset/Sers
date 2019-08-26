using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Mq.Mng;
using System;
using System.Collections.Generic;
using Sers.Core.Util.ConfigurationManager;

namespace Test.Sers.Mq.Client
{
    class Program
    {
       


        static void Main(string[] args)
        {

            
            ClientTest client = null;

            //Console.WriteLine("回车开始");            
            //Console.ReadLine();

            client = new ClientTest();

            client.Init();

            client.Connect();

          

            Console.ReadLine();

            //if (null != client) client.Close();


        }
    }
}
