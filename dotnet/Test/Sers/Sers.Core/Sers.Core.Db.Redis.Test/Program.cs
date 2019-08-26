using System;

namespace Sers.Core.Db.Redis.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new RedisQpsTest().Start();
        }
    }
}
