using System.Collections.Generic;
using App.Robot.Station.Logical;
using App.Robot.Station.Logical.Model;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.App;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.Threading;

namespace App.Robot.Station.Controllers
{
    public class TaskController : IApiController
    {
        /// <summary>
        ///  主线程开启的常驻线程，用以启动api触发的任务。
        ///  若在api中直接调用，则会导致 ApiClient中 RpcData错乱的问题
        ///  （RpcData通过AsyncCache保存api调用关系，故若api中直接开启线程调用api，可能会出现api中的 RpcData错乱）。
        /// </summary>
        public static readonly TaskQueue MainTask = new TaskQueue() { threadName = "Robot-MainTaskToStartTask" };

        static TaskController()
        {
            //TaskController.MainTask
            SersApplication.onStart += () => TaskController.MainTask.Start();
            SersApplication.onStop += () => TaskController.MainTask.Stop();
        }
 

        /// <summary>
        /// 保存到Cache
        /// </summary>
        /// <returns></returns>
        [SsRoute("taskMng/saveToCache")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn TaskMngSaveToCache()
        {
            TaskMng.Instance.TaskMngSaveToCache();
            return true;
        }


        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns>ArgModelDesc-returns</returns>
        [SsRoute("task/getAll")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn<List<Task>> GetAll()
        {
            return TaskMng.Instance.GetAll();
        }


        /// <summary>
        /// 创建新任务
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [SsRoute("task/add")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Add(TaskConfig config)
        {
            MainTask.AddTask(() =>
            {
                TaskMng.Instance.Add(config);
            });
            return true;
        }


        [SsRoute("task/start")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Start(int id)
        {
            MainTask.AddTask(() =>
            {
                TaskMng.Instance.Start(id);
            });
            return true;
        }

        [SsRoute("task/stop")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Stop(int id)
        {
            return TaskMng.Instance.Stop(id);
        }


        [SsRoute("task/remove")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Remove(int id)
        {
            return TaskMng.Instance.Remove(id);
        }

    }
}
