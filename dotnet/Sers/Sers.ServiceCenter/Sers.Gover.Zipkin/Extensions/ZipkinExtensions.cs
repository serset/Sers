using System;
using System.Collections.Generic;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Gover.Base;
using Vit.Core.Module.Log;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace Vit.Extensions
{
    public static partial class ZipkinExtensions
    {
        class Config
        {
            public float SamplingRate;
            public string zipkinCollectorUrl;
        }

        /// <summary>
        /// 从 appsettings.json::Sers.ServiceCenter.zipkin加载配置启用zipkin(若未指定则不启用zipkin)
        /// </summary>
        /// <param name="data"></param>
        public static void UseZipkin(this GoverManage data)
        {

            var config = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<Config>("Sers.ServiceCenter.zipkin");

            if (config == null) return;

            #region (x.1)注册和启动 Zipkin
            if (config.SamplingRate <= 0 || config.SamplingRate > 1) config.SamplingRate = 1;

            TraceManager.SamplingRate = config.SamplingRate;

            // 在链路追踪控制台获取 Zipkin Endpoint,注意 Endpoint 中不包含“/api/v2/spans”。
            var httpSender = new HttpZipkinSender(config.zipkinCollectorUrl, "application/json");

            var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());

            TraceManager.RegisterTracer(tracer);
            TraceManager.Start(new MyLogger());
            #endregion


            data.AddApiScopeEvent(ApiScopeEvent);

        }

        static Action<Object, List<ArraySegment<byte>>> ApiScopeEvent(IRpcContextData rpcData, ApiMessage apiRequestMessage)
        {
            //记录请求数据

            Trace trace = null;
            try
            {
                long spanId = 0;
                long traceId = 0;
                long? parentSpanId = null;

                {
                    var hexStr = rpcData.caller_rid_Get();
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

            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.Rpc("ServiceCenter"));
            trace.Record(Annotations.ServiceName(rpcData.apiStationName_Get()));          
            trace.Record(Annotations.Tag("http.url", rpcData.http_url_Get()));
            trace.Record(Annotations.Tag("http.method", rpcData.http_method_Get()));

            #region rpc data
            try
            {
                trace.Record(Annotations.Tag("ReqRpc", rpcData.oriJson.ToString()));
                string str = apiRequestMessage.value_OriData.ArraySegmentByteToString();
                trace.Record(Annotations.Tag("ReqData", str ?? "")); 
            }
            catch
            {
            }
            #endregion


            return (s, apiReplyMessage) => {

                //gover会在内部把route处理为真正的route名称
                trace.Record(Annotations.Tag("http.path", rpcData.route));
                trace.Record(Annotations.ClientRecv()); 
            };
        }


        #region MyLogger       
        class MyLogger : ILogger
        {
            public void LogError(string message)
            {
                Logger.Error("[zipkin]" + message);
            }

            public void LogInformation(string message)
            {
                Logger.Info("[zipkin]" + message);
            }

            public void LogWarning(string message)
            {
                Logger.Info("[zipkin]" + message);
            }
        }
        #endregion
    }
}
