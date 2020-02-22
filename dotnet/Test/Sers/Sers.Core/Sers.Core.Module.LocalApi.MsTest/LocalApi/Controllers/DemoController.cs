using Sers.ApiLoader.Sers;
using Sers.ApiLoader.Sers.Attribute;
using System.Collections.Generic;
using System.Linq;
using Vit.Core.Util.ComponentModel.Api;

namespace Sers.Core.Module.Api.MsTest.LocalApi.Controllers
{
    [SsStationName("Test")]
    public class DemoController : IApiController
    {

        public class Arg
        {
            public string name { get; set; }
        }


       
        [SsRoute("api/GetDeviceGuidList")]
        public string GetDeviceGuidList(string arg)
        {
            return arg+ "Test";
        }


        public class GetListArg
        {
            public string[] arr;
        }

        [SsRoute("api/getList")]
        public List<string> GetList(GetListArg arg)
        {
            var ret = from i in arg.arr select i + "_1";
            return ret.ToList();
        }


    }
}
