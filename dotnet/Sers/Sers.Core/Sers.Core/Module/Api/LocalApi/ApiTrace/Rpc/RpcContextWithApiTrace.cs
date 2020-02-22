using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;

namespace Sers.ServiceStation.ApiTrace.Rpc
{
    class RpcContextWithApiTrace: Sers.Core.Module.Rpc.RpcContext
    {
        private DateTime beginTime;
        private DateTime endTime;
        public RpcContextWithApiTrace()
        {
            beginTime=DateTime.Now;
            
        }

        static void LogTrace(RpcContextWithApiTrace rpcContext)
        {
            StringBuilder msg=new StringBuilder();

            msg.Append(Environment.NewLine).Append("┍------------ ---------┑");

            msg.Append(Environment.NewLine).Append("--BeginTime:").Append(rpcContext.beginTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--EndTime  :").Append(rpcContext.endTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--route    :").Append(rpcContext.rpcData.route);
            msg.Append(Environment.NewLine).Append("--duration :").Append((rpcContext.endTime- rpcContext.beginTime).TotalMilliseconds).Append(" ms");


            msg.Append(Environment.NewLine).Append("--Req rpc  :").Append(rpcContext.rpcData.oriJson);

            try
            {
                string str = rpcContext.apiRequestMessage.value_OriData.ArraySegmentByteToString();
                msg.Append(Environment.NewLine).Append("--Req data :").Append(str);
            }
            catch
            {
            }

            try
            {
                string str = rpcContext.apiReplyMessage.value_OriData.ArraySegmentByteToString();
                msg.Append(Environment.NewLine).Append("--Rep data :").Append(str);
            }
            catch
            {
            }
            try
            {
                string str = rpcContext.apiReplyMessage.rpcContextData_OriData.ArraySegmentByteToString();
                msg.Append(Environment.NewLine).Append("--Rep rpc  :").Append(str);
            }
            catch
            {
            }

            msg.Append(Environment.NewLine).Append("┕------------ ---------┙").Append(Environment.NewLine);

            Logger.log.LogTxt(Level.ApiTrace,msg.ToString());

        }


        public override void Dispose()
        {
            endTime = DateTime.Now;
            try
            {
                LogTrace(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.GetBaseException());
            }

            base.Dispose();
        }
    }
}
