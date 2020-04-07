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

            #region (x.1)get id(traceId spanId parentSpanId) and  create trace 
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
            #endregion


            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.Rpc(config.rpcName));
            trace.Record(Annotations.ServiceName(rpcData.apiStationName_Get()??""));          
    

   


            return (s, apiReplyMessage) => {

                //gover会在内部把route处理为真正的route名称

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


                //tags
                config.tags?.ForEach(item =>trace.Record(Annotations.Tag(GetTagValue(item.Key) ?? "" , GetTagValue(item.Value) ?? "")));

      
                trace.Record(Annotations.ClientRecv());

                #region method getTagValue
                string GetTagValue(string valueString) 
                {
                    if (string.IsNullOrEmpty(valueString)) return null;
                    if (!valueString.StartsWith("{{") || !valueString.EndsWith("}}")) return valueString;

                    valueString = valueString.Substring(2, valueString.Length - 4);                               

                    string dataType;
                    string path;

                    var splitIndex = valueString.IndexOf('.');
                    if (splitIndex < 0)
                    {
                        dataType = valueString;
                        path = "";
                    }
                    else 
                    {
                        dataType = valueString.Substring(0, splitIndex);
                        path = valueString.Substring(splitIndex + 1);
                    }

                    switch (dataType)
                    {
                        case "requestRpc":return rpcData?.oriJson.SelectToken(path).ConvertToString();
                        case "requestData": return apiRequestMessage.value_OriData.ArraySegmentByteToString();
                    }
                    return null;
                }
                #endregion

            };
        }


        public void InitEvent(JObject arg)
        {           
            config = arg.Deserialize<Config>();
            if (string.IsNullOrEmpty(config.rpcName)) config.rpcName = "ServiceCenter";
            Logger.Info("[zipkin]初始化中... config: " + config?.Serialize());
        }


        Config config;
       

        public void BeforeStart()
        {
            if (config == null) return;
            #region (x.1)注册和启动 Zipkin
            if (config.SamplingRate <= 0 ) config.SamplingRate = 1;

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

        public string serviceStationName;

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
