package StationDemo.Controllers;

import java.lang.reflect.Type;
import java.util.HashMap;
import java.util.List;

import com.google.gson.JsonObject;
import com.google.gson.reflect.TypeToken;

import Sers.Core.Module.Api.ApiClient;
import Sers.Core.Module.Api.Data.ApiReturn;
import Sers.Core.Module.Api.Data.ApiReturnBase;
import Sers.Core.Module.Message.ApiMessage;
import Sers.Core.Module.Rpc.RpcContextData;
import Sers.Core.Module.Rpc.RpcFactory;
import Sers.Core.Module.Rpc.RpcContext;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Module.SsApiDiscovery.IApiController;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsArgEntity;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsArgProperty;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDefaultValue;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDescription;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsExample;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsName;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsReturn;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsRoute;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsRoutePrefix;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsStationName;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsCallerSource;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsValidation;
import Sers.Core.Util.Data.ArraySegment;

//站点名称，可多个。若不指定，则从 配置文件的节点"Sers.Station.Name"获取
@SsStationName("JDemoStation")
//路由前缀，可不指定
@SsRoutePrefix("v1")
public class SampleController implements IApiController {

	// region (x.1)route

	@SsRoute("1/route/1")
	@SsRoute("1/route/2")
	@SsRoute("/JDemoStation/v1/1/route/3") // 使用绝对路径路由
	@SsDescription("route示例")
	public ApiReturnBase Route() {
		return new ApiReturn("hello world!");
	}

	@SsRoute("1/route/4/*") // 使用泛接口路径路由
	@SsDescription("route示例-泛接口")
	public ApiReturn<Object> Route4() {
		RpcContextData rpcData = RpcContext.RpcData_Get();
		String route = rpcData.route_Get();
		String http_url = rpcData.http_url_Get();
		String http_url_search = rpcData.http_url_search_Get();
		String http_url_RelativePath = rpcData.http_url_RelativePath_Get();
		HashMap<String, String> data = new HashMap();

		data.put("route", route);
		data.put("http_url", http_url);
		data.put("http_url_search", http_url_search);
		data.put("http_url_RelativePath", http_url_RelativePath);

		return new ApiReturn<Object>(data);
	}

	// endregion

	// region (x.2)Name和Desc

	@SsRoute("2/NameDesc/1")
	@SsName("NameDesc1")
	@SsDescription("演示 如何使用Name 和 Description")
	public ApiReturnBase NameDesc() {
		return new ApiReturnBase();
	}

	@SsRoute("2/NameDesc/2")
	@SsDescription("演示 如何使用Name 和 Description")
	public ApiReturnBase NameDesc2() {
		return new ApiReturnBase();
	}
	// endregion

	// region (x.3)参数

	// region (x.x.1)无参
	@SsRoute("3/arg/10")
	@SsDescription("arg1")
	@SsReturn(example = "{\"success\":true}", description = "return-description")
	public ApiReturn<String> Arg10() {
		return new ApiReturn("hello world!");
	}
	// endregion

	// region (x.x.2)首个参数为参数实体

	@SsRoute("3/arg/21")
	@SsDescription("首个参数为参数实体（引用类型）")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<ArgModel> Arg21(
			@SsExample("{\"arg\":\"arg\",\"arg2\":\"arg2\"}") @SsDefaultValue("{\"arg\":\"arg00\",\"arg2\":\"arg02\"}") @SsDescription("argDescription") ArgModel args) {
		return new ApiReturn<ArgModel>(args);
	}

	@SsRoute("3/arg/22")
	@SsDescription("首个参数为参数实体（引用类型）")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<Object> Arg22(
			@SsExample("[\"arg1\",\"arg2\"]") @SsDefaultValue("[\"arg1\",\"arg2\"]") @SsDescription("argDescription") String[] args) {
		return new ApiReturn(args);
	}

	@SsRoute("3/arg/23")
	@SsDescription("标识函数第一个参数为Api的参数实体。（忽略其他参数(若存在)，调用函数时其他参数(若存在)为空）")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<Object> Arg23(
			@SsExample("argExample") @SsDefaultValue("argDefaultValue") @SsDescription("argDescription") @SsArgEntity String args) {
		return new ApiReturn(args);
	}

	@SsRoute("3/arg/24")
	@SsDescription("标识函数第一个参数为Api的参数实体。（忽略其他参数(若存在)，调用函数时其他参数(若存在)为空）")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<Object> Arg24(@SsArgEntity String args, String arg2) {
		return new ApiReturn(args);
	}

	// endregion

	// region (x.x.3)函数参数列表为参数实体

	@SsRoute("3/arg/31")
	@SsDescription("函数参数列表为参数实体")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<Object> Arg31(
			@SsName("arg1") @SsExample("argExample") @SsDefaultValue("argDefaultValue") @SsDescription("argDescription") String arg1) {
		return new ApiReturn(arg1);
	}

	@SsRoute("3/arg/32")
	@SsDescription("函数参数列表为参数实体")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<ArgModel> Arg32(
			@SsName("arg1") @SsExample("argExample") @SsDefaultValue("argDefaultValue") @SsDescription("argDescription") String arg1,
			@SsName("arg2") @SsExample("argExample2") @SsDefaultValue("argDefaultValue2") @SsDescription("argDescription2") String arg2) {
		ArgModel ret = new ArgModel();
		ret.arg1 = arg1 + arg2;
		ret.arg2 = 3;
		return new ApiReturn<ArgModel>(ret);
	}

	@SsRoute("3/arg/33")
	@SsDescription("函数参数列表为参数实体")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<Object> Arg33(@SsName("arg1") @SsDescription("arg1注释") ArgModel arg1,
			@SsName("arg2") @SsExample("6") @SsDefaultValue("1") @SsDescription("arg2注释") int arg2) {
		return new ApiReturn(arg1);
	}

	@SsRoute("3/arg/34")
	@SsDescription("函数第一个参数为Api的参数实体的属性之一。（一般放置在第一个参数上，代表第一个参数不为参数实体）")
	@SsReturn(description = "ArgModelDesc-returns")
	public ApiReturn<Object> Arg33(@SsName("arg1") @SsArgProperty @SsDescription("arg1注释") ArgModel arg1) {
		return new ApiReturn(arg1);
	}

	// endregion

	public class ArgModel {
		/**
		 * arg1Desc-xml
		 */
		@SsDescription("arg1-Description")
		@SsExample("example1")
		@SsDefaultValue("default1")
		public String arg1;

		/**
		 * arg2Desc-xml
		 */
		@SsDescription("arg2-Description")
		@SsExample("20")
		@SsDefaultValue("2")
		public int arg2;
	}

	// endregion

	// region (x.4)返回值

	@SsRoute("4/ret/2")
	@SsDescription("Return2")
	@SsReturn(example = "{\"data\":12}", description = "成功个数")
	public ApiReturn<Integer> Return2() {
		return new ApiReturn<Integer>(5);
	}

	@SsRoute("4/ret/3")
	@SsDescription("Return3")
	@SsReturn(example = "{\"data\":[\"11\",\"12\"]}", description = "字符串数组")
	public ApiReturn<String[]> Return3() {
		return new ApiReturn<String[]>(new String[] { "1", "2" });
	}

	@SsRoute("4/ret/4")
	@SsDescription("Return4")
	@SsReturn(example = "test", description = "测试")
	public String Return4() {
		return "test1";
	}

	@SsRoute("4/ret/5")
	@SsDescription("Return5")
	@SsReturn(example = "5", description = "测试")
	public int Return5() {
		return 12;
	}

	@SsRoute("4/ret/6")
	@SsDescription("Return6")
	@SsReturn(example = "{\"arg1\":\"arg1\"}", description = "测试")
	public ReturnData Return6() {
		return new ReturnData();
	}

	public class ReturnData {
		@SsDescription("arg1-Description")
		@SsExample("example1")
		@SsDefaultValue("default1")
		public String arg1 = "default1";
	}

	// endregion

	// region (x.5) Rpc

	@SsRoute("5/rpc/GetRpcOriData")
	@SsDescription("rpc示例")
	public ApiReturn<Object> GetRpcOriData() {
		return new ApiReturn<Object>(RpcContext.RpcData_Get());
	}

	@SsRoute("5/rpc/RpcReplyDemo")
	@SsDescription("rpc示例")
	public ApiReturn<Object> RpcReplyDemo(@SsArgEntity @SsExample("requestContent") String request) {

		// region reply header
		RpcContextData replyRpcData = RpcFactory.Instance.CreateRpcContextData.create();

		JsonObject header = new JsonObject();

		header.addProperty("testHeader", "abc");
		header.addProperty("Content-Type", "application/json");

		replyRpcData.http_headers_Set(header);
		RpcContext.Current_Get().apiReplyMessage.rpcContextData_OriData_Set(replyRpcData.PackageOriData());
		// endregion

		HashMap<String, Object> data = new HashMap<>();

		data.put("RpcData", RpcContext.RpcData_Get());
		data.put("request", request);

		return new ApiReturn<Object>(data);

	}

	// endregion

	// region (x.6) ApiClient

	@SsRoute("6/ApiClient/1")
	@SsDescription("ApiClient示例,调用其他接口")
	public String ApiClient1(@SsName("apiRoute") @SsExample("/JDemoStation/v1/3/arg/32") String apiRoute,
			@SsName("arg") @SsExample("{\"arg1\":\"dd\",\"arg2\":\"33\"}") Object arg) {

		Type type = new TypeToken<ApiReturn<ArgModel>>() {
		}.getType();
		ApiReturn<ArgModel> apiRet = ApiClient.Instance.CallApi(apiRoute, arg, type);
		return Serialization.Instance.serializeToString(apiRet);

//		return ApiClient.Instance.CallApi(apiRoute, arg);
	}

	// region (x.x.2) ApiClient示例,传输二进制数据

	@SsRoute("6/ApiClient/2_1")
	@SsDescription("ApiClient示例,调用其他接口")
	public byte[] ApiClient2_1(@SsExample("1122AAFF") @SsDescription("二进制数据") byte[] ba)
	// 使用ArraySegmen 会减少一步复制的操作，若传输大数据建议使用ArraySegment
	{

		return ba;
	}

	@SsRoute("6/ApiClient/2_2")
	@SsDescription("ApiClient示例,传输二进制数据")
	public ArraySegment ApiClient2_2(@SsExample("1122AAFF") @SsDescription("二进制数据") ArraySegment ba)
	// 使用ArraySegmen 会减少一步复制的操作，若传输大数据建议使用ArraySegment
	{
		return ba;
	}

	@SsRoute("6/ApiClient/2_3")
	@SsDescription("ApiClient示例,传输二进制数据")
	public String ApiClient2_3() {
		Object apiRet = ApiClient.Instance.CallApiToBytes("/JDemoStation/v1/6/ApiClient/2_1",
				new byte[] { 3, 2, 3, 4, 5 });
		return Serialization.Instance.serializeToString(apiRet);
	}
	// endregion

	// #region (x.x.3) ApiClient示例,传输额外数据

	@SsRoute("6/ApiClient/3_1")
	@SsDescription("ApiClient示例,二进制数据和ReplyRpc")
	public ApiReturnBase ApiClient3_1() {
		ApiMessage apiReply = RpcContext.Current_Get().apiReplyMessage;

		byte[] file1 = new byte[] { 1, 2, 3, 4, 5 };
		byte[] file2 = new byte[] { 2, 3, 3, 4, 5 };

		List<ArraySegment> files=apiReply.Files_Get();
		files.add(new ArraySegment(file1));
		files.add(new ArraySegment(file2));	
//		apiReply.AddFile(new ArraySegment(file2));

		// region file from request
		ApiMessage apiRequest = RpcContext.Current_Get().apiRequestMessage;
		if (apiRequest.Files_Get().size() >= 3) {
			ArraySegment file3 = apiRequest.Files_Get().get(2);
			apiReply.AddFile(file3);
		}
		// endregion

		apiReply.rpcContextData_OriData_Set(RpcContext.Current_Get().apiRequestMessage.rpcContextData_OriData_Get());

		return new ApiReturnBase();
	}

	@SsRoute("6/ApiClient/3_2")
	@SsDescription("ApiClient示例,二进制数据和ReplyRpc")
	public ApiReturnBase ApiClient3_2() {
		
		ApiMessage apiRequestMessage = new ApiMessage().InitAsApiRequestMessage("/JDemoStation/v1/6/ApiClient/3_1",
				null);

		byte[] fileToSend = new byte[] { 3, 2, 3, 4, 5 };
		apiRequestMessage.AddFile(new ArraySegment(fileToSend));

		ApiMessage apiReplyMessage = ApiClient.Instance.CallApi(apiRequestMessage);

		List<ArraySegment> files = apiReplyMessage.Files_Get();
		ArraySegment file1 = files.get(2);
		ArraySegment file2 = files.get(3);
		ArraySegment file3 = files.get(4);

//      RpcContextData replyRpcData = RpcFactory.Instance.CreateRpcContextData.create();
//      replyRpcData.UnpackOriData(apiReplyMessage.rpcContextData_OriData_Get());     

		return new ApiReturnBase();
	}

	// endregion

	// endregion
	
	
	//region (x.8) Rpc Valid
	@SsRoute("8/valid/1")
	@SsDescription("自定义限制 SsValidation")
	@SsValidation(path = "http.method",	ssValid = "{\"type\":\"Equal\",\"value\":\"PUT\"}",
	errorMessage = "只接受PUT请求",errorCode=1000)
	public ApiReturnBase Valid() {
		return new ApiReturnBase();
	}
	
	
	@SsRoute("8/valid/2")
	@SsDescription("自定义限制 SsValidation")
	@SsValidation(path = "caller.source",	ssValid = "{\"type\":\"Equal\",\"value\":\"Internal\"}",
	errorMessage = "权限限制(没有权限)",errorCode=405)
	@SsValidation(path = "caller.source",	ssValid = "{\"type\":\"Required\"}",
	errorMessage = "权限限制(没有权限)",errorCode=405)
	public ApiReturnBase Valid2() {
		return new ApiReturnBase();
	}
	
	
	@SsRoute("8/valid/3/*")
	@SsDescription("自定义限制 SsValidation")
	@SsValidation(path = "http.url",	ssValid = "{\"type\":\"Regex\",\"value\":\"(?:\\\\.html)$\"}",
	errorMessage = "url后缀必须为 .html",errorCode=1000)
	public ApiReturnBase Valid3() {
		return new ApiReturnBase();
	}
	
	

	@SsRoute("8/valid/4")
	@SsDescription("自定义限制 SsValidation")
	@SsCallerSource(SsCallerSource.Internal)
	public ApiReturnBase Valid4() {
		return new ApiReturnBase();
	}
	
	//endregion

}
