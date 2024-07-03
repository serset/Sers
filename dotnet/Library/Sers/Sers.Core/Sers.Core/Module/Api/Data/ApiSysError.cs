using System;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Object_Serialize_Extensions;

namespace Sers.Core.Module.Api.Data
{
    public class ApiSysError
    {

        public static void LogSysError(RpcContextData rpcContextData, ApiMessage reqMessage, SsError error)
        {
            try
            {
                string msg = "[ApiCallError]route:" + rpcContextData.route;
                msg += Environment.NewLine + "error:" + error.Serialize();
                msg += Environment.NewLine + "rpcData:" + rpcContextData.Serialize();
                try
                {
                    msg += Environment.NewLine + "RequestBody:" + reqMessage.value_OriData.ArraySegmentByteToString();
                }
                catch { }

                Logger.Error(msg);
            }
            catch { }
        }
    }
}
