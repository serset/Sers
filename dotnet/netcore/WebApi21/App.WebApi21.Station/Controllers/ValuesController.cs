using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace App.Station.Controllers
{
    [Route("apidemo/[controller]")]
    [Route("apidemo/v1")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        #region (x.1)route

        /// <summary>
        /// GET apidemo/Values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object  Route0()
        {
            //var requestFeature = Request.HttpContext.Features.Get<IHttpRequestFeature>();             
            return "GET apidemo/Values";
        }


        /// <summary>
        /// GET apidemo/Values/route_x
        /// </summary>
        /// <returns></returns>
        [HttpGet("route1")]
        [HttpGet("route2")]
        [HttpGet("/apidemo/Values/route3")]
        public object Route1()
        {            
            return new{
                Request.Path,
                Method = Request.Method
            };
        }


        /// <summary>
        /// GET apidemo/Values/route4
        /// </summary>
        /// <returns></returns>     
        [HttpGet("[action]")]
        public object route4()
        {
            return new
            {
                name= "GET apidemo/Values/route4",
                Request.Path,
                Method = Request.Method
            };
        }
        #endregion


        #region (x.2) result

        /// <summary>
        /// GET apidemo/Values/result1
        /// </summary>
        /// <returns></returns>
        [HttpGet("result1")]
        public object Result1()
        {
            return "GET apidemo/Values/result1";
        }

        /// <summary>
        /// GET apidemo/Values/result2
        /// </summary>
        /// <returns></returns>
        [HttpGet("result2")]
        public ActionResult<string> Result2()
        {
            return "GET apidemo/Values/result2";
        }


        /// <summary>
        /// GET apidemo/Values/result3
        /// </summary>
        /// <returns></returns>
        [HttpGet("result3")]
        public ActionResult<IEnumerable<string>> Result3()
        {
            return new string[] { "GET apidemo/Values/result3", "" };
        }


        /// <summary>
        /// GET apidemo/Values/result4
        /// </summary>
        /// <returns></returns>
        [HttpGet("result4")]
        public async Task<string> Result4()
        {
            await Task.Run(() => { Thread.Sleep(2000); });

            return "GET apidemo/Values/result4";
        }

        /// <summary>
        /// GET apidemo/Values/result5
        /// </summary>
        /// <returns></returns>
        [HttpGet("result5")]
        public async Task<ActionResult<string>> Result5()
        {
            await Task.Run(() => { Thread.Sleep(2000); });

            return "GET apidemo/Values/result5";
        }
        #endregion


        #region (x.3) HttpMethod

        /// <summary>
        /// GET apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpGet("method")]
        public string Method_Get()
        {
            return "GET apidemo/Values/method";
        }

        /// <summary>
        /// POST apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpPost("method")]
        public string Method_Post()
        {
            return "POST apidemo/Values/method";
        }

        /// <summary>
        /// Put apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpPut("method")]
        public string Method_Put()
        {
            return "Put apidemo/Values/method";
        }

        /// <summary>
        /// Delete apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpDelete("method")]
        public string Method_Delete()
        {
            return "Delete apidemo/Values/method";
        }

        /// <summary>
        /// Head apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpHead("method")]
        public string Method_Head()
        {
            return "Head apidemo/Values/method";
        }

        /// <summary>
        /// Options apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpOptions("method")]
        public string Method_Options()
        {
            return "Options apidemo/Values/method";
        }

        /// <summary>
        /// Patch apidemo/Values/method
        /// </summary>
        /// <returns></returns>
        [HttpPatch("method")]
        public string Method_Patch()
        {
            return "Patch apidemo/Values/method";
        }


        /// <summary>
        /// get|post apidemo/Values/method2
        /// </summary>
        /// <returns></returns>
        [Route("method2")]
        [HttpGet,HttpPost]
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
        /// GET apidemo/Values/arg1/{id}/{id2}
        /// </summary>
        /// <returns></returns>
        [HttpGet("arg1/{id}/{id2}")]
        public object Arg1(string id,string id2)
        {
            return new {
                route= "GET apidemo/Values/arg1/{id}/{id2}",
                arg = new { id,id2 }
            }; 
        }

        ///// <summary>
        ///// GET apidemo/Values/arg2/arg2?id={id}
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("arg2?id={id}")]
        //public object Arg2(string id)
        //{
        //    return new
        //    {
        //        route = "GET apidemo/Values/arg2/arg2?id={id}",
        //        arg = new { id }
        //    };
        //}


        /// <summary>
        /// POST apidemo/Values/arg3
        /// </summary>
        /// <returns></returns>
        [HttpPost("arg3")]
        public object Arg3([FromBody] string arg1)
        {
            return new
            {
                route = "POST apidemo/Values/arg3",
                arg = new { arg1 }
            };
        }



        /// <summary>
        /// POST apidemo/Values/arg4
        /// </summary>
        /// <returns></returns>
        [HttpPost("arg4")]
        public object Arg4([FromBody] Arg4Model arg)
        {
            return new
            {
                route = "POST apidemo/Values/arg4",
                arg = arg
            };
        }
        public class Arg4Model {
            public string arg1 { get; set; }
            public string arg2 { get; set; }
        };
        #endregion


    }
}
