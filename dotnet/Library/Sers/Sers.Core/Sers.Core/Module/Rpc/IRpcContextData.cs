using Newtonsoft.Json.Linq;
using System;

namespace Sers.Core.Module.Rpc
{
    public interface IRpcContextData
    {
        /*
    {
        "route": "/DemoStation/v1/api/5/rpc/2",
        "caller": {
            "rid": "8320becee0d945e9ab93de6fdac7627a",
            "source": "Outside"
        },
        "http": {
            "url": "http://127.0.0.1:6000/DemoStation/v1/api/5/rpc/2",
            "headers": {
                "Cache-Control": "no-cache",
                "Connection": "keep-alive",
                "Content-Type": "application/javascript",
                "Accept": "* / *",
                "Accept-Encoding": "gzip, deflate",
                "Authorization": "bearer",
                "Host": "127.0.0.1:6000",
                "User-Agent": "PostmanRuntime/7.6.0",
                "Postman-Token": "78c5a1cb-764f-4e04-b2ae-514924a40d5a"
            }
        },
        "user": {}
    }

             */



        IRpcContextData UnpackOriData(ArraySegment<byte> oriData);
        ArraySegment<byte> PackageOriData();

        string route { get; set; }


        JObject oriJson { get; }

    }
}
