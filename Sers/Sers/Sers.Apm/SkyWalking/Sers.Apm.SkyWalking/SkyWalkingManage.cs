using Sers.Apm.SkyWalking.Core.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.ServiceCenter.ApiTrace;

namespace Sers.Apm.SkyWalking
{
    /// <summary>
    /// 注意：要手动复制 dll/runtimes文件夹
    /// </summary>
    public class SkyWalkingManage
    {
        
        public static Dictionary<string, string> config => SkyWalkingHost.config;

        public static void Init()
        {
            SkyWalkingHost.Init();

            TraceModel.OnPublish += traceMng.OnPublish;
        }

        static TraceMng traceMng = new TraceMng();

  

        public class TraceMng
        { 

            void Publish(List<TraceModel> traces)
            {
                var requests = traces.Select(m => new RequestModel
                {
                    requestId = m.requestId,
                    parentRequestId = m.parentRequestId,
                    startTime_dt = m.startTime,
                    endTime_dt = m.endTime,
                    route = m.route
                }).ToList();
                SkyWalkingHost.Publish(requests);
            }


            /// <summary>
            /// 
            /// </summary>
            readonly ConcurrentDictionary<string, List<TraceModel>> traceCache = new ConcurrentDictionary<string, List<TraceModel>>();


         

            public void OnPublish(TraceModel m)
            {
                var rootRid = m.rootRequestId;
                if (string.IsNullOrWhiteSpace(rootRid))
                {
                    rootRid = m.requestId;
                }

                lock (this)
                {
                    if (!traceCache.TryGetValue(rootRid, out var traces))
                    {
                        traces = new List<TraceModel>();
                        traceCache[rootRid] = traces;
                    }

                    traces.Add(m);

                    if (m.requestId == rootRid)
                    {
                        traceCache.TryRemove(rootRid, out _);
                        Publish(traces);
                    }
                }
            }



        }
    }
}
