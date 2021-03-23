using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Robot.Station.Logical.Worker;
using Newtonsoft.Json;
using Vit.Core.Util.ConfigurationManager;

namespace App.Robot.Station.Logical
{
    public class TaskMng
    {
        static TaskMng LoadTaskMng()
        {
            var taskMng = JsonFile.GetFromFile<TaskMng>(new[] { "Data", "App.Robot.json" }) ;

            if (null == taskMng)
            {
                taskMng= new TaskMng();
            }

            taskMng.tasks.Values.Where(m => m.config.autoStart).ToList().ForEach(task => task.Start());
            return taskMng;
        }

        public static readonly TaskMng Instance = LoadTaskMng();





        public void TaskMngSaveToCache()
        {
            JsonFile.SetToFile(this, new[] { "Data", "App.Robot.json" });
        }



        [JsonProperty]
         ConcurrentDictionary<int, IWorker> tasks = new ConcurrentDictionary<int, IWorker>();
        [JsonProperty]
        int curKeyIndex = 0;



        public TaskMng()
        {
        }

     

        private int GetNewKey() => Interlocked.Increment(ref curKeyIndex);

       
        public bool Add(TaskConfig config)
        {
            var key = GetNewKey();

            IWorker task;

            switch (config.type)
            {
                case "ApiClientAsync": task = new Worker_ApiClientAsync(config); break;
                case "HttpClient": task = new Worker_HttpClient(config); break;
                case "HttpUtil": task = new Worker_HttpUtil(config); break;
                default: config.type = "ApiClient"; task = new Worker_ApiClient(config); break;
            }

            task.id = key;
            return tasks.TryAdd(key, task);
        }
        public bool Start(int id)
        {
            if (tasks.TryGetValue(id, out var task))
            {
                task.Start();
                return true;
            }
            return false;
        }

        public bool Stop(int id)
        {
            if (tasks.TryGetValue(id, out var task))
            {
                task.Stop();
                return true;
            }
            return false;
        }

        public bool Remove(int id)
        {
            if (tasks.TryGetValue(id, out var task))
            {
                task.Stop();
                tasks.TryRemove(id, out _);
                return true;
            }
            return false;             
        }


        public  List<IWorker> GetAll()
        {
            return tasks.Values.ToList();
        }

      

    }
}
