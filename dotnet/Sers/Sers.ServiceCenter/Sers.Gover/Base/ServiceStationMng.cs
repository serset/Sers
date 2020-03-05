using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Env;
using Sers.Gover.Base.Model;
using Sers.Gover.Service.SersEvent;
using Sers.ServiceCenter.Entity;
using Vit.Extensions;

namespace Sers.Gover.Base
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
        /// connKey 和 服务站点 的映射
        /// </summary>
        ConcurrentDictionary<int, ServiceStation> serviceStation_ConnKey_Map = new ConcurrentDictionary<int, ServiceStation>();

        /// <summary>
        ///  serviceStationKey 和 服务站点 的映射
        /// </summary>
        ConcurrentDictionary<string, ServiceStation> serviceStationKey_Map = new ConcurrentDictionary<string, ServiceStation>();


        public void PublishUsageInfo(EnvUsageInfo item)
        {
            if (!string.IsNullOrEmpty(item.serviceStationKey) && serviceStationKey_Map.TryGetValue(item.serviceStationKey, out var serviceStation))
            {
                serviceStation.usageStatus = item.usageStatus;
            }
        }


        public List<ServiceStationData> ServiceStation_GetAll()
        {
            return serviceStation_ConnKey_Map.Values.Select(
                (m) => new ServiceStationData
                {
                    connKey = ""+m.connection.GetHashCode(),
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
        public void ServiceStation_Add(ServiceStation serviceStation)
        {
            serviceStation_ConnKey_Map[serviceStation.connection.GetHashCode()] = serviceStation;


            if (string.IsNullOrEmpty(serviceStation.serviceStationInfo.serviceStationKey)) 
                serviceStation.serviceStationInfo.serviceStationKey = "tmp" + serviceStation.GetHashCode();
            serviceStationKey_Map[serviceStation.serviceStationKey] = serviceStation;            

            serviceStation.Status_Set(EServiceStationStatus.正常);
            goverManage.apiStationMng.ServiceStation_Add(serviceStation);

            //发布 Sers Event
            SersEventService.Publish(SersEventService.Event_ServiceStation_Add, serviceStation);

        }



        /// <summary>
        /// 更新服务站点设备硬件信息
        /// </summary>
        /// <param name="serviceStation"></param>
        public bool ServiceStation_UpdateStationInfo(ServiceStation serviceStation)
        {
            if (!serviceStation_ConnKey_Map.TryGetValue(serviceStation.connection.GetHashCode(), out var old_serviceStation))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(old_serviceStation.serviceStationKey))
                serviceStationKey_Map.TryRemove(old_serviceStation.serviceStationKey, out _);


            if (serviceStation.serviceStationInfo != null)
                old_serviceStation.serviceStationInfo = serviceStation.serviceStationInfo;

            if (serviceStation.deviceInfo != null)
                old_serviceStation.deviceInfo = serviceStation.deviceInfo;

            if (string.IsNullOrEmpty(old_serviceStation.serviceStationInfo.serviceStationKey)) 
                old_serviceStation.serviceStationInfo.serviceStationKey = "tmp" + old_serviceStation.GetHashCode();
            serviceStationKey_Map[old_serviceStation.serviceStationKey] = old_serviceStation;

            return true;
        }



        public ServiceStation ServiceStation_Remove(string connKey)
        {           
            if (!serviceStation_ConnKey_Map.TryRemove(int.Parse(connKey), out var serviceStation))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(serviceStation.serviceStationKey))
                serviceStationKey_Map.TryRemove(serviceStation.serviceStationKey, out _);

            goverManage.apiStationMng.ServiceStation_Remove(serviceStation);

            //发布 Sers Event
            SersEventService.Publish(SersEventService.Event_ServiceStation_Remove, serviceStation);
            return serviceStation;
        }


        public ServiceStation ServiceStation_Pause(string connKey)
        {
            lock (this)
            {
                if (!serviceStation_ConnKey_Map.TryGetValue(int.Parse(connKey), out var serviceStation))
                {
                    return null;
                }               


                if (serviceStation.Status_Get() == EServiceStationStatus.暂停)
                {
                    return serviceStation;
                }  


                serviceStation.Status_Set(EServiceStationStatus.暂停);
                goverManage.apiStationMng.ServiceStation_Pause(serviceStation);

                //发布 Sers Event
                SersEventService.Publish(SersEventService.Event_ServiceStation_Pause, serviceStation);

                return serviceStation;
            }
        }
        public ServiceStation ServiceStation_Start(string connKey)
        {
            lock (this)
            {
                if (!serviceStation_ConnKey_Map.TryGetValue(int.Parse(connKey), out var serviceStation))
                {
                    return null;
                }

                if (serviceStation.Status_Get() == EServiceStationStatus.正常)
                {
                    return serviceStation;
                }          

                serviceStation.Status_Set(EServiceStationStatus.正常);
                goverManage.apiStationMng.ServiceStation_Start(serviceStation);


                //发布 Sers Event
                SersEventService.Publish(SersEventService.Event_ServiceStation_Start, serviceStation);

                return serviceStation;
            }
        }



      

        #endregion



    }
}
