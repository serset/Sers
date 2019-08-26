using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace App.StationDemo.Station.Controllers.Demo
{
 
    //路由前缀，可不指定
    [SsRoutePrefix("demo")]
    public class DemoController : IApiController
    {


        /// <summary>
        /// Demo1-注释
        /// </summary>
        /// <param name="arg1">arg1注释</param>
        /// <returns>ArgModelDesc-returns</returns>
        [SsRoute("demo/1")]
        //[SsCallerSource(ECallerSource.Internal)]
        public ApiReturn<string> Demo1(ArgModel arg1)
        {
            return new ApiReturn<string>() { data= arg1?.Name };
        }

        /// <summary>
        /// Demo2-注释
        /// </summary>
        /// <param name="arg1">arg1注释</param>
        /// <param name="arg2">arg2注释</param>
        /// <returns>是否成功</returns>
        [SsRoute("demo/2")]
        public ApiReturn Demo2(
            [SsExample("example1"), SsDefaultValue("default1")]string arg1,
            [SsExample("6"), SsDefaultValue("1")]int arg2)
        {
            return new ApiReturn();
        }



        public class ArgModel
        {
            /// <summary>
            /// 用户名
            /// </summary>
            [SsExample("张三"), SsDefaultValue("未指定")]
            [JsonProperty("name")]
            public string Name { get; set; }


            /// <summary>
            /// 年龄
            /// </summary>
            [SsExample("12"), SsDefaultValue("")]
            [JsonProperty("age")]
            public int Age { get; set; }

        }
        
    }
}
