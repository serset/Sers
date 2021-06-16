using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Sers.Core.Module.App;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Threading;

namespace App.Robot.Station.Logical
{
    public class TaskMng
    {
        public static void Init()
        {
        }


        /// <summary>
        ///  主线程开启的常驻线程，用以启动api触发的任务。
        ///  若在api中直接调用，则会导致 ApiClient中 RpcData错乱的问题
        ///  （RpcData通过AsyncCache保存api调用关系，故若api中直接开启线程调用api，可能会出现api中的 RpcData错乱）。
        /// </summary>
        public static readonly TaskQueue MainTask = new TaskQueue() { threadName = "Robot-MainTaskToStartTask" };

        public static readonly TaskMng Instance;
       
        static TaskMng()
        {
            //TaskController.MainTask
            SersApplication.onStart += () => MainTask.Start();
            SersApplication.onStop += () => MainTask.Stop();

            Instance = JsonFile.GetFromFile<TaskMng>(new[] { "Data", "App.Robot.json" }) ;

            if (null == Instance)
            {
                Instance = new TaskMng();
            }

            MainTask.AddTask(() =>
            {
                Thread.Sleep(2000);
                Instance.tasks.Values.Where(m => m.config.autoStart).ToList().ForEach(task => task.worker.Start());
            });

          
         
        }

      
                     
        public void TaskMngSaveToCache()
        {
            JsonFile.SetToFile(this, new[] { "Data", "App.Robot.json" });
        }



        [JsonProperty]
        ConcurrentDictionary<int, TaskItem> tasks = new ConcurrentDictionary<int, TaskItem>();

               


        public bool Add(TaskConfig config)
        {
            lock (this)
            {
                var id = 1;

                if (tasks.Count != 0)
                {
                    id = tasks.Keys.Max() + 1;
                }

                TaskItem taskItem = new TaskItem { config = config, id = id };

                if (!tasks.TryAdd(id, taskItem))
                {
                    return false;
                }

                if (config.autoStart)
                {
                    MainTask.AddTask(() =>
                    {
                        taskItem.worker.Start();
                    });
                }
                TaskMngSaveToCache();
                return true;
            }                     
        }
        public bool Start(int id)
        {
            if (tasks.TryGetValue(id, out var taskItem))
            {
                MainTask.AddTask(() =>
                {
                    taskItem.worker.Start();
                });           
                return true;
            }
            return false;
        }

        public bool Stop(int id)
        {
            if (tasks.TryGetValue(id, out var task))
            {
                task.worker.Stop();
                return true;
            }
            return false;
        }

        public bool Remove(int id)
        {
            if (tasks.TryGetValue(id, out var task))
            {
                task.worker.Stop();
                tasks.TryRemove(id, out _);

                TaskMngSaveToCache();

                return true;
            }
            return false;             
        }


        public  List<TaskItem> GetAll()
        {
            return tasks.Values.ToList();
        }

      

    }
}
