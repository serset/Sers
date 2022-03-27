using System;
using System.Collections.Generic;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;

namespace Sers.CL.Ipc.SharedMemory
{
    public class DeliveryServer : IDeliveryServer
    {
        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _conn.securityManager = value; }

        public DeliveryServer()
        {
            _conn = new DeliveryConnection();

            _conn.OnDisconnected = (conn) =>
            {
                try
                {
                    Conn_OnDisconnected?.Invoke(conn);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                try
                {
                    Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                try
                {
                    Start();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            };

        }

        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; }

        readonly DeliveryConnection _conn;

        public IEnumerable<IDeliveryConnection> ConnectedList => new[] { _conn };





        /// <summary>
        /// 共享内存名称
        /// </summary>
        public string name { get; set; } = "Sers.CL.Ipc";

        /// <summary>
        /// 共享内存节点个数
        /// </summary>
        public int nodeCount { get; set; } = 64;
        /// <summary>
        /// 共享内存节点大小
        /// </summary>
        public int nodeBufferSize { get; set; } = 10240;


        public void Stop()
        {
            _conn.Close();
        }

        public bool Start()
        {
            Logger.Info("[CL.Ipc] Ipc.SharedMemory,starting", new { name });


            if (!_conn.InitAsServer(name, nodeCount, nodeBufferSize))
            {
                Stop();
                return false;
            }


            Conn_OnConnected?.Invoke(_conn);


            if (!_conn.Start())
            {
                Stop();
                return false;
            }

            Logger.Info("[CL.Ipc] Ipc.SharedMemory,started");
            return true;
        }
    }
}
