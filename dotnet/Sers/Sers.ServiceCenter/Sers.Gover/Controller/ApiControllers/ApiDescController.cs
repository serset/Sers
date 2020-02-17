using System.Collections.Generic;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.Rpc;
using Sers.ApiLoader.Sers;
using Sers.ApiLoader.Sers.Attribute;
using System.Linq;
using Sers.Gover.Base;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;

namespace Sers.Gover.Controller.ApiControllers
{
    [SsStationName("_gover_")]
    public class ApiDescController : IApiController
    { 

       
        [SsRoute("apiDesc/getActive")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取所有可调用api")]
        [SsDescription("获取所有可调用api。返回ApiDesc列表")]
        public ApiReturn<List<SsApiDesc>> GetActive([SsDescription("route前缀（不指定则返回所有），例如 \"_gover_\"、\"/demo/a.html\" "),SsExample("_gover_")]string r)
        {
            var apiDescs = GoverManage.Instance.ApiDesc_GetActive();
            if (!string.IsNullOrWhiteSpace(r))
            {
                if (!r.StartsWith("/")) r = "/" + r;
                apiDescs = apiDescs.Where(m=>m.route.StartsWith(r));
            }
            return new ApiReturn<List<SsApiDesc>> { data= apiDescs.ToList() };       
        }


        [SsRoute("apiDesc/getAll")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取所有api")]
        [SsDescription("获取所有api。返回ApiDesc列表")]
        public ApiReturn<List<SsApiDesc>> GetAll([SsDescription("route前缀（不指定则返回所有），例如 \"_gover_\"、\"/demo/a.html\" "), SsExample("_gover_")]string r)
        {
            var apiDescs = GoverManage.Instance.ApiDesc_GetAll();
            if (!string.IsNullOrWhiteSpace(r))
            {
                if (!r.StartsWith("/")) r = "/" + r;
                apiDescs = apiDescs.Where(m => m.route.StartsWith(r));
            }
            return new ApiReturn<List<SsApiDesc>> { data = apiDescs.ToList() }; 
        }

 
 




    }
}
