
using Microsoft.AspNetCore.Mvc;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using System.Collections.Generic;

namespace WebTest.Controllers
{
    [Route("AspNetCoreDemo/[controller]")]
    public class ValuesController : Controller
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">请求名称</param>
        /// <returns></returns>
        [HttpGet("/AspNetCoreDemo/fold1/a1")]
        [HttpGet("fold2/a2")]
        [HttpPost("a3")]
        public List<string> Test([SsExample("req001")]string request)
        {
            return new List<string> { request, "hello" };
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">请求名称</param>
        /// <returns></returns>
        [HttpGet]
        public List<string> Test2([SsExample("req001")]string request)
        {
            return new List<string> { request, "hello" };

        }


        


    }
}
