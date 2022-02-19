using System;
using System.Reflection;
using Vit.Core.Util.Extensible;

namespace Sers.SersLoader
{
    public class ApiLoaderConfig: Extensible
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


        #region assembly

        /// <summary>
        /// 在此Assembly中查找服务(assembly、assemblyFile、assemblyName 指定任一即可)
        /// </summary>
        public Assembly assembly
        {
            get => GetData<Assembly>("assembly");
            set => SetData("assembly", value);
        }


        /// <summary>
        /// 在此Assembly中查找服务(如 App.StationDemo.Station.dll)(assembly、assemblyFile、assemblyName 指定任一即可)
        /// </summary>
        public String assemblyFile
        {
            get => GetData<String>("assemblyFile");
            set => SetData("assemblyFile", value);
        }


        /// <summary>
        /// 在此Assembly中查找服务(如 App.StationDemo.Station)(assembly、assemblyFile、assemblyName 指定任一即可)
        /// </summary>
        public String assemblyName
        {
            get => GetData<String>("assemblyName");
            set => SetData("assemblyName", value);
        }
        #endregion



        /// <summary>
        /// 强制路由前缀,例如："demo/v1"。可不指定。（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix）
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


        /// <summary>
        /// 生命周期。可为 Scoped(每次请求都创建一个新的controller对象)、Singleton（请求共用一个在服务启动时初始化的controller对象）、Transient（同Scoped）。默认Singleton
        /// </summary>
        public string controllerLifetime
        {
            get => GetData<string>("controllerLifetime");
            set => SetData("controllerLifetime", value);
        }


    }
}
