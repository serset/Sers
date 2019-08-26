using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sers.Hardware.Hardware.Os
{
    public class OsShell
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"> 如 reboot </param>
        /// <param name="arguments"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static void Shell(string fileName,string arguments,out string output)
        {
            output = null;
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo(fileName, arguments)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            })
            {
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }  

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"> 如 reboot </param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static void Shell(string fileName, string arguments)
        {
       
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo(fileName, arguments)
                {
                    //RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            })
            {
                process.Start();          
                process.WaitForExit();
            }

        }
    }
}
