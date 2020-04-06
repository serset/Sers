using Newtonsoft.Json.Linq;
using Sers.Core.Module.App;
using Sers.Core.Module.App.AppEvent;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Gover.Base;
using System;
using System.Collections.Generic;
using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Extensions.IEnumerable;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace Sers.Gover.Apm.Zipkin
{

    public class AppEvent : IAppEvent
    {        

        Action<Object, List<ArraySegment<byte>>> ApiScopeEvent(IRpcContextData rpcData, ApiMessage apiRequestMessage)
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
            trace.Record(Annotations.Rpc(config.rpcName));
            trace.Record(Annotations.ServiceName(rpcData.apiStationName_Get()));
            trace.Record(Annotations.Tag("http.url", rpcData.http_url_Get()));
            trace.Record(Annotations.Tag("http.method", rpcData.http_method_Get()));
            trace.Record(Annotations.Tag("serviceStation", serviceStationName));

            //tags
            config.tags?.ForEach(item => trace.Record(Annotations.Tag(item.Key, item.Value)));

            #region rpc data
            try
            {
                trace.Record(Annotations.Tag("ReqRpc", rpcData.oriJson.ToString()));
                string str = apiRequestMessage.value_OriData.ArraySegmentByteToString() ?? "";
                trace.Record(Annotations.Tag("ReqData", str ));
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


        public void InitEvent(JObject arg)
        {           
            config = arg.Deserialize<Config>();
            if (string.IsNullOrEmpty(config.rpcName)) config.rpcName = "ServiceCenter";
            Logger.Info("[zipkin]初始化中... config: " + config?.Serialize());
        }


        Config config;
        string serviceStationName;

        public void BeforeStart()
        {
            if (config == null) return;
            #region (x.1)注册和启动 Zipkin
            if (config.SamplingRate <= 0 || config.SamplingRate > 1) config.SamplingRate = 1;

            TraceManager.SamplingRate = config.SamplingRate;

            // 在链路追踪控制台获取 Zipkin Endpoint,注意 Endpoint 中不包含“/api/v2/spans”。
            HttpZipkinSender httpSender = new HttpZipkinSender(config.zipkinCollectorUrl, "application/json");

            var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());

            TraceManager.RegisterTracer(tracer);
            TraceManager.Start(new MyLogger());
            #endregion


            GoverManage.Instance.AddApiScopeEvent(ApiScopeEvent);

            Logger.Info("[zipkin]启动成功");
        }

       

        public void OnStart()
        { 
        }
        
        public void AfterStart()
        {
            serviceStationName = SersApplication.serviceStationInfo?.serviceStationName??"";
        }
        public void BeforeStop()
        {
            TraceManager.Stop();
           
        }
        public void AfterStop()
        {
            
        }
    }


    #region Config Model
    class Config
    {
        public float SamplingRate;  
        public string zipkinCollectorUrl;
        public string rpcName;

        public IDictionary<string, string> tags;
    }
    #endregion

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
            Logger.log.Log(Level.WARN, "[zipkin]" + message);
        }
    }
    #endregion
}
