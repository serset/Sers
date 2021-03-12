using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    /// qps : 1400万   producer:32    consumer:32
    /// qps : 1200万   producer:16    consumer:16
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_WorkerPoolCascade<T> : IConsumer<T>
    {
        public int[] workCountArray  = { 16,1 };

        public int workThreadCount { get => workCountArray[0]; set => workCountArray = new[] { value, 1 }; }

        public string name { get; set; }

        public Action<T> processor { get; set; }

 


        List<Consumer_WorkerPool<T>> workerList=new List<Consumer_WorkerPool<T>>();


        int curRootIndex;
        Consumer_WorkerPool<T>[] rootWorkerList;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">从1开始</param>
        /// <returns></returns>
        Consumer_WorkerPool<T> BuildLevel(int level) 
        {
       
            int workerCount = workCountArray[level - 1];

            if (level == workCountArray.Length)
            {
                //最后一层

                var worker = new Consumer_WorkerPool<T>();
                worker.processor = processor;
                worker.workThreadCount = workerCount;

                workerList.Add(worker);
         
                return worker;
            }
            else 
            {
                var processorList = Enumerable.Range(0, workerCount).Select(m =>
                {
                    var child = BuildLevel(level + 1);
                    Action<T> processor = t => child.Publish(t);
                    return processor;
                }).ToArray();

                var worker = new Consumer_WorkerPool<T>();
                worker.processorList = processorList;
                workerList.Add(worker);

                return worker;
            }


            
        }


        public bool IsRunning { get; private set; } = false;
        public void Start()
        {
            lock (this)
            {
                if (IsRunning) return;
                IsRunning = true;

                rootWorkerList = Enumerable.Range(0, workCountArray[0]).Select(m =>
                {
                    return BuildLevel(2);
                }).ToArray();

                curRootIndex = 0;

                workerList.ForEach(m => m.Start());
            }
        }


        public void Stop()
        {
            lock (this)
            {
                if (!IsRunning) return;
                IsRunning = false;

                workerList.ForEach(m => m.Stop());
                workerList.Clear();

                rootWorkerList = null;
            }

        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T data)
        {  
            var index = Interlocked.Increment(ref curRootIndex);

            index = Math.Abs(index) % rootWorkerList.Length;      
            //index = Math.Abs(index) & 0b1111;

            rootWorkerList[index]?.Publish(data);
        }






    }
}
