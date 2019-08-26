using Microsoft.Extensions.DependencyInjection;
using Sers.Apm.SkyWalking.Core.Extensions;
using System.Collections.Generic;
using System.IO;
using Sers.Apm.SkyWalking.Core.Model;
using Sers.Apm.SkyWalking.Core.Base;

namespace Sers.Apm.SkyWalking.Core
{
    public class SkyWalkingManage
    {

        /// <summary>
        /// SkyWalking配置项 
        /// </summary>
        public static Dictionary<string, string> config { get; } = new Dictionary<string, string>
            {
                {"SkyWalking:Namespace", string.Empty},
                {"SkyWalking:ApplicationCode", "My_Application"},
                {"SkyWalking:SpanLimitPerSegment", "300"},
                {"SkyWalking:Sampling:SamplePer3Secs", "-1"},
                {"SkyWalking:Logging:Level", "Information"},
                {"SkyWalking:Logging:FilePath",  Path.Combine("logs", "SkyWalking-{Date}.log")},
                {"SkyWalking:Transport:Interval", "3000"},
                {"SkyWalking:Transport:PendingSegmentLimit", "30000"},
                {"SkyWalking:Transport:PendingSegmentTimeout", "1000"},
                {"SkyWalking:Transport:gRPC:Servers", "localhost:11800"},
                {"SkyWalking:Transport:gRPC:Timeout", "2000"},
                {"SkyWalking:Transport:gRPC:ConnectTimeout", "10000"}
            };

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Init(IServiceCollection services)
        {
            return ServiceCollectionExtensions.AddSersSkyWalking(services);
        }

        #region Send

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requests"></param>
        public static void Send(List<RequestModel> requests)
        {
            PushSpan(RequestTree.LoadFromRequestModel(requests));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestTree"></param>
        public static void Send(RequestTree requestTree)
        {
            PushSpan(requestTree);
        }


        static void PushSpan(RequestTree requestTree)
        {
            DiagnosticProcessor.BeginEntrySpan(requestTree);

            if (requestTree.children != null)
            {
                foreach (var item in requestTree.children)
                {
                    DiagnosticProcessor.BeginExitSpan(item);
                    PushSpan(item);
                    DiagnosticProcessor.EndExitSpan(item);
                }
            }

            DiagnosticProcessor.EndEntrySpan(requestTree);
        }
        #endregion
    }
}
