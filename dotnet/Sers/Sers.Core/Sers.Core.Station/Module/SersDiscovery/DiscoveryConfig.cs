using Sers.Core.Util.Extensible;
using System.Collections.Generic;
using System.Reflection;

namespace Sers.Core.Module.SersDiscovery
{
    public class DiscoveryConfig: Extensible
    {

        /// <summary>
        /// 强制指定ApiStation名称。可不指定。（优先级从高到低：  apiStationName_Force 、 在代码上的SsStationNameAttribute特性指定 、 apiStationName 、 appsettings.json指定）
        /// </summary>
        public string apiStationName_Force
        {
            get => GetData<string>("apiStationName_Force");
            set => SetData("apiStationName_Force", value);
        }

        /// <summary>
        /// ApiStation名称。可不指定。（优先级从高到低：  apiStationName_Force 、 在代码上的SsStationNameAttribute特性指定 、 apiStationName 、 appsettings.json指定）
        /// </summary>
        public string apiStationName
        {
            get => GetData<string>("apiStationName");
            set => SetData("apiStationName", value);
        }

        /// <summary>
        /// 在此Assembly中查找Api
        /// </summary>
        public Assembly assembly
        {
            get => GetData<Assembly>("assembly");
            set => SetData("assembly", value);
        }

        /// <summary>
        /// 强制路由前缀,例如："demo/v1"。可不指定。（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix ）
        /// </summary>
        public string routePrefix_Force
        {
            get => GetData<string>("routePrefix_Force");
            set => SetData("routePrefix_Force", value);
        }


        /// <summary>
        /// 路由前缀,例如："demo/v1"。可不指定。（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix ）
        /// </summary>
        public string routePrefix
        {
            get => GetData<string>("routePrefix");
            set => SetData("routePrefix", value);
        }


    }
}
