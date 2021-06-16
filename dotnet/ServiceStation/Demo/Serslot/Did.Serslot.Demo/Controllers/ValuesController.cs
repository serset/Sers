using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;

namespace Did.Serslot.Demo.Controllers
{
    [Route("did_serslot/[controller]")]
    [Route("did_serslot/v1")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        #region (x.1)route

        /// <summary>
        /// GET did_serslot/Values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public object Route0([FromQuery]string a)
        {
            //var requestFeature = Request.HttpContext.Features.Get<IHttpRequestFeature>();             
            return "GET did_serslot/Values?a=" + a;
        }


        /// <summary>
        /// GET did_serslot/Values/route_x
        /// </summary>
        /// <returns></returns>
        [HttpGet("101/route")]
        [HttpGet("102/route")]
        [HttpGet("/did_serslot/Values/103/route")]
        public object Route1()
        {
            return new
            {
                Request.Path,
                Request.Method
            };
        }


        /// <summary>
        /// GET did_serslot/Values/route104
        /// </summary>
        /// <returns></returns>     
        [HttpGet("[action]")]
        public object route104()
        {
            return new
            {
                name = "GET did_serslot/Values/route104",
                Request.Path,
                Request.Method
            };
        }
        #endregion


        #region (x.2)Name和Desc

        /// <summary>
        /// 演示 如何使用Name 和 Description。
        /// 函数注释和使用SsDescription是一样的效果。
        /// </summary>
        /// <returns></returns>
        [HttpGet("201/NameDesc")]
        [SsName("NameDesc1")]
        public ApiReturn NameDesc()
        {
            return new ApiReturn();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("202/NameDesc")]
        [SsDescription("演示 如何使用Name 和 Description。\n函数注释和使用SsDescription是一样的效果。")]
        public ApiReturn NameDesc2()
        {
            return new ApiReturn();
        }
        #endregion



        #region (x.3) 参数

        #region (x.x.1) 从route获取参数
        /// <summary>
        /// 从route获取参数
        /// GET did_serslot/Values/301/arg/{id}/{id2}       
        /// </summary>
        /// <returns></returns>
        [HttpGet("301/arg/{name}/{age}")]
        public object Arg301(
             [SsExample("lith"), SsDefaultValue("NoName"), SsDescription("姓名")]string name,
             [SsExample("30"), SsDefaultValue("0"), SsDescription("年龄，请指定在16-50中间的整数")]int age)
        {
            return new
            {
                Request.Path,
                Request.Method,
                request = new { name, age }
            };
        }
        #endregion

        #region (x.x.2)从Query String获取参数
        /// <summary>
        /// 从Query String获取参数
        /// GET did_serslot/Values/302/arg?name=lith&amp;age=30
        /// </summary>
        /// <returns></returns>
        [HttpGet("302/arg")]
        public object Arg302(
             [FromQuery, SsExample("lith"),SsDescription("姓名")]string name,
             [FromQuery, SsExample("30"), SsDescription("年龄，请指定在16-50中间的整数")]int age)
        {
            return new
            {
                Request.Path,
                Request.Method,
                request = new { name, age }
            };
        }
        #endregion


        #region (x.x.3)从Body获取参数

        /// <summary>
        /// 从Body获取参数
        /// POST did_serslot/Values/303/arg
        /// </summary>
        /// <returns></returns>
        [HttpPost("303/arg")]
        public object Arg3([FromBody, SsExample("\"lith\""), SsDescription("姓名,请以双引号开始和结束")]string name)
        {
            return new
            {
                Request.Path,
                Request.Method,
                request = new { name }
            };
        }
        #endregion

        #region (x.x.4)从Body获取参数

        /// <summary>
        /// 从Body获取参数
        /// POST did_serslot/Values/304/arg
        /// </summary>
        /// <returns></returns>
        [HttpPost("304/arg")]
        public object Arg4([FromBody] ArgModel arg)
        {
            return new
            {
                Request.Path,
                Request.Method,
                request = arg
            };
        }

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


        #region (x.x.5)从Form获取参数

        /// <summary>
        /// 从Form获取参数
        /// POST did_serslot/Values/304/arg
        /// </summary>
        /// <returns></returns>
        [HttpPost("305/arg")]
        public object Arg5([FromForm]ArgModel arg)
        {
            return new
            {
                Request.Path,
                Request.Method,
                request = arg
            };
        }
        #endregion



        #endregion


        #region (x.4) 返回值


        #region (x.x.1)返回值注释
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("401/ret")]
        [return: SsExample("test1"), SsDescription("返回test1")]
        public string Return4()
        {
            return "test1";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("402/ret")]
        [return: SsExample("5"), SsDescription("返回5")]
        public int Return5()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("403/ret")]
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


        #region (x.x.2)异步返回

       

        /// <summary>
        /// GET did_serslot/Values/201/result
        /// </summary>
        /// <returns></returns>
        [HttpGet("420/result")]
        public object Result1()
        {
            return Request.Method + " " + Request.Path;
        }

        /// <summary>
        /// GET did_serslot/Values/202/result
        /// </summary>
        /// <returns></returns>
        [HttpGet("421/result")]
        public ActionResult<string> Result2()
        {
            return Request.Method + " " + Request.Path;
        }


        /// <summary>
        /// GET did_serslot/Values/203/result
        /// </summary>
        /// <returns></returns>
        [HttpGet("422/result")]
        public ActionResult<IEnumerable<string>> Result3()
        {
            return new string[] { Request.Method + " " + Request.Path, "" };
        }


        /// <summary>
        /// GET did_serslot/Values/204/result
        /// </summary>
        /// <returns></returns>
        [HttpGet("423/result")]
        public async Task<string> Result4()
        {
            await Task.Run(() => { Thread.Sleep(2000); });

            return Request.Method + " " + Request.Path;
        }

        /// <summary>
        /// GET did_serslot/Values/205/result
        /// </summary>
        /// <returns></returns>
        [HttpGet("424/result")]
        public async Task<ActionResult<string>> Result5()
        {
            await Task.Run(() => { Thread.Sleep(2000); });

            return Request.Method + " " + Request.Path;
        }

        #endregion


        #endregion


        #region (x.5)指定参数或返回值类型
        /// <summary>
        /// 指定参数或返回值类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("500/type")]
        [return: SsType(typeof(List<string>)),SsExample("[5,6]"), SsDescription("返回原值")]
        public string Type(
            [SsType(typeof(List<string>)), SsExample("[5,6]"), SsDescription("id数组")]string ids
            )
        {
            return ids;
        }
        #endregion


        #region (x.6) HttpMethod

        /// <summary>
        /// GET did_serslot/Values/600/method
        /// </summary>
        /// <returns></returns>
        [HttpGet("600/method")]
        public string Method_Get()
        {
            return Request.Method + " " + Request.Path;
        }

        /// <summary>
        /// POST did_serslot/Values/600/method
        /// </summary>
        /// <returns></returns>
        [HttpPost("600/method")]
        public string Method_Post()
        {
            return Request.Method + " " + Request.Path;
        }

        /// <summary>
        /// Put did_serslot/Values/600/method
        /// </summary>
        /// <returns></returns>
        [HttpPut("600/method")]
        public string Method_Put()
        {
            return Request.Method + " " + Request.Path;
        }

        /// <summary>
        /// Delete did_serslot/Values/600/method
        /// </summary>
        /// <returns></returns>
        [HttpDelete("600/method")]
        public string Method_Delete()
        {
            return Request.Method + " " + Request.Path;
        }

        ///// <summary>
        ///// Head did_serslot/Values/600/method
        ///// </summary>
        ///// <returns></returns>
        //[HttpHead("600/method")]
        //public string Method_Head()
        //{
        //    return Request.Method + " " + Request.Path;
        //}

        /// <summary>
        /// Options did_serslot/Values/600/method
        /// </summary>
        /// <returns></returns>
        [HttpOptions("600/method")]
        public string Method_Options()
        {
            return Request.Method + " " + Request.Path;
        }

        /// <summary>
        /// Patch did_serslot/Values/600/method
        /// </summary>
        /// <returns></returns>
        [HttpPatch("600/method")]
        public string Method_Patch()
        {
            return Request.Method + " " + Request.Path;
        }


        /// <summary>
        /// get|post did_serslot/Values/601/method
        /// </summary>
        /// <returns></returns>
        [Route("601/method")]
        [HttpGet, HttpPost]
        public string Method2()
        {
            return Request.Method + " " + Request.Path;
        }
        #endregion


        #region (x.7)ApiClient

        #region (x.x.1)ApiClient示例,调用其他接口
        /// <summary>
        /// ApiClient示例,调用其他接口
        /// </summary>
        /// <returns></returns>

        [HttpPost("700/ApiClient700")]
        public ApiReturn<Object> ApiClient700(
            [SsExample("/did_serslot/Values")] string apiRoute,
            [SsExample("{\"a\":\"aaa111\"}")] dynamic arg)
        {
            var apiRet = ApiClient.CallRemoteApi<string>((string)arg.apiRoute,(object) arg.arg, "POST");
            return new ApiReturn<Object>(new
            {
                RpcContext.RpcData,
                apiRet
            });
        }
        #endregion


        #region (x.x.2)ApiClient示例,调用其他接口
        /// <summary>
        /// ApiClient示例,调用其他接口
        /// </summary>
        /// <returns></returns>

        [HttpPost("702/ApiClient")]
        public ApiReturn<Object> ApiClient702()
        {
            return new ApiReturn<Object>(new
            {
                RpcContext.RpcData
            });
        }


        [HttpPost("703/ApiClient")]
        public string ApiClient703()
        {
            var rpc=RpcContext.RpcData;
            var apiRet = ApiClient.CallRemoteApi<string>("/did_serslot/v1/702/ApiClient", null,"POST");
            return apiRet;
        }
        #endregion

        #endregion
    }
}
