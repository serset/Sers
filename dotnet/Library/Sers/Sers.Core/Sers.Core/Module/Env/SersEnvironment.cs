using System.Reflection;
using Sers.Hardware.Env;

namespace Sers.Core.Module.Env
{
    public class SersEnvironment
    {
        /// <summary>
        /// deviceKey of every server machines is always unique at anytime
        /// </summary>
        public static   string deviceKey => EnvHelp.MachineUnqueKey;

        /// <summary>
        /// serviceStationKey of every service station is always unique at anytime
        /// </summary>
        public static string serviceStationKey => EnvHelp.AppUnqueKey;



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetEntryAssemblyVersion()
        {
            string version = null;
            try
            {
                //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Reflection.Assembly assembly = Assembly.GetEntryAssembly();
                var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                //version = fvi.FileVersion;  // 2.1.20.0
                version = fvi.ProductVersion;  // 2.1.20-preview12
            }
            catch { }
            return version;
        }

    }
}
