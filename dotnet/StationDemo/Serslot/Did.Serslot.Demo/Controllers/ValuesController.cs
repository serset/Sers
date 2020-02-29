using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

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
        public object Route0([FromQuery]string a)
        {
            //var requestFeature = Request.HttpContext.Features.Get<IHttpRequestFeature>();             
            return "GET did_serslot/Values?a=" + a;
        }


        /// <summary>
        /// GET did_serslot/Values/route_x
        /// </summary>
        /// <returns></returns>
        [HttpGet("route1")]
        [HttpGet("route2")]
        [HttpGet("/did_serslot/Values/route3")]
        public object Route1()
        {
            return new
            {
                Request.Path,
                Method = Request.Method
            };
        }


        /// <summary>
        /// GET did_serslot/Values/route4
        /// </summary>
        /// <returns></returns>     
        [HttpGet("[action]")]
        public object route4()
        {
            return new
            {
                name = "GET did_serslot/Values/route4",
                Request.Path,
                Method = Request.Method
            };
        }
        #endregion


        #region (x.2) result

        /// <summary>
        /// GET did_serslot/Values/result1
        /// </summary>
        /// <returns></returns>
        [HttpGet("result1")]
        public object Result1()
        {
            return "GET did_serslot/Values/result1";
        }

        /// <summary>
        /// GET did_serslot/Values/result2
        /// </summary>
        /// <returns></returns>
        [HttpGet("result2")]
        public ActionResult<string> Result2()
        {
            return "GET did_serslot/Values/result2";
        }


        /// <summary>
        /// GET did_serslot/Values/result3
        /// </summary>
        /// <returns></returns>
        [HttpGet("result3")]
        public ActionResult<IEnumerable<string>> Result3()
        {
            return new string[] { "GET did_serslot/Values/result3", "" };
        }


        /// <summary>
        /// GET did_serslot/Values/result4
        /// </summary>
        /// <returns></returns>
        [HttpGet("result4")]
        public async Task<string> Result4()
        {
            await Task.Run(() => { Thread.Sleep(2000); });

            return "GET did_serslot/Values/result4";
        }

        /// <summary>
        /// GET did_serslot/Values/result5
        /// </summary>
        /// <returns></returns>
        [HttpGet("result5")]
        public async Task<ActionResult<string>> Result5()
        {
            await Task.Run(() => { Thread.Sleep(2000); });

            return "GET did_serslot/Values/result5";
        }
        #endregion


        #region (x.3) HttpMethod

        /// <summary>
        /// GET did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpGet("method")]
        public string Method_Get()
        {
            return "GET did_serslot/Values/method";
        }

        /// <summary>
        /// POST did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpPost("method")]
        public string Method_Post()
        {
            return "POST did_serslot/Values/method";
        }

        /// <summary>
        /// Put did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpPut("method")]
        public string Method_Put()
        {
            return "Put did_serslot/Values/method";
        }

        /// <summary>
        /// Delete did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpDelete("method")]
        public string Method_Delete()
        {
            return "Delete did_serslot/Values/method";
        }

        /// <summary>
        /// Head did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpHead("method")]
        public string Method_Head()
        {
            return "Head did_serslot/Values/method";
        }

        /// <summary>
        /// Options did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpOptions("method")]
        public string Method_Options()
        {
            return "Options did_serslot/Values/method";
        }

        /// <summary>
        /// Patch did_serslot/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpPatch("method")]
        public string Method_Patch()
        {
            return "Patch did_serslot/Values/method";
        }


        /// <summary>
        /// get|post did_serslot/Values/method2
        /// </summary>
        /// <returns></returns>
        [Route("method2")]
        [HttpGet, HttpPost]
        public object Method2()
        {
            return new
            {
                Request.Path,
                Method = Request.Method
            };
        }
        #endregion


        #region (x.4) Arg


        /// <summary>
        /// GET did_serslot/Values/arg1/{id}/{id2}
        /// </summary>
        /// <returns></returns>
        [HttpGet("arg1/{id}/{id2}")]
        public object Arg1(string id, string id2)
        {
            return new
            {
                route = "GET did_serslot/Values/arg1/{id}/{id2}",
                arg = new { id, id2 }
            };
        }

        ///// <summary>
        ///// GET did_serslot/Values/arg2/arg2?id={id}
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("arg2?id={id}")]
        //public object Arg2(string id)
        //{
        //    return new
        //    {
        //        route = "GET did_serslot/Values/arg2/arg2?id={id}",
        //        arg = new { id }
        //    };
        //}


        /// <summary>
        /// POST did_serslot/Values/arg3
        /// </summary>
        /// <returns></returns>
        [HttpPost("arg3")]
        public object Arg3([FromBody] string arg1)
        {
            return new
            {
                route = "POST did_serslot/Values/arg3",
                arg = new { arg1 }
            };
        }



        /// <summary>
        /// POST did_serslot/Values/arg4
        /// </summary>
        /// <returns></returns>
        [HttpPost("arg4")]
        public object Arg4([FromBody] Arg4Model arg)
        {
            return new
            {
                route = "POST did_serslot/Values/arg4",
                arg = arg
            };
        }
        public class Arg4Model
        {
            public string arg1 { get; set; }
            public string arg2 { get; set; }
        };
        #endregion


    }
}
