 
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace Statistics
{


    public class QpsData 
    {
        public QpsData(StatisticsQpsAsync stat)
        {
            stat.qpsList.Enqueue(this);
        }

        public long RequestCount = 0;
 
    }


    public class StatisticsQpsAsync
    {
        public  ConcurrentQueue<QpsData> qpsList = new ConcurrentQueue<QpsData>();

  

        bool finished = false;
        public void Start(string name)
        {
            
            finished = false;
       
            Console.WriteLine("开始");

            Task.Run(() => {

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
            Console.WriteLine("结束");
            Console.WriteLine(ToString());
        }
 

    

        long lastSumCount = 0;
    
        DateTime lastTime=DateTime.Now;
        public override string ToString()
        {
            var curTime = DateTime.Now;

           var curSumCount = qpsList.Sum(data => Interlocked.Read(ref data.RequestCount));
            //var curSumCount = qpsList.Sum(data =>  (long)data.RequestCount);

            long curCount = curSumCount - lastSumCount;
            double qps = curCount / (curTime - lastTime).TotalSeconds;
            lastSumCount = curSumCount;
            lastTime=curTime;

            var msg = "";

            //qps

            msg += $" qps: {qps.ToString("0.00")}万 ";
            msg += $" curCount: {curCount} ";
            msg += $" sumCount: {curSumCount} ";
           
            
            return msg;
        }
    }
}