using System.Collections.Generic;
using App.Robot.Station.Logical;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;

namespace App.Robot.Station.Controllers
{
    public class TaskController : IApiController
    {

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
        public ApiReturn<List<TaskItem>> GetAll()
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
            return TaskMng.Instance.Add(config);         
        }


        [SsRoute("task/start")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Start(int id)
        {
            return TaskMng.Instance.Start(id);
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
