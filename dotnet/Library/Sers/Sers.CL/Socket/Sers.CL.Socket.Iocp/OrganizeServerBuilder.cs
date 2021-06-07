using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.Socket.Iocp
{
    public class OrganizeServerBuilder : IOrganizeServerBuilder
    {
        public void Build(List<IOrganizeServer> organizeList, JObject config)
        {

            #region security     
            Sers.Core.Util.StreamSecurity.SecurityManager securityManager = null;
            if (config["security"] is JArray securityConfigs)
            {
                securityManager = Sers.Core.Util.StreamSecurity.SecurityManager.BuildSecurityManager(securityConfigs);
            }
            #endregion
 

            string mode = config["mode"]?.ToString();

            switch (mode)
            {
                default:
                //case "Simple":
                    {
                        var delivery = new Mode.Simple.DeliveryServer();

                        delivery.securityManager = securityManager;

                        delivery.host = config["host"].ConvertToString() ?? delivery.host;
                        delivery.port = config["port"]?.ConvertBySerialize<int?>() ?? delivery.port;

                        #region 接收缓存区 配置
                        // 接收缓存区大小（单位:byte,默认：8192）
                        delivery.receiveBufferSize = config["receiveBufferSize"]?.ConvertBySerialize<int?>() ?? delivery.receiveBufferSize;
                        #endregion

                        organizeList.Add(new OrganizeServer(delivery, config));
                    }
                    break;
                case "Timer":              
                    {
                        var delivery = new Mode.Timer.DeliveryServer();

                        delivery.securityManager = securityManager;

                        delivery.host = config["host"].ConvertToString() ?? delivery.host;
                        delivery.port = config["port"]?.ConvertBySerialize<int?>() ?? delivery.port;

                        #region 接收缓存区 配置
                        // 接收缓存区大小（单位:byte,默认：8192）
                        delivery.receiveBufferSize = config["receiveBufferSize"]?.ConvertBySerialize<int?>() ?? delivery.receiveBufferSize;
                        #endregion

                        #region 发送缓冲区 配置
                        // 发送缓冲区刷新间隔（单位：毫秒,默认：1）
                        delivery.sendFlushInterval = config["sendFlushInterval"].ConvertBySerialize<int?>() ?? delivery.sendFlushInterval;

                        // 发送缓冲区数据块的最小大小（单位：byte,默认 1000000）                     
                        delivery.sendBufferSize = config["sendBufferSize"].ConvertBySerialize<int?>() ?? delivery.sendBufferSize;

                        // 发送缓冲区个数（默认1024）  
                        delivery.sendBufferCount = config["sendBufferCount"].ConvertBySerialize<int?>() ?? delivery.sendBufferSize;
                        #endregion

                        organizeList.Add(new OrganizeServer(delivery, config));
                    }
                    break;
                 case "ThreadWait":           
                    {
                        var delivery = new Mode.ThreadWait.DeliveryServer();

                        delivery.securityManager = securityManager;

                        delivery.host = config["host"].ConvertToString() ?? delivery.host;
                        delivery.port = config["port"]?.ConvertBySerialize<int?>() ?? delivery.port;

                        #region 接收缓存区 配置
                        // 接收缓存区大小（单位:byte,默认：8192）
                        delivery.receiveBufferSize = config["receiveBufferSize"]?.ConvertBySerialize<int?>() ?? delivery.receiveBufferSize;
                        #endregion


                        #region 发送缓冲区 配置
                        // 发送缓冲区刷新间隔（单位：毫秒,默认：1）
                        delivery.sendFlushInterval = config["sendFlushInterval"].ConvertBySerialize<int?>() ?? delivery.sendFlushInterval;

                        // 发送缓冲区数据块的最小大小（单位：byte,默认 1000000）                     
                        delivery.sendBufferSize = config["sendBufferSize"].ConvertBySerialize<int?>() ?? delivery.sendBufferSize;

                        // 发送缓冲区个数（默认1024）  
                        delivery.sendBufferCount = config["sendBufferCount"].ConvertBySerialize<int?>() ?? delivery.sendBufferSize;
                        #endregion

                        organizeList.Add(new OrganizeServer(delivery, config));
                    }
                    break;
            }


        }
       
    }
}
