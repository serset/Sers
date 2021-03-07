using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.RpcVerify;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.Net.Http.FormFile;

namespace Did.SersLoader.Demo.Controllers.Demo
{
    //站点名称，可多个。若不指定，则从 配置文件中LocalApiService中的"apiStationNames"获取
    [SsStationName("demo")]
    //[SsStationName("demo2"), SsStationName("demo3")]
    //路由前缀，可不指定
    [SsRoutePrefix("v1/api")]
    public class SampleController
        : IApiController
    {



        #region (x.1)route


        /// <summary>
        /// route示例
        /// </summary>
        /// <returns></returns>
        [SsRoute("101/route"), SsRoute("102/route")]
        [SsRoute("/demo/v1/api/103/route")] //使用绝对路径路由  
        public ApiReturn<object> Route()
        {
            return new { url = RpcContext.RpcData.http_url_Get() };
        }



        /// <summary>
        /// route示例-泛接口
        /// </summary>
        /// <returns></returns>

        [SsRoute("104/route/*")] //使用泛接口路径路由
        public ApiReturn<object> Route4()
        {
            var rpcData = RpcContext.RpcData;
            var route = rpcData.route;
            var http_url = rpcData.http_url_Get();
            var http_url_search = rpcData.http_url_search_Get();
            var http_url_RelativePath = rpcData.http_url_RelativePath_Get();
            var http_method = rpcData.http_method_Get() ;

            var data = new
            {
                route,
                http_url,
                http_url_search,
                http_url_RelativePath,
                http_method
            };

            return new ApiReturn<object>(data);
        }

        /// <summary>
        /// HttpMethod示例GET
        /// </summary>
        /// <returns></returns>
        [SsRoute("105/route",HttpMethod ="GET")]
        public ApiReturn<object> Route5_Get()
        {
            return new { apiName = "Route5_Get", rpcData = RpcContext.RpcData };
        }

        /// <summary>
        /// HttpMethod示例POST
        /// </summary>
        /// <returns></returns>
        [SsRoute("105/route", HttpMethod = "POST")]
        public ApiReturn<object> Route5_POST()
        {
            return new { apiName = "Route5_POST", rpcData = RpcContext.RpcData };
        }



        #endregion


        #region (x.2)Name和Desc

        /// <summary>
        /// 演示 如何使用Name 和 Description。
        /// 函数注释和使用SsDescription是一样的效果。
        /// </summary>
        /// <returns></returns>
        [SsRoute("201/NameDesc")]
        [SsName("NameDesc1")]
        public ApiReturn NameDesc()
        {
            return new ApiReturn();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("202/NameDesc")]
        [SsDescription("演示 如何使用Name 和 Description。\n函数注释和使用SsDescription是一样的效果。")]
        public ApiReturn NameDesc2()
        {
            return new ApiReturn();
        }
        #endregion

        #region (x.3)参数

        #region (x.x.1)无参

        /// <summary>
        /// 无参
        /// </summary>
        /// <returns></returns>
        [SsRoute("310/arg")]
        public ApiReturn Arg301()
        {
            return new ApiReturn();
        }
        #endregion

        #region (x.x.2)首个参数为参数实体

        /// <summary>
        ///  首个参数为参数实体（引用类型）
        /// </summary>
        /// <param name="args">arg1注释</param>
        /// <returns>ArgModelDesc-returns</returns>
        [SsRoute("320/arg")]
        public ApiReturn<Object> Arg320(
             [SsExample("{\"arg\":\"arg\",\"arg2\":\"arg2\"}"), SsDefaultValue("{\"arg\":\"arg\",\"arg2\":\"arg2\"}"), SsDescription("argDescription")]ArgModel args
            )
        {
            return args;
        }

        /// <summary>
        /// 首个参数为参数实体（引用类型）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [SsRoute("321/arg")]
        public ApiReturn<Object> Arg321(
             [SsExample("[\"arg1\",\"arg2\"]"), SsDefaultValue("[\"arg1\",\"arg2\"]"), SsDescription("argDescription")]string[] args)
        {
            return args;
        }

        /// <summary>
        /// 标识函数第一个参数为Api的参数实体。（忽略其他参数(若存在)，调用函数时其他参数(若存在)为空）
        /// </summary>
        /// <param name="args">args注释</param>
        /// <returns></returns>
        [SsRoute("322/arg")]
        public ApiReturn<Object> Arg322(
            [SsExample("argExample"), SsDefaultValue("argDefaultValue"), SsDescription("argDescription"),SsArgEntity]string args)
        {
            return args;
        }

        /// <summary>
        /// 标识函数第一个参数为Api的参数实体。（忽略其他参数(若存在)，调用函数时其他参数(若存在)为空）
        /// </summary>
        /// <param name="args"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        [SsRoute("323/arg")]
        public ApiReturn<Object> Arg323([SsArgEntity]string  args,string arg2)
        {
            return args;
        }


        #endregion

        #region (x.x.3)函数参数列表为参数实体


        /// <summary>
        /// 函数参数列表为参数实体
        /// </summary>
        /// <param name="arg1"></param>
        /// <returns></returns>
        [SsRoute("330/arg")]
        public ApiReturn<Object> Arg330([SsExample("argExample"), SsDefaultValue("argDefaultValue"), SsDescription("argDescription")]string arg1)
        {
            return new { arg1 };
        }


        /// <summary>
        /// 函数参数列表为参数实体
        /// </summary>
        /// <param name="arg1">arg1注释</param>
        /// <param name="arg2">arg2注释</param>
        /// <returns></returns>
        [SsRoute("331/arg")]
        public ApiReturn<Object> Arg331(
            [SsExample("example1"), SsDefaultValue("default1"), SsDescription("desc1")]string arg1,
            [SsExample("6"), SsDefaultValue("1")]int arg2)
        {
            return new { arg1 , arg2 };
        }





        /// <summary>
        /// 函数参数列表为参数实体
        /// </summary>
        /// <param name="arg1">arg1注释</param>
        /// <param name="arg2">arg2注释</param>
        /// <returns></returns>
        [SsRoute("332/arg")]
        public ApiReturn<Object> Arg332(ArgModel arg1,
            [SsExample("6"), SsDefaultValue("1")]int arg2)
        {
            return new { arg1, arg2 };
        }


        /// <summary>
        /// 函数第一个参数为Api的参数实体的属性之一。（一般放置在第一个参数上，代表第一个参数不为参数实体）
        /// </summary>
        /// <param name="arg1">arg1注释</param>     
        /// <returns></returns>
        [SsRoute("333/arg")]
        public ApiReturn<Object> Arg333([SsArgProperty]ArgModel arg1)
        {
            return new { arg1};
        }
        #endregion


        public class ArgModel
        {
            /// <summary>
            /// 姓名
            /// </summary>
            [SsExample("lith"), SsDefaultValue("NoName")]
            [JsonProperty("arg")]
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [SsExample("20"), SsDefaultValue("0"), SsDescription("年龄，请指定在16-50中间的整数")]
            public int age;
        }

        #endregion


        #region (x.4)返回值
        /// <summary>
        /// Return1
        /// </summary>
        /// <returns>Return注释-FromXml</returns>
        [SsRoute("401/ret")]
        public ApiReturn<ReturnData> Return1()
        {
            return new ReturnData { name = "张三" };
        }

        /// <summary>
        /// Return2
        /// </summary>
        /// <returns></returns>
        [SsRoute("402/ret")]
        [return: SsExample("{\"data\":12}"), SsDescription("成功个数")]
        public ApiReturn<int> Return2()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("403/ret")]
        public ApiReturn<List<string>> Return3()
        {
            return new List<string>{ "1","2"};
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("404/ret")]
        [return: SsExample("test"), SsDescription("返回test1")]
        public string Return4()
        {
            return "test1";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("405/ret")]
        [return: SsExample("5"), SsDescription("返回5")]
        public int Return5()
        {
            return 5;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("406/ret")]
        [return: /*SsExample("{\"name\":\"张三\"}"),*/ SsDescription("返回模型数据")]
        public ReturnData Return6()
        {
            return new ReturnData { name = "张三" };
        }


        /// <summary>
        /// Return注释-FromType
        /// </summary>
        public class ReturnData
        {

            /// <summary>
            /// 姓名
            /// </summary>
            [SsExample("lith"), SsDefaultValue("NoName")]
            public string name { get; set; }
        }

        #endregion


        #region (x.4-2)指定参数或返回值类型
        /// <summary>
        /// 指定参数或返回值类型
        /// </summary>
        /// <returns></returns>
        [SsRoute("409/type")]
        [return: SsType(typeof(List<int>)), SsExample("[5,6]"), SsDescription("返回原值")]
        public string Type(
            [SsType(typeof(List<string>)), SsExample("[5,6]"), SsDescription("id数组")]string ids
            )
        {
            return ids;
        }
        #endregion



        #region (x.5) Rpc

        /// <summary>
        /// rpc示例
        /// </summary>
        /// <returns></returns>

        [SsRoute("501/GetRpcOriData")]
        public ApiReturn<Object> GetRpcOriData()
        {
            return new ApiReturn<Object>(RpcContext.RpcData);
        }


        /// <summary>
        /// rpc示例
        /// </summary>
        /// <returns></returns>
        [SsRoute("502/RpcDemo")]
        public ApiReturn<Object> RpcDemo()
        {
            var rpcData = RpcContext.RpcData;

            var data = new
            {
                rpcData.route,
                http_url = rpcData.http_url_Get(),
                userInfo = rpcData.user_userInfo_Get(),
                serviceId= DateTime.Now.ToString("sss")
            };

            return new ApiReturn<Object>(data);
        }


        /// <summary>
        /// rpc示例
        /// </summary>
        /// <returns></returns>

        [SsRoute("503/RpcReplyDemo")]
        public ApiReturn<Object> RpcReplyDemo([SsArgEntity,SsExample("requestContent")]string request)
        {
            #region reply header

            RpcContext.Current.apiReplyMessage.rpcContextData_OriData =
                    RpcFactory.CreateRpcContextData()
                    .http_statusCode_Set(201)
                    //.http_header_Set("Content-Type", "application/json")
                    .http_header_ContentType_Set("application/json")
                    .http_header_Set("testHeader", "abc")
                    .PackageOriData();


            //var replyRpcData = RpcFactory.Instance.CreateRpcContextData();

            //var header = new JObject();

            //header["testHeader"] = "abc";
            //header["Content-Type"] = "application/json";s

            //replyRpcData.http_headers_Set(header);

            //RpcContext.Current.apiReplyMessage.rpcContextData_OriData = replyRpcData.PackageOriData();
            #endregion

            var data = new
            {
                RpcContext.RpcData,
                request
            };

            return data;
        }


        #endregion


        #region (x.6) ApiClient


        #region (x.x.1)ApiClient示例,调用其他接口
        /// <summary>
        /// ApiClient示例,调用其他接口
        /// </summary>
        /// <returns></returns>

        [SsRoute("610/ApiClient")]
        public ApiReturn<Object> ApiClient610(
            [SsExample("/demo/v1/api/332/arg")]string apiRoute, 
            [SsExample("{\"arg1\":\"dd\",\"arg2\":\"33\"}")]object arg)
        {
            var apiRet = ApiClient.CallRemoteApi<ApiReturn<Object>>(apiRoute, arg);
            return new ApiReturn<Object>(new
            {
                RpcContext.RpcData,
                apiRet
            });
        }
        #endregion

        #region (x.x.2) ApiClient示例,传输二进制数据

        /// <summary>
        /// ApiClient示例,传输二进制数据
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        [SsRoute("620/ApiClient")]
        [SsName("ApiClient demo")]
        [return:SsExample("1122AAFF"), SsDescription("二进制数据")]
        public byte[] ApiClient620( 
            [SsExample("1122AAFF"), SsDescription("二进制数据")]byte[] ba)    // 使用ArraySegment<byte> 会减少一步复制的操作，若传输大数据建议使用ArraySegment<byte>
        {
            return ba;
        }

        /// <summary>
        /// ApiClient示例,传输二进制数据
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        [SsRoute("621/ApiClient")]
        [SsName("ApiClient demo")]
        [return: SsExample("1122AAFF"), SsDescription("二进制数据")]
        public ArraySegment<byte> ApiClient621(
            [SsExample("1122AAFF"), SsDescription("二进制数据")]ArraySegment<byte> ba)
        {
            return ba;
        }

        /// <summary>
        /// ApiClient示例,传输二进制数据
        /// </summary>
        /// <returns></returns>
        [SsRoute("622/ApiClient")]
        public ApiReturn ApiClient622()
        {
            var apiRet = ApiClient.CallRemoteApi<ArraySegment<byte>>("/demo/v1/api/621/ApiClient", new byte[] { 3, 2, 3, 4, 5 });
            return new ApiReturn();
        }
        #endregion

        #region (x.x.3) ApiClient示例,传输额外数据

        /// <summary>
        /// ApiClient示例,二进制数据和ReplyRpc
        /// </summary>
        /// <returns></returns>
        [SsRoute("630/ApiClient")]
        public ApiReturn ApiClient630()
        {
            var apiReply = RpcContext.Current.apiReplyMessage;

            var file1 = new byte[] { 1, 2, 3, 4, 5 };
            var file2 = new byte[] { 2, 3, 3, 4, 5 };
            apiReply.AddFiles(file1.BytesToArraySegmentByte(), file2.BytesToArraySegmentByte());

            #region file from request
            var apiRequest = RpcContext.Current.apiRequestMessage;
            if (apiRequest.Files.Count >= 3)
            {
                var file3 = apiRequest.Files[2];
                apiReply.AddFiles(file3);
            }
            #endregion


            apiReply.rpcContextData_OriData = RpcContext.Current.apiRequestMessage.rpcContextData_OriData;

            return new ApiReturn();
        }

        /// <summary>
        /// ApiClient示例,传输二进制数据和ReplyRpc
        /// </summary>
        /// <returns></returns>
        [SsRoute("631/ApiClient")]
        public ApiReturn ApiClient3_2()
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage("/demo/v1/630/ApiClient", null);

            var fileToSend = new byte[] { 3, 2, 3, 4, 5 };
            apiRequestMessage.AddFiles(fileToSend.BytesToArraySegmentByte());

            var apiReplyMessage = ApiClient.CallRemoteApi(apiRequestMessage);


            var file1 = apiReplyMessage.Files[2];
            var file2 = apiReplyMessage.Files[3];
            var file3 = apiReplyMessage.Files[4];

            //var replyRpcData = RpcFactory.Instance.CreateRpcContextData();
            //replyRpcData.UnpackOriData(apiReplyMessage.rpcContextData_OriData);

            return new ApiReturn();
        }
        #endregion

        #endregion


        #region (x.7) UseStaticFiles

        // wwwroot 路径从配置文件获取
        static StaticFileMap staticFileMap = new StaticFileMap(ConfigurationManager.Instance.GetByPath<StaticFilesConfig>("Demo.staticFiles"));

        /// <summary>
        /// UseStaticFiles
        /// </summary>
        /// <returns></returns>
        [SsRoute("700/static/*")]
        public byte[] UseStaticFiles()
        {
            return staticFileMap.TransmitFile();
        }
        #endregion



        #region (x.8) RpcVerify

        #region (x.8.1) 自定义限制 SsRpcVerify
        /// <summary>
        /// 810自定义限制 SsRpcVerify
        /// </summary>
        /// <returns></returns>
        [SsRoute("810/SsRpcVerify")]
        [SsRpcVerify( condition = "{\"type\":\"==\",\"path\":\"http.method\",\"value\":\"PUT\"}", verifiedWhenNull = false , errorMessage = "只接受PUT请求", errorCode = 1000)]
        // condition：   SsExp,参考 rpcVerify2.md
        // path：        RpcData中被验证的值，（参考 RpcContextData.md）。如 http.url 、http.method 、 http.headers.Authorization
        // verifiedWhenNull: 当出现空值时，是否通过验证（默认不通过，false）。例如，为fakse时，若没有指定值http.method，则禁止调用接口，返回错误消息；
        //                                                                      为true时，在没有指定http.method时可以调用哦接口
        //
        // errorMessage:    校验不通过时的提示消息，若不指定则使用默认提示消息
        // errorCode:       校验不通过时的errorCode, 如 1000。可不指定
        public ApiReturn SsRpcVerify()
        {
            return new ApiReturn();
        }
        #endregion



        #region (x.8.2) CallerSource

        /// <summary>
        /// 820限制只可内部调用
        /// </summary>
        /// <returns></returns>
        [SsRoute("820/CallerSource")]
        [SsNotEqual(path = "caller.source", value = "Internal", errorMessage = "无权限")]
        //[SsNotEqual(type="!=",path = "caller.source", value = "Internal", errorMessage = "无权限")]
        public ApiReturn CallerSource820()
        {
            return new ApiReturn();
        }


        /// <summary>
        /// 821限制只可内部调用
        /// </summary>
        /// <returns></returns>
        [SsRoute("821/CallerSource")]
        //[SsCallerSource(ECallerSource.Internal)]
        //[SsCallerSource(ECallerSource.Internal| ECallerSource.OutSide)]
        [SsCallerSource(callerSourceString = "Internal,OutSide")]
        public ApiReturn CallerSource821()
        {
            return new ApiReturn();
        }

        /// <summary>
        /// 822限制只可外部调用
        /// </summary>
        /// <returns></returns>
        [SsRoute("822/CallerSource")]
        [SsCallerSource(ECallerSource.OutSide, errorMessage = "只可外部调用")]
        public ApiReturn CallerSource822()
        {
            return new ApiReturn();
        }

        #endregion



        #region (x.8.3) SsCmp
        /// <summary>
        /// 830 SsCmp
        /// </summary>
        /// <returns></returns>
        [SsRoute("830/SsCmp")]
        [SsCmp(path = "http.method", type = "==" , value="PUT")]
        public ApiReturn SsCmp830()
        {
            return new ApiReturn();
        }
        #endregion


        #region (x.8.4) SsEqual
        /// <summary>
        /// 840 SsEqual
        /// </summary>
        /// <returns></returns>
        [SsRoute("840/SsEqual")]
        [SsEqual(path = "http.method", value = "PUT")]
        public ApiReturn SsEqual840()
        {
            return new ApiReturn();
        }
        #endregion


        #region (x.8.5) SsNotEqual
        /// <summary>
        /// 850 SsNotEqual
        /// </summary>
        /// <returns></returns>
        [SsRoute("850/SsNotEqual")]
        [SsNotEqual(path = "http.method", value = "PUT",errorMessage = "不可为PUT请求")]
        public ApiReturn SsNotEqual850()
        {
            return new ApiReturn();
        }
        #endregion


        #region (x.8.6) SsNotNull
        /// <summary>
        /// 860 SsNotNull
        /// </summary>
        /// <returns></returns>
        [SsRoute("860/SsNotNull")]
        [SsNotNull(path = "http.headers.Authorization",   errorMessage = "必须指定Authorization")]
        public ApiReturn SsNotNull860()
        {
            return new ApiReturn();
        }
        #endregion

        #region (x.8.7) SsRegex
        /// <summary>
        /// 870 SsRegex(请求地址 /demo/v1/api/870/SsRegex/a.html )
        /// </summary>
        /// <returns></returns>
        [SsRoute("870/SsRegex/*")]
        [SsRegex(path = "http.url", value = "(?<=\\.html)$", errorMessage = "url后缀必须为 .html")]
        public ApiReturn SsRegex870()
        {
            return new ApiReturn();
        }
        #endregion




        #endregion


        #region (x.9)特殊示例

       

        #region (x.x.1) 递归demo
        /// <summary>
        ///  递归。 f(n)  =  f(n-1) + f(n-2)
        /// </summary>
        /// <returns></returns>
        [SsRoute("901/Recursive")]
        public ApiReturn<int> Recursive([SsExample("4")]int n)
        {
            if (n <= 2) return 1;

            var task1 = ApiClient.CallRemoteApiAsync<ApiReturn<int>>(RpcContext.RpcData.route, new { n = n - 1 });
            var task2 = ApiClient.CallRemoteApiAsync<ApiReturn<int>>(RpcContext.RpcData.route, new { n = n - 2 });
            Task.WaitAll(task1, task1);
            var apiRet1 = task1.Result;
            var apiRet2 = task2.Result;
            if (!apiRet1.success) return apiRet1.error;
            if (!apiRet2.success) return apiRet2.error;
            return apiRet1.data+ apiRet2.data;
        }
        #endregion


        #region (x.x.2) Sleep
        /// <summary>
        ///  睡眠指定时间
        /// </summary>
        /// <returns></returns>
        [SsRoute("902/sleep")]
        public ApiReturn<int> Sleep(
            [SsExample("2"),SsDescription("单位:s")]int s)
        {
            Thread.Sleep(s * 1000);            
            return s;
        }
        #endregion




        #region (x.x.3) Upload
        /// <summary>
        ///  上传文件(测试地址 /demo/v1/api/700/static/upload.html)
        /// </summary>
        /// <returns></returns>
        [SsRoute("903/upload",HttpMethod = "POST")]
        public ApiReturn<object> Upload(byte[] data)
        {
            MultipartForm result=new MultipartForm(data, RpcContext.RpcData.http_header_ContentType_Get());
              
            return result;
        }
        #endregion



        #endregion
    }
}
