using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Extensions;
using Radical;
using System;

namespace App.Common.Station.Controllers.Radical
{
    public class RadicalController : IApiController
    {

        [SsDescription("获取value开方后的结果")]
        [SsRoute("math/radical/calc")]
        [SsCallerSource(ECallerSource.Internal)]
        public string Calc(
            [SsExample("180719.0011"),SsDescription("要开方的数据")]string value,
            [SsExample("16"),SsDescription("开方的位数")]int?len)
        {
            //(x.1) 设置返回为 txt 格式
            RpcContext.Current.apiReplyMessage.rpcContextData_OriData =
                    RpcFactory.Instance.CreateRpcContextData()
                    .http_header_Set("Content-Type", "text/txt; charset=utf-8")
                    .PackageOriData();



            //(x.2) 计算

            if (string.IsNullOrEmpty(value)) value = DateTime.Now.ToString("ddMMyy");

             RadicalHelp radical = new RadicalHelp();

            radical.calcLength = len ?? 16;
            radical.Calc(value);

            var strResult = radical.getResult();
            var strProcess = radical.getProcess();

            var response = strResult + Environment.NewLine + Environment.NewLine + strProcess;


            return response;
        }
         
        
    }
}
