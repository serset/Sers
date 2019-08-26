using Sers.Core.Module.App;
using Sers.Core.Util.Common;
using Sers.Core.Util.ConfigurationManager;
using Sers.Hardware.Hardware;
using System.Reflection;

namespace Sers.Core.Module.Env
{
    public class SersEnvironment
    {
        /// <summary>
        /// 每台机器的deviceKey在任何时候都唯一
        /// </summary>
        public static readonly string deviceKey = GetDeviceKey();

        /// <summary>
        /// 每个服务站点的serviceStationKey在任何时候都唯一
        /// </summary>
        public static readonly string serviceStationKey = GetServiceStationKey();




        static string GetDeviceKey()
        {
            //TOOD

            var file = new JsonFile("Data", "SersStationConfig.json");

            var key = file.GetStringByPath("env.deviceKey");

            if (string.IsNullOrEmpty(key))
            {
                key= "deviceKey" + CommonHelp.NewGuidLong();
                file.SetValueByPath(key, "env", "deviceKey");
            }           
            return key;
        }

       

        static string GetServiceStationKey()
        {
            //TOOD
            var file = new JsonFile("Data", "SersStationConfig.json");

            var key = file.GetStringByPath("env.serviceStationKey");

            if (string.IsNullOrEmpty(key))
            {
                key = "serviceStationKey" + CommonHelp.NewGuidLong();
                file.SetValueByPath(key, "env", "serviceStationKey");
            }
            return key;             
        }

     


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
