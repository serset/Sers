using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Sers.Core.Module.Env;
using Sers.Gover.Base.Model;
using Sers.Gover.Service.SersEvent;
using Sers.ServiceCenter.Entity;

using Vit.Extensions;

namespace Sers.Gover.Base
{
    public class ServiceStationMng
    {
        GoverApiCenterService goverManage;

        public ServiceStationMng Init(GoverApiCenterService goverManage)
        {
            this.goverManage = goverManage;
            return this;
        }

        public ICollection<ServiceStation> serviceStationCollection => serviceStation_ConnKey_Map.Values;


        /// <summary>
        /// connKey 和 服务站点 的映射
        /// </summary>
        readonly ConcurrentDictionary<int, ServiceStation> serviceStation_ConnKey_Map = new ConcurrentDictionary<int, ServiceStation>();

        /// <summary>
        ///  serviceStationKey 和 服务站点 的映射
        /// </summary>
        /// 
        readonly Reference<string, ServiceStation> serviceStationKey_Map = new Reference<string, ServiceStation>();


        public void SaveUsageInfo(EnvUsageInfo item)
        {
            lock (this)
            {
                var station = serviceStationKey_Map.Get(item.serviceStationKey);
                if (station == null) return;

                if (item.usageStatus != null)
                    station.usageStatus = item.usageStatus;

                if (item.Process != null)
                    station.Process = item.Process;
            }
        }


        public List<ServiceStationData> ServiceStation_GetAll()
        {
            return serviceStation_ConnKey_Map.Values
                .Select(
                    m => new ServiceStationData
                    {
                        connKey = "" + m.connKey,
                        connectionIp = m.connectionIp,
                        startTime = m.startTime,
                        deviceInfo = m.deviceInfo,
                        serviceStationInfo = m.serviceStationInfo,
                        status = "" + m.Status_Get(),
                        usageStatus = m.usageStatus,
                        Process = m.Process,
                        counter = m.counter,
                        qps = m.qps,
                        apiNodeCount = m.apiNodes.Count,
                        activeApiNodeCount = m.ActiveApiNodeCount_Get(),
                        apiStationNames = m.ApiStationNames_Get()
                    }
                ).OrderBy(m => m?.serviceStationInfo?.serviceStationName)
                .ThenBy(m => m.startTime)
                .ToList();
        }


        #region action


        /// <summary>
        /// 添加并启用站点
        /// </summary>
        /// <param name="serviceStation"></param>
        public void ServiceStation_Add(ServiceStation serviceStation)
        {
            lock (this)
            {
                serviceStation.startTime = DateTime.Now;

                serviceStation_ConnKey_Map[serviceStation.connKey] = serviceStation;


                if (string.IsNullOrEmpty(serviceStation.serviceStationInfo.serviceStationKey))
                    serviceStation.serviceStationInfo.serviceStationKey = "tmp" + serviceStation.GetHashCode();

                serviceStationKey_Map.Add(serviceStation.serviceStationKey, serviceStation);

                serviceStation.Status_Set(EServiceStationStatus.正常);
                goverManage.apiStationMng.ServiceStation_Add(serviceStation);

                //发布 Sers Event
                SersEventService.Publish(SersEventService.Event_ServiceStation_Add, serviceStation);
            }
        }



        /// <summary>
        /// 更新服务站点设备硬件信息
        /// </summary>
        /// <param name="newServiceStation"></param>
        public bool ServiceStation_UpdateStationInfo(ServiceStation newServiceStation)
        {
            lock (this)
            {
                if (!serviceStation_ConnKey_Map.TryGetValue(newServiceStation.connKey, out var serviceStation))
                {
                    return false;
                }

                serviceStationKey_Map.Remove(serviceStation.serviceStationKey);


                if (newServiceStation.serviceStationInfo != null)
                {
                    serviceStation.serviceStationInfo = newServiceStation.serviceStationInfo;

                    if (string.IsNullOrEmpty(serviceStation.serviceStationInfo.serviceStationKey))
                        serviceStation.serviceStationInfo.serviceStationKey = "tmp" + serviceStation.GetHashCode();
                }

                if (newServiceStation.deviceInfo != null)
                    serviceStation.deviceInfo = newServiceStation.deviceInfo;

                if (newServiceStation.Process != null)
                    serviceStation.Process = newServiceStation.Process;

                serviceStationKey_Map.Add(serviceStation.serviceStationKey, serviceStation);

                return true;
            }
        }



        public ServiceStation ServiceStation_Remove(string connKey)
        {
            lock (this)
            {
                if (!serviceStation_ConnKey_Map.TryRemove(int.Parse(connKey), out var serviceStation))
                {
                    return null;
                }

                serviceStationKey_Map.Remove(serviceStation.serviceStationKey);

                goverManage.apiStationMng.ServiceStation_Remove(serviceStation);

                //发布 Sers Event
                SersEventService.Publish(SersEventService.Event_ServiceStation_Remove, serviceStation);
                return serviceStation;
            }
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



        #region Reference
        /// <summary>
        /// 线程不安全
        /// </summary>
        /// <typeparam name="KeyType"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        class Reference<KeyType, ValueType>
        {
            class Item
            {
                public int count = 1;
                public ValueType value;
            }


            SortedDictionary<KeyType, Item> map = new SortedDictionary<KeyType, Item>();

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public ValueType Get(KeyType key)
            {
                if (map.TryGetValue(key, out var item))
                {
                    return item.value;
                }
                return default;
            }


            public ValueType Add(KeyType key, ValueType value)
            {
                if (map.TryGetValue(key, out var item))
                {
                    item.count++;
                    return item.value;
                }

                map.Add(key, new Item { value = value });

                return value;
            }

            public ValueType Remove(KeyType key)
            {
                if (map.TryGetValue(key, out var item))
                {
                    if ((--item.count) <= 0)
                    {
                        map.Remove(key);
                    }
                    return item.value;
                }
                return default;
            }


        }
        #endregion



    }
}
