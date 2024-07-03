/// v4

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CLClient.Statistics
{
    public class StatisticsQpsInfo
    {
        //public static StatisQpsInfo Instance = new StatisQpsInfo();


        string name = "";


        DateTime? startTime;

        bool finished = false;
        public void Start(string name)
        {
            this.name += name;
            finished = false;
            startTime = DateTime.Now;
            Console.WriteLine("¿ªÊ¼");

            Task.Run(() =>
            {

                while (!finished)
                {
                    Console.WriteLine(ToString());
                    Thread.Sleep(1000);
                }

            });


        }
        public void Stop()
        {
            finished = true;
            Console.WriteLine("½áÊø");
            Console.WriteLine(ToString());
        }

        public int RequestCount = 0;
        public void IncrementRequest() => Interlocked.Increment(ref RequestCount);
        public void IncrementRequest(int count) => Interlocked.Add(ref RequestCount, count);

        public long RequestTicks = 0;
        public void IncrementRequestTicks(long value) => Interlocked.Add(ref RequestTicks, value);


        public int ErrorCount = 0;
        public void IncrementError() => Interlocked.Increment(ref ErrorCount);


        long lastRequestTicks = 0;
        int lastCount = 0;
        DateTime lastTime;
        public override string ToString()
        {
            var curCount = RequestCount;
            var curRequestTicks = RequestTicks;

            var msg = name;

            double d;

            //msg += $"ReqCount: {curCount}";
            //if (curCount > 0)
            //{
            //    d = ErrorCount * 100.0 / curCount;
            //    msg += $",error:{ErrorCount}({d.ToString("0.00")}%)";
            //}

            if (startTime.HasValue)
            {
                if (lastCount == 0)
                {
                    lastTime = startTime.Value;
                }
                var curTime = DateTime.Now;
                double ms;

                //sum
                //ms = (curTime - startTime.Value).TotalMilliseconds;
                //d =  curCount / ms * 1000;
                //msg += $",qps:{ d.ToString("0.00") }";

                //ms = 1.0 * curRequestTicks / TimeSpan.TicksPerMillisecond;
                //d = (curCount <= 0 ? 0 : ms / curCount);
                //msg += $",ms/req:{ d.ToString("0.00") }";


                //cur
                //msg += $",------Cur ";                
                msg += $" ReqCount: {curCount}Ç§";
                ms = (curTime - lastTime).TotalMilliseconds;
                d = (curCount - lastCount) / ms * 1000;
                msg += $",qps:{d.ToString("0.00")}Ç§";
                ms = 1.0 * (curRequestTicks - lastRequestTicks) / TimeSpan.TicksPerMillisecond;
                d = (curCount <= lastCount ? 0 : ms / (curCount - lastCount));
                msg += $",ms/req:{d.ToString("0.00")}Ç§";


                lastRequestTicks = curRequestTicks;
                lastCount = curCount;
                lastTime = curTime;
                return msg;
            }
            return msg;
        }
    }
}