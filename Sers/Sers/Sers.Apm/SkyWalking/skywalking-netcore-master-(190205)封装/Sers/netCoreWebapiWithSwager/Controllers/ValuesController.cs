
using Microsoft.AspNetCore.Mvc;
using Sers.Apm.SkyWalking.Core;
using Sers.Apm.SkyWalking.Core.Model;

namespace WebTest.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
         
        [HttpGet("Send")]
        public string Send()
        {
            SkyWalkingManage.Send(RequestModel.CreateDemo());
            return "sw70";
        }




         
    }
}
