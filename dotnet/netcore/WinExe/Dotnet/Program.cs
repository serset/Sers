using System;

namespace Dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainFinder.Dotnet();
            }
            catch (Exception ex)
            {
                ex = ex.GetBaseException();
                Console.WriteLine("出错：" + ex.Message);
                Console.WriteLine("StackTrace：" + ex.StackTrace);
                Console.ReadLine();
            }
            Console.WriteLine("结束。");
            Console.ReadLine();
        }
    }
}
