using System.Collections.Generic;
using App.Robot.ServiceStation.Logical;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.ConfigurationManager;
using Sers.ServiceStation.Util.StaticFileTransmit;

namespace App.Station.Robot.Controllers
{ 
    public class TaskController : IApiController
    {

        #region UseStaticFiles

        // wwwroot 路径从配置文件获取
        static StaticFileMap staticFileMap = new StaticFileMap(ConfigurationManager.Instance.GetByPath<string>("Host.wwwroot"));

        /// <summary>
        /// UseStaticFiles
        /// </summary>
        /// <returns></returns>
        [SsRoute("*")]
        public byte[] UseStaticFiles()
        {
            return staticFileMap.TransmitFile();
        }
        #endregion


        /// <summary>
        /// 保存到Cache
        /// </summary>
        /// <returns></returns>
        [SsRoute("taskMng/saveToCache")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn  TaskMngSaveToCache()
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
        public ApiReturn  Add(TaskConfig config)
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
