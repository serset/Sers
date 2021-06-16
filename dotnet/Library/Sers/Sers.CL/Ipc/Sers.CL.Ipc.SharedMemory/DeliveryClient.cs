using System;
using Sers.CL.Ipc.SharedMemory.Stream;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;

namespace Sers.CL.Ipc.SharedMemory
{
    public class DeliveryClient : IDeliveryClient
    {

        public DeliveryClient()
        {
            _conn = new DeliveryConnection();      
        }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件 
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set => _conn.OnGetFrame = value; }

        public Action<IDeliveryConnection> Conn_OnDisconnected {  set => _conn.OnDisconnected = value; }

        readonly DeliveryConnection _conn ;
        public IDeliveryConnection conn => _conn;



        //-------------------------------------------------------
 

        /// <summary>
        /// 共享内存名称
        /// </summary>
        public string name { get; set; } = "Sers.CL.Ipc";


        public void Close()
        {
            _conn.Close();           
        }

        public bool Connect()
        {

            Logger.Info("[CL.Ipc] Ipc.SharedMemory,connecting... name:" + name);          

            if (!_conn.InitAsClient(name))
            {
                Close();
                return false;
            }


            if (!_conn.Start())
            {
                Close();
                return false;
            }

          
            Logger.Info("[CL.Ipc] Ipc.SharedMemory,connected.");
            return true;
        }
    }
}
