using System;
using System.IO;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port = 8888;
                //var wwwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var wwwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");



                Sers.Core.Common.WebHost.Startup.Run(port, wwwwroot);


                Console.WriteLine("Hello World!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:"+ex.Message);

            }
          
 
        }
    }
}
