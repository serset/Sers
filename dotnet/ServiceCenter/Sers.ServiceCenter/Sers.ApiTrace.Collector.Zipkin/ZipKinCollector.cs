using Newtonsoft.Json.Linq;

using Sers.Core.Module.ApiTrace.Collector;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;
using System.Collections.Generic;

using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace Sers.ApiTrace.Collector.Zipkin
{
    public class ZipKinCollector : IApiTraceCollector,IDisposable
    {
        ~ZipKinCollector() 
        {
            Dispose();
        }

        public void Init(JObject arg)
        {
            config = arg.Deserialize<Config>();
            if (string.IsNullOrEmpty(config.rpcName)) config.rpcName = "ServiceCenter";
            Logger.Info("[ApiTrace.ZipKinCollector] init ...", config);
 
            if (config == null) return;
            #region (x.1)注册和启动 Zipkin
            if (config.SamplingRate <= 0) config.SamplingRate = 1;

            TraceManager.SamplingRate = config.SamplingRate;

            // 在链路追踪控制台获取 Zipkin Endpoint,注意 Endpoint 中不包含“/api/v2/spans”。
            HttpZipkinSender httpSender = new HttpZipkinSender(config.zipkinCollectorUrl, "application/json");

            var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());
            TraceManager.RegisterTracer(tracer);
            TraceManager.Start(new MyLogger());
            #endregion

            Logger.Info("[ApiTrace.ZipKinCollector] init success.");
        }

        public void Dispose()
        {
            try
            {
                TraceManager.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error("[ApiTrace.ZipKinCollector] Dispose", ex);
            }
        }



        public object TraceStart(RpcContextData rpcData)
        {
            Trace trace;

            #region (x.1)get id(traceId spanId parentSpanId) and  create trace 
            try
            {
                long spanId = 0;
                long traceId = 0;
                long? parentSpanId = null;

                {
                    var hexStr = rpcData.caller.rid;
                    spanId = hexStr.Substring(0, 16).HexStringToInt64();
                }
                {
                    var hexStr = rpcData.caller_rootRid_Get();
                    if (hexStr == null)
                    {
                        traceId = spanId;
                    }
                    else
                    {
                        traceId = hexStr.Substring(0, 16).HexStringToInt64();
                    }
                }
                {
                    var hexStr = rpcData.caller_parentRid_Get();
                    parentSpanId = hexStr?.Substring(0, 16).HexStringToInt64();
                }

                var spanState = new zipkin4net.SpanState(traceId, parentSpanId, spanId, true, false);
                trace = Trace.CreateFromId(spanState);
            }
            catch (Exception ex)
            {
                trace = Trace.Create();
                Logger.Error(ex);
            }
            Trace.Current = trace;
            #endregion


            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.Rpc(config.rpcName));
            trace.Record(Annotations.ServiceName(rpcData.apiStationName_Get() ?? ""));

            return trace;
        }
        public void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage)
        {
            Trace trace = (Trace)traceData;


            #region rpc data
            //trace.Record(Annotations.Tag("http.url", rpcData.http_url_Get()));
            //trace.Record(Annotations.Tag("http.method", rpcData.http_method_Get()));
            //trace.Record(Annotations.Tag("http.path", rpcData.route));


            //try
            //{
            //    trace.Record(Annotations.Tag("ReqRpc", rpcData.oriJson.ToString()));
            //    string str = apiRequestMessage.value_OriData.ArraySegmentByteToString() ?? "";
            //    trace.Record(Annotations.Tag("ReqData", str ));
            //}
            //catch
            //{
            //}
            #endregion 

            JObject eventData = SplunkCollector.BuildEventData(null, rpcData, apiRequestMessage, GetApiReplyMessage, config.tags);
            foreach (var kv in eventData)
            {
                var key = kv.Key;
                var value = kv.Value;
                if (key != null && value != null)
                {
                    trace.Record(Annotations.Tag(key, value.ConvertToString()));
                }
            }

            trace.Record(Annotations.ClientRecv());
        }


        Config config;












        #region Config Model
        class Config
        {
            public float SamplingRate;
            public string zipkinCollectorUrl;
            public string rpcName;

            public string serviceStationName;

            public IDictionary<string, string> tags;
        }
        #endregion


        #region MyLogger       
        class MyLogger : ILogger
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void LogError(string message)
            {
                Logger.Error("[zipkin]" + message);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void LogInformation(string message)
            {
                Logger.Info("[zipkin]" + message);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void LogWarning(string message)
            {
                Logger.log.Log(Level.warn, "[zipkin]" + message);
            }
        }
        #endregion
    }
}
