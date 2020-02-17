using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Vit.Extensions;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.App;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ConfigurationManager;
using Sers.ApiLoader.Sers.Attribute;
using Sers.ApiLoader.Sers;
using System.Threading;
using App.Demo.Station.Controllers.PubSub;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;
using Vit.Net.Http.FormFile;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit;

namespace App.Demo.Station.Controllers.Demo
{
    //站点名称，可多个。若不指定，则从 配置文件中LocalApiService中的"apiStationNames"获取
    [SsStationName("demo")]
    //[SsStationName("demo2"), SsStationName("demo3")]
    //路由前缀，可不指定
    [SsRoutePrefix("v1/api")]
    public class SampleController
        : IApiController
    {

        #region (x.0) static init
        static SampleController()
        {
            SersApplication.onStart += SubscriberDemo.Subscribe;
        }
        #endregion





        #region (x.1)route


        /// <summary>
        /// route示例
        /// </summary>
        /// <returns></returns>
        [SsRoute("1/route/1"), SsRoute("1/route/2")]
        [SsRoute("/demo/v1/api/1/route/3")] //使用绝对路径路由  
        public ApiReturn<object> Route()
        {
            return new { url = RpcContext.RpcData.http_url_Get() };
        }



        /// <summary>
        /// route示例-泛接口
        /// </summary>
        /// <returns></returns>

        [SsRoute("1/route/4/*")] //使用泛接口路径路由
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
        [SsRoute("1/route/5",HttpMethod ="GET")]
        public ApiReturn<object> Route5_Get()
        {
            return new { apiName = "Route5_Get", rpcData = RpcContext.RpcData };
        }

        /// <summary>
        /// HttpMethod示例POST
        /// </summary>
        /// <returns></returns>
        [SsRoute("1/route/5", HttpMethod = "POST")]
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
        [SsRoute("2/NameDesc/1")]
        [SsName("NameDesc1")]
        public ApiReturn NameDesc()
        {
            return new ApiReturn();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("2/NameDesc/2")]
        [SsDescription("演示 如何使用Name 和 Description。\n函数注释和使用SsDescription是一样的效果。")]
        public ApiReturn NameDesc2()
        {
            return new ApiReturn();
        }
        #endregion

        #region (x.3)参数

        #region (x.x.1)无参

        /// <summary>
        /// arg1
        /// </summary>
        /// <returns></returns>
        [SsRoute("3/arg/10")]
        public ApiReturn Arg10()
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
        [SsRoute("3/arg/21")]
        public ApiReturn<Object> Arg21(
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
        [SsRoute("3/arg/22")]
        public ApiReturn<Object> Arg22(
             [SsExample("[\"arg1\",\"arg2\"]"), SsDefaultValue("[\"arg1\",\"arg2\"]"), SsDescription("argDescription")]string[] args)
        {
            return args;
        }

        /// <summary>
        /// 标识函数第一个参数为Api的参数实体。（忽略其他参数(若存在)，调用函数时其他参数(若存在)为空）
        /// </summary>
        /// <param name="args">args注释</param>
        /// <returns></returns>
        [SsRoute("3/arg/23")]
        public ApiReturn<Object> Arg23(
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
        [SsRoute("3/arg/24")]
        public ApiReturn<Object> Arg24([SsArgEntity]string  args,string arg2)
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
        [SsRoute("3/arg/31")]
        public ApiReturn<Object> Arg31([SsExample("argExample"), SsDefaultValue("argDefaultValue"), SsDescription("argDescription")]string arg1)
        {
            return new { arg1 };
        }


        /// <summary>
        /// 函数参数列表为参数实体
        /// </summary>
        /// <param name="arg1">arg1注释</param>
        /// <param name="arg2">arg2注释</param>
        /// <returns></returns>
        [SsRoute("3/arg/32")]
        public ApiReturn<Object> Arg32(
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
        [SsRoute("3/arg/33")]
        public ApiReturn<Object> Arg33(ArgModel arg1,
            [SsExample("6"), SsDefaultValue("1")]int arg2)
        {
            return new { arg1, arg2 };
        }


        /// <summary>
        /// 函数第一个参数为Api的参数实体的属性之一。（一般放置在第一个参数上，代表第一个参数不为参数实体）
        /// </summary>
        /// <param name="arg1">arg1注释</param>     
        /// <returns></returns>
        [SsRoute("3/arg/34")]
        public ApiReturn<Object> Arg34([SsArgProperty]ArgModel arg1)
        {
            return new { arg1};
        }
        #endregion


        public class ArgModel
        {
            /// <summary>
            /// arg1Desc-xml
            /// </summary>
            [SsExample("example1"), SsDefaultValue("default1")]
            [JsonProperty("arg")]
            public string arg1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [SsExample("20"), SsDefaultValue("2"), SsDescription("desc2")]
            public int arg2;
        }

        #endregion


        #region (x.4)返回值
        /// <summary>
        /// Return1
        /// </summary>
        /// <returns>Return注释-FromXml</returns>
        [SsRoute("4/ret/1")]
        public ApiReturn<ReturnData> Return1()
        {
            return new ReturnData { arg1 ="arg1"};
        }

        /// <summary>
        /// Return2
        /// </summary>
        /// <returns></returns>
        [SsRoute("4/ret/2")]
        [return: SsExample("{\"data\":12}"), SsDescription("成功个数")]
        public ApiReturn<int> Return2()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("4/ret/3")]
        public ApiReturn<List<string>> Return3()
        {
            return new List<string>{ "1","2"};
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("4/ret/4")]
        [return: SsExample("test"), SsDescription("测试")]
        public string Return4()
        {
            return "test1";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("4/ret/5")]
        [return: SsExample("12"), SsDescription("测试")]
        public int Return5()
        {
            return 5;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("4/ret/6")]
        [return: SsExample("{\"arg1\":\"arg1\"}"), SsDescription("测试")]
        public ReturnData Return6()
        {
            return new ReturnData { arg1 = "arg1" };
        }


        /// <summary>
        /// Return注释-FromType
        /// </summary>
        public class ReturnData
        {

            /// <summary>
            /// arg1Desc-xml
            /// </summary>
            [SsExample("example1"), SsDefaultValue("default1")]
            public string arg1 { get; set; }
        }

        #endregion


        #region (x.5) Rpc

        /// <summary>
        /// rpc示例
        /// </summary>
        /// <returns></returns>

        [SsRoute("5/rpc/GetRpcOriData")]
        public ApiReturn<Object> GetRpcOriData()
        {
            return new ApiReturn<Object>(RpcContext.RpcData);
        }


        /// <summary>
        /// rpc示例
        /// </summary>
        /// <returns></returns>
        [SsRoute("5/rpc/RpcDemo")]
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

        [SsRoute("5/rpc/RpcReplyDemo")]
        public ApiReturn<Object> RpcReplyDemo([SsArgEntity,SsExample("requestContent")]string request)
        {
            #region reply header

            RpcContext.Current.apiReplyMessage.rpcContextData_OriData =
                    RpcFactory.Instance.CreateRpcContextData()
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

        /// <summary>
        /// ApiClient示例,调用其他接口
        /// </summary>
        /// <returns></returns>

        [SsRoute("6/ApiClient/1")]
        public ApiReturn<Object> ApiClient1(
            [SsExample("/demo/v1/3/arg/32")]string apiRoute, 
            [SsExample("{\"arg1\":\"dd\",\"arg2\":\"33\"}")]object arg)
        {
            var apiRet = ApiClient.CallRemoteApi<ApiReturn<Object>>(apiRoute, arg);
            return new ApiReturn<Object>(new
            {
                RpcContext.RpcData,
                apiRet
            });
        }


        #region (x.x.2) ApiClient示例,传输二进制数据

        /// <summary>
        /// ApiClient示例,传输二进制数据
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        [SsRoute("6/ApiClient/2_1")]
        [SsName("ApiClient demo")]
        [return:SsExample("1122AAFF"), SsDescription("二进制数据")]
        public byte[] ApiClient2_1( 
            [SsExample("1122AAFF"), SsDescription("二进制数据")]byte[] ba)    // 使用ArraySegment<byte> 会减少一步复制的操作，若传输大数据建议使用ArraySegment<byte>
        {
            return ba;
        }

        /// <summary>
        /// ApiClient示例,传输二进制数据
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        [SsRoute("6/ApiClient/2_2")]
        [SsName("ApiClient demo")]
        [return: SsExample("1122AAFF"), SsDescription("二进制数据")]
        public ArraySegment<byte> ApiClient2_2(
            [SsExample("1122AAFF"), SsDescription("二进制数据")]ArraySegment<byte> ba)
        {
            return ba;
        }

        /// <summary>
        /// ApiClient示例,传输二进制数据
        /// </summary>
        /// <returns></returns>
        [SsRoute("6/ApiClient/2_3")]
        public ApiReturn ApiClient2_3()
        {
            var apiRet = ApiClient.CallRemoteApi<ArraySegment<byte>>("/demo/v1/api/6/ApiClient/2_1", new byte[] { 3, 2, 3, 4, 5 });
            return new ApiReturn();
        }
        #endregion

        #region (x.x.3) ApiClient示例,传输额外数据

        /// <summary>
        /// ApiClient示例,二进制数据和ReplyRpc
        /// </summary>
        /// <returns></returns>
        [SsRoute("6/ApiClient/3_1")]
        public ApiReturn ApiClient3_1()
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
        [SsRoute("6/ApiClient/3_2")]
        public ApiReturn ApiClient3_2()
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage("/demo/v1/6/ApiClient/3_1", null);

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
        static StaticFileMap staticFileMap = new StaticFileMap(ConfigurationManager.Instance.GetByPath<string>("Demo.wwwroot"));

        /// <summary>
        /// UseStaticFiles
        /// </summary>
        /// <returns></returns>
        [SsRoute("7/static/*")]
        public byte[] UseStaticFiles()
        {
            return staticFileMap.TransmitFile();
        }
        #endregion



        #region (x.8) Rpc Valid



        /// <summary>
        /// 自定义限制 SsValidation
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid/1")]
        [SsRpcVerify(errorMessage = "只接受PUT请求", condition = "{\"type\":\"!=\",\"path\":\"http.method\",\"value\":\"PUT\"}")] 
        public ApiReturn Valid()
        {
            return new ApiReturn();
        }


        /// <summary>
        /// SsIsNull
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid/2")]
        [SsIsNull(path = "http.method", errorMessage = "必须指定method")]
        [SsIsNull(path = "caller.source", errorMessage = "必须指定调用来源")]
        [SsIsNull(path = "http.headers.Authorization", errorMessage = "必须指定Authorization")]
        public ApiReturn Valid2()
        {
            return new ApiReturn();
        }



        /// <summary>
        /// SsEqual
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid/3")]
        [SsEqual(path = "http.method", value = "PUT", errorMessage = "只接受PUT请求")]
        public ApiReturn Valid3()
        {
            return new ApiReturn();
        }


        /// <summary>
        /// SsRegex
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid/4/*")]
        [SsRegex(path = "http.url", value = "(?<!\\.html)$", errorMessage = "url后缀必须为 .html")]
        public ApiReturn Valid4()
        {
            return new ApiReturn();
        }


        #endregion


        #region (x.9) Rpc Valid2

        /// <summary>
        /// 限制只可内部调用
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid2/1")]
        [SsNotEqual(path = "caller.source", value = "Internal", errorMessage = "无权限")]
        //[SsNotEqual(type="!=",path = "caller.source", value = "Internal", errorMessage = "无权限")]
        public ApiReturn Valid2_1()
        {
            return new ApiReturn();
        }


        /// <summary>
        /// 限制只可内部调用
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid2/2")]
        //[SsCallerSource(ECallerSource.Internal)]
        [SsCallerSource(callerSourceString = "Internal,GoverGateway")]
        public ApiReturn Valid2_2()
        {
            return new ApiReturn();
        }

        /// <summary>
        /// 限制只可外部调用
        /// </summary>
        /// <returns></returns>
        [SsRoute("8/valid2/3")]
        [SsCallerSource(ECallerSource.OutSide, errorMessage = "只可外部调用")]
        public ApiReturn Valid2_3()
        {
            return new ApiReturn();
        }

        #endregion


        #region (x.10) 递归demo
        /// <summary>
        ///  递归demo。f(n)  =  f(n-1) + 1
        /// </summary>
        /// <returns></returns>
        [SsRoute("10/fn")]
        public ApiReturn<int> Fn([SsExample("2")]int n)
        {
            if (n <= 1) return 1;
         
            var apiRet = ApiClient.CallRemoteApi<ApiReturn<int>>(RpcContext.RpcData.route, new { n= n-1});
            if (!apiRet.success) return apiRet.error;
            return apiRet.data+1;
        }
        #endregion


        #region (x.11) Sleep
        /// <summary>
        ///  睡眠指定时间
        /// </summary>
        /// <returns></returns>
        [SsRoute("11/sleep")]
        public ApiReturn<int> Sleep(
            [SsExample("2"),SsDescription("单位:s")]int s)
        {
            Thread.Sleep(s * 1000);            
            return s;
        }
        #endregion




        #region (x.12) Upload
        /// <summary>
        ///  上传文件(测试地址 /demo/v1/api/7/static/upload.html)
        /// </summary>
        /// <returns></returns>
        [SsRoute("12/upload",HttpMethod = "POST")]
        public ApiReturn<object> Upload(byte[] data)
        {
            MultipartForm result=new MultipartForm(data, RpcContext.RpcData.http_header_ContentType_Get());
              
            return result;
        }
        #endregion


    }
}
