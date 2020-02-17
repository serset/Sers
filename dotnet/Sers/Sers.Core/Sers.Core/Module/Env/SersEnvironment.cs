using System.Reflection;
using Sers.Hardware.Env;

namespace Sers.Core.Module.Env
{
    public class SersEnvironment
    {
        /// <summary>
        /// 每台机器的deviceKey在任何时候都唯一
        /// </summary>
        public static   string deviceKey => EnvHelp.MachineUnqueKey;

        /// <summary>
        /// 每个服务站点的serviceStationKey在任何时候都唯一
        /// </summary>
        public static string serviceStationKey => EnvHelp.AppUnqueKey; 
     


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetEntryAssemblyVersion()
        {
            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.Assembly assembly = Assembly.GetEntryAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return version;
        }

    }
}
