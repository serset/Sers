using Sers.Core.Util.ConfigurationManager;
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
                var urls =   ConfigurationManager.Instance.GetByPath<string[]>("host.urls");
                var wwwwroot = ConfigurationManager.Instance.GetByPath<string>("host.wwwroot");

                if (string.IsNullOrWhiteSpace(wwwwroot))
                    wwwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");


                Sers.Core.Common.WebHost.Startup.Run(wwwwroot,null, urls);
 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:"+ex.Message);

            }
          
 
        }
    }
}
