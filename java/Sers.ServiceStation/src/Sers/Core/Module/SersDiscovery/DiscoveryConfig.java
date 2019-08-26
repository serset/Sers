package Sers.Core.Module.SersDiscovery;

import Sers.Core.Util.Extensible.Extensible;

public class DiscoveryConfig extends Extensible {
	
	//region (x.1)apiStationName_Force
	
	/**
	 * 强制指定ApiStation名称。可不指定。（优先级从高到低：  apiStationName_Force 、 在代码上的SsStationNameAttribute特性指定 、 apiStationName 、 appsettings.json指定）
	 */
	public String apiStationName_Force;	 
	
	// endregion
	

	// region (x.2)apiStationName
 
	/**
	 * ApiStation名称。可不指定。（优先级从高到低：  apiStationName_Force 、 在代码上的SsStationNameAttribute特性指定 、 apiStationName 、 appsettings.json指定）
	 */
	public String apiStationName;

	// endregion

	// region (x.3)packageName
	 
	 /**
	  * 在此package中查找服务(如 App.StationDemo.Station)(assembly、assemblyFile、assemblyName 指定任一即可)
	  */
	public String packageName;

	// endregion

	// region (x.4)routePrefix_Force

	/**
	 * 强制路由前缀,例如："demo/v1"。可不指定。（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix）
	 */
	public String routePrefix_Force;

	// endregion

	// region (x.5)routePrefix

	/**
	 * 路由前缀,例如："demo/v1"。可不指定。（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix ）
	 */
	public String routePrefix;

	// endregion
	

}
