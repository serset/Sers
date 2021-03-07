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

        Action<Object, Vit.Core.Util.Pipelines.ByteData> ApiScopeEvent(IRpcContextData rpcData, ApiMessage apiRequestMessage)
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

                #region method getTagValue


                string requestData_oriString = null;
                JObject requestData_json = null;

                ApiMessage apiResponseMessage = null;

                string responseRpc_oriString = null;
                JObject responseRpc_json = null;

                string responseData_oriString = null;
                JObject responseData_json = null;

                string GetTagValue(string valueString)
                {
                    if (string.IsNullOrEmpty(valueString)) return null;
                    if (!valueString.StartsWith("{{") || !valueString.EndsWith("}}")) return valueString;

                    try
                    {

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
                            case "requestRpc": return rpcData?.oriJson.SelectToken(path).ConvertToString();
                            case "requestData":
                                if (requestData_oriString == null)
                                {
                                    requestData_oriString = apiRequestMessage.value_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return requestData_oriString;
                                }
                                if (requestData_json == null)
                                {
                                    requestData_json = requestData_oriString.Deserialize<JObject>();
                                }
                                return requestData_json.SelectToken(path).ConvertToString();

                            case "responseRpc":
                                if (apiResponseMessage == null)
                                {
                                    apiResponseMessage = new ApiMessage();
                                    apiResponseMessage.Unpack(apiReplyMessage.ToArraySegment());
                                }
                                if (responseRpc_oriString == null)
                                {
                                    responseRpc_oriString = apiResponseMessage.rpcContextData_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return responseRpc_oriString;
                                }
                                if (responseRpc_json == null)
                                {
                                    responseRpc_json = responseRpc_oriString.Deserialize<JObject>();
                                }
                                return responseRpc_json.SelectToken(path).ConvertToString();

                            case "responseData":
                                if (apiResponseMessage == null)
                                {
                                    apiResponseMessage = new ApiMessage();
                                    apiResponseMessage.Unpack(apiReplyMessage.ToArraySegment());
                                }
                                if (responseData_oriString == null)
                                {
                                    responseData_oriString = apiResponseMessage.value_OriData.ArraySegmentByteToString();
                                }
                                if (string.IsNullOrEmpty(path))
                                {
                                    return responseData_oriString;
                                }
                                if (responseData_json == null)
                                {
                                    responseData_json = responseData_oriString.Deserialize<JObject>();
                                }
                                return responseData_json.SelectToken(path).ConvertToString();
                        }
                    }
                    catch
                    {
                    }
                    return null;
                }
                #endregion

                //tags
                config.tags?.ForEach(item =>
                {
                    var key = GetTagValue(item.Key);
                    var value = GetTagValue(item.Value);
                    if (key != null && value != null)
                    {
                        trace.Record(Annotations.Tag(key, value));
                    }
                });


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
