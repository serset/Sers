package Sers.Core.Module.Rpc;

import java.io.File;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Common.CommonHelp;
import Sers.Core.Util.Common.JsonHelp;
import Sers.Core.Util.Data.ArraySegment;

//public interface IRpcContextData  {
//	   IRpcContextData UnpackOriData(ArraySegment  oriData);
//	   
//	   String route_Get();
//}

public class RpcContextData {

	JsonObject oriJson;

	public JsonElement ElementGetByPath(String... path) {
		if (oriJson == null)
			return null;

		return JsonHelp.ElementGetByPath(oriJson, path);
	}

	public void ValueSetByPath(String value, String... path) {
		if (oriJson == null)
			oriJson = new JsonObject();

		JsonHelp.ValueSetByPath(oriJson, value, path);
	}

	public void ValueSetByPath(JsonElement value, String... path) {
		if (oriJson == null)
			oriJson = new JsonObject();

		JsonHelp.ValueSetByPath(oriJson, value, path);
	}

	public String StringGetByPath(String... path) {
		if (oriJson == null)
			return null;

		return JsonHelp.StringGetByPath(oriJson, path);
	}

	
	
	
	public ArraySegment PackageOriData() {
		String strOriData = (oriJson == null ? "{}" : oriJson.toString());
		return new ArraySegment(Serialization.Instance.stringToBytes(strOriData));
	}

	public RpcContextData UnpackOriData(ArraySegment oriData) {

		try {
			oriJson=null;
			oriJson = Serialization.Instance.deserializeFromBytes(oriData, JsonObject.class);

		} catch (Exception e) {
			Logger.Error(e);
		}
		return this;
	}

	// region Init

	public RpcContextData Init(String callerSource) {
		String rid = CommonHelp.NewGuid();

		ValueSetByPath(rid, "caller", "rid");

		ValueSetByPath(callerSource, "caller", "source");

		return this;
	}

	// endregion

	// region InitFromRpcContext

	public RpcContextData InitFromRpcContext() {
		Init("Internal");

		RpcContextData rpcDataFromContext = RpcContext.RpcData_Get();
		if (null == rpcDataFromContext)
			return this;

		// region (x.2)caller_callStack
		String parentRid = rpcDataFromContext.StringGetByPath("caller", "rid");
		JsonArray callStack;

		JsonElement elem = rpcDataFromContext.ElementGetByPath("caller", "callStack");
		if (elem != null && elem.isJsonArray()) {
			callStack = elem.getAsJsonArray().deepCopy();
		} else {
			callStack = new JsonArray();
		}
		callStack.add(parentRid);
		ValueSetByPath(callStack, "caller", "callStack");
		// endregion

		// (x.3) http
//           ValueSetByPath(rpcDataFromContext.ElementGetByPath("http"),"http");

		// (x.4) user
		ValueSetByPath(rpcDataFromContext.ElementGetByPath("user"), "user");

		return this;
	}
	// endregion

	
//	   public JsonObject oriJson_Get(){
//		   return null;
//	   }

//		   public byte[] Serialize() {
//		   return null;
//	   }

	
	// region 常用扩展

	// region (x.x.1) route
	public void route_Set(String route) {
		ValueSetByPath(route, "route");
	}

	public String route_Get() {
		return StringGetByPath("route");
	}
	// endregion

	// region (x.x.2) http_url
	public void http_url_Set(String url) {
		ValueSetByPath(url, "http", "url");
	}

	public String http_url_Get() {
		return StringGetByPath("http", "url");
	}
	// endregion
	
	
	 //region http_url_search_Get
	
	/**
	 * 获取泛接口 路由 * 实际传递的内容。<br/>
     * （若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则search为"1/2.html?c=9"）
	 * @return
	 */
     public String http_url_search_Get()
     {
         // "http://127.0.0.1/Station1/fold1/a/1/2.html?c=9"
    	 String http_url = http_url_Get();

         // "/Station1/fold1/a/*"
    	 String route = route_Get();

         int index = http_url.indexOf(route.substring(0, route.length() - 1));
         if (index < 0) return null;

         return http_url.substring(index + route.length() - 1);   
     }
     //endregion
	
     
     
     //region http_url_RelativePath_Get
 
	/**
     * 获取当前url对应的相对文件路径(若不合法，则返回null)。demo:"rpc/2.html"
     * （若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则 relativePath为"1/2.html"）
     *  route: "/DemoStation/v1/api/5/*"
     * @return
      */
     public String http_url_RelativePath_Get()
     {
         // "1/2.html?c=9"
    	 String search = http_url_search_Get();
         if (CommonHelp.StringIsNullOrEmpty(search)) return null;
         int index = search.indexOf('?');

         String relativePath= search;
         if (index >= 0)
         {
             relativePath= relativePath.substring(0,index);
         }
         return relativePath.replace('/',  File.separatorChar);
     }
     //endregion
     
     
     //region http_headers
     public JsonObject http_headers_Get()
     {
    	 JsonElement elem= ElementGetByPath("http","headers");
    	 if(elem!=null && elem.isJsonObject()) return elem.getAsJsonObject();    	 
         return null;
     }
     
     public String http_header_Get( String key)
     {
         return StringGetByPath("http","headers",key); 
     }
     public  void http_header_Set(String key,String value)
     {
        ValueSetByPath(value,"http","headers",key);
     }
     
     public  void http_headers_Set(JsonObject value)
     {
        ValueSetByPath(value,"http","headers");
     }
    
     //endregion
     

	// endregion

}