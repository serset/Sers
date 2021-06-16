using System.Runtime.CompilerServices;
using System.Threading;
using App.Robot.Station.Logical.Worker;
using Newtonsoft.Json;

namespace App.Robot.Station.Logical
{
    public class TaskItem
    {
        public int id;

        public long sumCount = 0;
        public long sumFailCount = 0;

        public long curCount = 0;
        public long failCount = 0;

        public long targetCount = 0;


        public TaskConfig config { get => config_;  set { config_ = value; EventAfterSetConfig(); }  }
        private TaskConfig config_;


        public int RunningThreadCount => worker.RunningThreadCount;

        public bool IsRunning => worker.IsRunning;


        [JsonIgnore]
        public IWorker worker { get; private set; }

        void EventAfterSetConfig() 
        {
            var config = config_;

            targetCount = config.threadCount * config.loopCountPerThread;

            switch (config.type)
            {
                case "ApiClientAsync": worker = new Worker_ApiClientAsync(this); break;
                case "HttpClient": worker = new Worker_HttpClient(this); break;
                case "HttpUtil": worker = new Worker_HttpUtil(this); break;
                default: config.type = "ApiClient"; worker = new Worker_ApiClient(this); break;
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StepUp(bool success)
        {
            Interlocked.Increment(ref curCount);
            Interlocked.Increment(ref sumCount);
            if (!success)
            {
                Interlocked.Increment(ref sumFailCount);
                Interlocked.Increment(ref failCount);
            }

        }
    }
}
