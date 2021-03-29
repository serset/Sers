 
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Statistics
{


    public class QpsData 
    {
        public QpsData(StatisticsQpsAsync stat) 
        {
            lock (stat.qpsList) 
            {
                stat.qpsList.Add(this);
            }
        }

        public long RequestCount = 0;
 
    }


    public class StatisticsQpsAsync
    {
        public  List<QpsData> qpsList = new List<QpsData>();

  

        bool finished = false;
        public void Start(string name)
        {
            
            finished = false;
       
            Console.WriteLine("¿ªÊ¼");

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
            Console.WriteLine("½áÊø");
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

            msg += $" qps: {qps.ToString("0.00")} ";
            msg += $" curCount: {curCount} ";
            msg += $" sumCount: {curSumCount} ";
           
            
            return msg;
        }
    }
}