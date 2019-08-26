using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Module.Rpc;
using System;
using System.Text;

namespace Sers.Core.Station.Module.Api.LocalApi.ApiTrace.Rpc
{
    public class ApiTraceLog:IDisposable
    {
        private DateTime beginTime;
        private DateTime endTime;
        public ApiTraceLog()
        {
            beginTime = DateTime.Now;
        }

        static void LogTrace(ApiTraceLog trace)
        {


            var rpcContext = RpcContext.Current;

            StringBuilder msg = new StringBuilder();

            msg.Append(Environment.NewLine).Append("┍------------ ---------┑");

            msg.Append(Environment.NewLine).Append("--BeginTime:").Append(trace.beginTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--EndTime  :").Append(trace.endTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--route    :").Append(rpcContext.rpcData.route);
            msg.Append(Environment.NewLine).Append("--duration :").Append((trace.endTime - trace.beginTime).TotalMilliseconds).Append(" ms");


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

            Logger.log.LogTxt(Level.ApiTrace, msg.ToString());

        }


        public void Dispose()
        {
            endTime = DateTime.Now;
            try
            {
                LogTrace(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
