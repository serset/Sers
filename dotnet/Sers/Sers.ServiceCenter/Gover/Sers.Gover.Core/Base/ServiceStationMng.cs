using Sers.Core.Extensions;
using Sers.Core.Module.Env;
using Sers.Gover.Core.Model;
using Sers.Gover.Core.Service.SersEvent;
using Sers.ServiceCenter;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sers.Gover.Core
{
    public class ServiceStationMng
    {
        GoverManage goverManage;
       
        public ServiceStationMng Init(GoverManage goverManage)
        {
            this.goverManage = goverManage;
            return this;
        }

        /// <summary>
        /// mqConnKey 和 服务站点 的映射
        /// </summary>
        ConcurrentDictionary<string, ServiceCenter.ServiceStation> serviceStation_MqConnKey_Map = new ConcurrentDictionary<string, ServiceCenter.ServiceStation>();

        /// <summary>
        ///  serviceStationKey 和 服务站点 的映射
        /// </summary>
        ConcurrentDictionary<string, ServiceCenter.ServiceStation> serviceStationKey_Map = new ConcurrentDictionary<string, ServiceCenter.ServiceStation>();


        public void PublishUsageInfo(EnvUsageInfo item)
        {
            if (serviceStationKey_Map.TryGetValue(item.serviceStationKey, out var serviceStation))
            {
                serviceStation.usageStatus = item.usageStatus;
            }
        }


        public List<ServiceStationData> ServiceStation_GetAll()
        {
            return serviceStation_MqConnKey_Map.Values.Select(
                (m) => new ServiceStationData
                {
                    mqConnKey = m.mqConnGuid,
                    deviceInfo = m.deviceInfo,
                    serviceStationInfo = m.serviceStationInfo,
                    status = "" + m.Status_Get(),
                    usageStatus=m.usageStatus,
                    counter=m.counter,
                    apiNodeCount =m.apiNodes.Count,
                    activeApiNodeCount = m.ActiveApiNodeCount_Get(),
                    apiStationNames= m.ApiStationNames_Get()
                }
            ).ToList();
        }


        #region action


        /// <summary>
        /// 添加并启用站点
        /// </summary>
        /// <param name="serviceStation"></param>
        public void ServiceStation_Add(ServiceCenter.ServiceStation serviceStation)
        {
            serviceStation_MqConnKey_Map[serviceStation.mqConnGuid] = serviceStation;

            serviceStationKey_Map[serviceStation.serviceStationKey] = serviceStation;            

            serviceStation.Status_Set(EServiceStationStatus.正常);
            goverManage.apiStationMng.ServiceStation_Add(serviceStation);

            //发布 Sers Event
            SersEventService.Publish(SersEventService.Event_ServiceStation_Add, serviceStation);

        }


        public void ServiceStation_Remove(string mqConnKey)
        {           
            if (!serviceStation_MqConnKey_Map.TryRemove(mqConnKey, out var serviceStation))
            {
                return;
            }   

            serviceStationKey_Map.TryRemove(serviceStation.serviceStationKey, out _);
            goverManage.apiStationMng.ServiceStation_Remove(serviceStation);

            //发布 Sers Event
            SersEventService.Publish(SersEventService.Event_ServiceStation_Remove, serviceStation);

        }


        public bool ServiceStation_Pause(string mqConnKey)
        {
            lock (this)
            {
                if (!serviceStation_MqConnKey_Map.TryGetValue(mqConnKey, out var serviceStation))
                {
                    return false;
                }               


                if (serviceStation.Status_Get() == EServiceStationStatus.暂停)
                {
                    return true;
                }  


                serviceStation.Status_Set(EServiceStationStatus.暂停);
                goverManage.apiStationMng.ServiceStation_Pause(serviceStation);

                //发布 Sers Event
                SersEventService.Publish(SersEventService.Event_ServiceStation_Pause, serviceStation);

                return true;
            }
        }
        public bool ServiceStation_Start(string mqConnKey)
        {
            lock (this)
            {
                if (!serviceStation_MqConnKey_Map.TryGetValue(mqConnKey, out var serviceStation))
                {
                    return false;
                }

                if (serviceStation.Status_Get() == EServiceStationStatus.正常)
                {
                    return true;
                }          

                serviceStation.Status_Set(EServiceStationStatus.正常);
                goverManage.apiStationMng.ServiceStation_Start(serviceStation);


                //发布 Sers Event
                SersEventService.Publish(SersEventService.Event_ServiceStation_Start, serviceStation);

                return true;
            }
        }



      

        #endregion



    }
}
