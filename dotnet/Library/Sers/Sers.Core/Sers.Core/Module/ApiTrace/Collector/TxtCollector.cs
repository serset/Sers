using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;
using System.Collections.Generic;
using System.Text;
using Vit.Extensions;
using Vit.Core.Module.Log;

namespace Sers.Core.Module.ApiTrace.Collector
{
    public class TxtCollector : IApiTraceCollector
    {

        IDictionary<string, string> tagsTemplate;
        public void Init(JObject arg)
        {
            Logger.Info("[ApiTrace.TxtCollector]初始化中");

            tagsTemplate = arg?["tags"]?.Deserialize<IDictionary<string, string>>();
        }

        public void AppBeforeStart()
        {
            Logger.Info("[ApiTrace.TxtCollector]初始化成功");
        }

        public void AppBeforeStop()
        {

        }

        Vit.Core.Module.Log.LogCollector.TxtCollector logCollector = new Vit.Core.Module.Log.LogCollector.TxtCollector();

        public object TraceStart(RpcContextData rpcData)
        {
            return DateTime.Now;
        }
        public void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage)
        {
            var beginTime = (DateTime)traceData;

            var endTime = DateTime.Now;


            StringBuilder msg = new StringBuilder(1024 * 4);

            msg.Append(Environment.NewLine).Append("┍------------ ---------┑");

            msg.Append(Environment.NewLine).Append("--beginTime:").Append(beginTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--endTime  :").Append(endTime.ToString("[HH:mm:ss.ffffff]"));
            msg.Append(Environment.NewLine).Append("--duration :").Append((endTime - beginTime).TotalMilliseconds).Append(" ms");


            JObject eventData = SplunkCollector.BuildEventData(null, rpcData, apiRequestMessage, GetApiReplyMessage, tagsTemplate);
            foreach (var kv in eventData)
            {
                var key = kv.Key;
                var value = kv.Value;
                if (key != null)
                {
                    msg.Append(Environment.NewLine).Append("--" + key + ":").Append(value);
                }
            }

            msg.Append(Environment.NewLine).Append("┕------------ ---------┙").Append(Environment.NewLine);

            logCollector.Write(Level.ApiTrace, msg.ToString());
        }

    }
}
