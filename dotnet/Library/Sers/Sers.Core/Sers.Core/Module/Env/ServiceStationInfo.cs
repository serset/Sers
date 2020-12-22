using Newtonsoft.Json.Linq;

namespace Sers.Core.Module.Env
{
    public class ServiceStationInfo
    {
        /// <summary>
        /// 站点key码，同一台机器上的同一个站点不变（根据机器码文件夹路径等计算得到）
        /// </summary>
        public string serviceStationKey;


        /// <summary>
        /// 站点版本信息
        /// </summary>
        public string stationVersion;

        /// <summary>
        /// 站点Name
        /// </summary>
        public string serviceStationName;

        /// <summary>
        /// 站点附加信息
        /// </summary>
        public JObject info;

    }
}
