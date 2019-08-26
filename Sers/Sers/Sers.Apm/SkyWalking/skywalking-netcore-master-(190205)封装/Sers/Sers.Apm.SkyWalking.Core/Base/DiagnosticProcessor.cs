using System;
using System.Collections.Generic;
using Sers.Apm.SkyWalking.Core.Model;
using SkyWalking.Components;
using SkyWalking.Context;
using SkyWalking.Context.Tag;
using SkyWalking.Context.Trace;
using SkyWalking.Diagnostics;


namespace Sers.Apm.SkyWalking.Core.Base
{
    public class DiagnosticProcessor
    {

          

        private static IContextCarrierFactory _contextCarrierFactory =>      global::SkyWalking.AspNetCore.Diagnostics.HostingTracingDiagnosticProcessor.Instance_ContextCarrierFactory;


        static void Inject(ISpan span, RequestModel request)
        {
            if (! (span is  AbstractTracingSpan tracingSpan))
            {
                return;
            }

        }



        [DiagnosticName("Microsoft.AspNetCore.Hosting.BeginRequest")]
        public static void BeginEntrySpan(RequestModel request)
        {
            var carrier = _contextCarrierFactory.Create();
            //foreach (var item in carrier.Items)
            //    item.HeadValue = httpContext.Request.Headers[item.HeadKey];
            var span = ContextManager.CreateEntrySpan($"EntrySpan: {request.route}", carrier);

            if (span is AbstractTracingSpan tracingSpan)
            {
                tracingSpan.startTime = request.startTime;
                tracingSpan.endTime = request.endTime;
            }
 

            span.AsHttp();
            span.SetComponent(ComponentsDefine.AspNetCore);
            Tags.Url.Set(span, request.route);
            Tags.HTTP.Method.Set(span, "GET");
            span.Log(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                new Dictionary<string, object>
                {
                    {"event", "AspNetCore Hosting BeginRequest"},
                    {"message", $"Request starting Http GET {request.route}"}
                });
            //httpContext.Items["sw3-http"] = httpRequestSpan;
        
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.EndRequest")]
        public static void EndEntrySpan(RequestModel request)
        {
            var span = ContextManager.ActiveSpan;
            if (span == null)
            {
                return;
            }

            var statusCode = 200;
            if (statusCode >= 400)
            {
                span.ErrorOccurred();
            }

            Tags.StatusCode.Set(span, statusCode.ToString());
            span.Log(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                new Dictionary<string, object>
                {
                    {"event", "AspNetCore Hosting EndRequest"},
                    {"message", $"Request finished {statusCode} {"text/json"}"}
                });
            ContextManager.StopSpan(span);           
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.UnhandledException")]
        public void UnhandledException(Exception exception)
        {
            ContextManager.ActiveSpan?.ErrorOccurred()?.Log(exception);
        }



        [DiagnosticName("System.Net.Http.Request")]
        public static void BeginExitSpan(RequestModel request)
        {
            //return;
            var contextCarrier = _contextCarrierFactory.Create();
            var peer = $"127.0.0.1:8888";
            var span = ContextManager.CreateExitSpan("ExitSpan " + request.route, contextCarrier, peer);

            if (span is AbstractTracingSpan tracingSpan)
            {
                tracingSpan.startTime = request.startTime;
                tracingSpan.endTime = request.endTime;
            }

            Tags.Url.Set(span, request.route);
            span.AsHttp();
            span.SetComponent(ComponentsDefine.HttpClient);
            Tags.HTTP.Method.Set(span, "GET");
            //foreach (var item in contextCarrier.Items)
            //    request.Headers.Add(item.HeadKey, item.HeadValue);
        }

        [DiagnosticName("System.Net.Http.Response")]
        public static void EndExitSpan(RequestModel request)
        {
        
            var span = ContextManager.ActiveSpan;
            if (span == null)
            {
                return;               
            }

            Tags.StatusCode.Set(span, "200");

            ContextManager.StopSpan(span); 
        }

        [DiagnosticName("System.Net.Http.Exception")]
        public void HttpException([Property(Name = "Exception")] Exception ex)
        {
            var span = ContextManager.ActiveSpan;
            if (span != null && span.IsExit)
            {
                span.ErrorOccurred();
            }
        }
    }
}
