package Sers.Core.Module.SsApiDiscovery;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

import Sers.Core.Module.Api.ApiDesc.SsApiDesc;
import Sers.Core.Module.Api.ApiDesc.SsModel;
import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Reflection.ReflectionHelp;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Module.SersDiscovery.DiscoveryConfig;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDescription;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsName;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsRoute;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsRoutePrefix;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsStationName;
import Sers.Core.Module.SsApiDiscovery.SersValid.SersValidMng;
import Sers.Core.Util.Common.CommonHelp;
import Sers.Core.Util.ConfigurationManager.ConfigurationManager;

public class SsApiDiscovery {
	
	
    public Map<String, LocalApiNode> apiMap;
    
    SsModelBuilder ssModelBuilder = new SsModelBuilder();
    
    public SsApiDiscovery(Map<String, LocalApiNode> apiMap)
    {
        this.apiMap = apiMap;
    }
    
    
    
    
 



	public void discovery(DiscoveryConfig config) throws Exception {

		String packageName=config.packageName;
		
		Class clazzController=IApiController.class;
		IApiController controller;
		 
		//(x.1) 获取Controller实体类
		List<Class> types = ReflectionHelp.getClasses(packageName);

		// (x.2)遍历Controller
		if (types == null)
			return;

		for (Class type : types) {
			try {
				
				if (!clazzController.isAssignableFrom(type))
					continue;

				controller = (IApiController) type.newInstance();
				if (controller == null)
					continue;
				
                 // (x.x.1) 获取stationNames
                List<String> stationNames = getStationName(config,type);
                
                
                //(x.x.2) 获取 routePrefix
                String routePrefix = getRoutePrefix(config,type);
                
				//region (x.x.3) 遍历method     
				Method[] methods = type.getMethods();//c.getDeclaredMethods()
		        for (Method method : methods ) {
		            //(x.x.x.1)  route
		        	ArrayList<String> routes = new ArrayList<String>();
               
                    discoveryApiRoutes(stationNames, routePrefix, method, routes);
                    if (routes.size() == 0) continue;                        
                    
                    
                    //(x.x.x.2) sampleApiDesc
                    SsApiDesc sampleApiDesc = discoveryApiDesc(method);
                    SsApiDesc apiDesc = sampleApiDesc;
                    

                    
                    //(x.x.x.3) 构建ApiNode
                    for(String route : routes) {
                        if (apiDesc == null) apiDesc = Serialization.Instance.convertBySerialize(sampleApiDesc,SsApiDesc.class);

                        apiDesc.route = route;
                        
                        LocalApiNode apiNode=GetApiNodeByMethod(apiDesc,method,clazzController,controller);
                        
                        apiMap.put(route, apiNode);
 
                        apiDesc = null;
                    }	            
		      
		        }					
				//endregion				

			} catch (Exception e) {
				// TODO Auto-generated catch block
				Logger.Error(e);	
				throw e;
			}			
			
		}
	}
	
	
 
	
    //region GetStationName

    protected  List<String> getStationName(DiscoveryConfig config, Class type)
    {
    	//（优先级从高到低：  apiStationName_Force 、 在代码上的SsStationNameAttribute特性指定 、 apiStationName 、 appsettings.json指定）

        List<String> stationNames= new ArrayList<String>();
        String stationName;

        //(x.1) apiStationName_Force
        stationName = config.apiStationName_Force;
        if (!CommonHelp.StringIsNullOrEmpty(stationName))
        {            
            stationNames.add(stationName);
            return stationNames;
        }


        //(x.2) 在代码上的SsStationNameAttribute特性指定
        SsStationName attrs = (SsStationName) type.getAnnotation(SsStationName.class);        
        if (null != attrs)             
        {
        	stationNames.add(attrs.value());
            return stationNames;
        }



        //(x.3) apiStationName
        stationName = config.apiStationName;
        if (!CommonHelp.StringIsNullOrEmpty(stationName))
        {
        	stationNames.add(stationName);
            return stationNames;
        }


        //(x.4)  appsettings.json指定
        String[] stations = ConfigurationManager.Instance.GetByPath("Sers.ApiStation.apiStationName",String[].class);
        if (stations!=null)
        {
        	return Arrays.asList(stations); 
        } 
        return null;

    }

    //endregion
    
    
    
    //region GetRoutePrefix

    protected String getRoutePrefix(DiscoveryConfig config,Class type)
    {
        //（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix）


        String routePrefix;


        //(x.1) routePrefix_Force
        routePrefix = config.routePrefix_Force;
        if (!CommonHelp.StringIsNullOrEmpty(routePrefix))
        {
            return routePrefix;
        }

        //(x.2) 在代码上的SsRoutePrefixAttribute特性指定
        SsRoutePrefix attrs = (SsRoutePrefix) type.getAnnotation(SsRoutePrefix.class);        
        if (null != attrs)             
        {        	 
            return attrs.value();
        }

        //(x.3) routePrefix
        routePrefix = config.routePrefix;
        if (!CommonHelp.StringIsNullOrEmpty(routePrefix))
        {
            return routePrefix;
        }
         
        return null;
    }
    //endregion
	
    
    
    //region DiscoveryApiRoutes
 
    protected void discoveryApiRoutes(List<String> stationNames, String routePrefix, Method  method, List<String> routes)
    {
        if (null == stationNames || stationNames.size() == 0) return;

        SsRoute[] attrs = method.getAnnotationsByType(SsRoute.class);
        if (null == attrs || attrs.length == 0) return;

        for(SsRoute attr : attrs){        	
         
        	String route = attr.value().trim();

            if (CommonHelp.StringIsNullOrEmpty(route)) continue;


            if (route.startsWith("/"))
            {
                // (x.1)  绝对路径。  /Station1/fold1/fold2/api1  
                routes.add(route);
            }
            else
            {
                // (x.2)  相对路径。  fold1/fold2/api1  
                for(String stationName : stationNames)
                {
                    //  /{stationName}/{routePrefix}/route

                	String absRoute = "/" + stationName;
                    if (!CommonHelp.StringIsNullOrEmpty(routePrefix))
                    {
                        absRoute += "/" + routePrefix;
                    }
                    absRoute += "/" + route;
                    routes.add(absRoute);
                }
            }
        }

    }

    //endregion
    
    
    
    //region DiscoveryApiDesc
 
    protected SsApiDesc discoveryApiDesc(Method method)
    {
    	SsApiDesc apiDesc = new SsApiDesc();
    	//(x.1) name from code
        apiDesc.name = CommonHelp.CallWhenNotNull(method.getAnnotation(SsName.class), m->m.value() );
        if(CommonHelp.StringIsNullOrEmpty(apiDesc.name)) {
        	apiDesc.name=method.getName();
        }

        //(x.2) description from code
        apiDesc.description = CommonHelp.CallWhenNotNull(method.getAnnotation(SsDescription.class), m->m.value() ); 

        //(x.3) rpcValidations from code
        apiDesc.rpcValidations = SersValidMng.getRpcValidationsFromMethod(method);    

        //region (x.4) ArgType        
        apiDesc.argType = ssModelBuilder.BuildSsModel_Arg(method);       
        //endregion
        
        
        //(x.5)ReturnType
        apiDesc.returnType = ssModelBuilder.BuildSsModel_Return(method);
       
        
        return apiDesc;
    }
    //endregion
    
    
    
    
	static LocalApiNode GetApiNodeByMethod(SsApiDesc apiDesc,Method method,Class clazzController,IApiController controller) {
		
		LocalApiNode apiNode=new LocalApiNode();
		
		apiNode.apiDesc=apiDesc;
		
		apiNode.init(method,controller); 
		return apiNode;
		
		
	}

}
