using System;
using Sers.Core.Module.Message;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ComponentModel.SsError;

namespace Sers.Core.Module.Api.Data
{
    public class ApiSysError
    {

        public static void LogSysError(IRpcContextData rpcContextData, ApiMessage reqMessage, SsError error)
        {
            try
            {
                string msg = "[ApiCallError]route:" + rpcContextData.route;
                msg += Environment.NewLine+"error:" + error.Serialize();
                msg += Environment.NewLine + "rpcData:" + rpcContextData.oriJson;
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
