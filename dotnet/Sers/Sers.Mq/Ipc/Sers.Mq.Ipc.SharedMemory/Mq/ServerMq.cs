using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Mq.Ipc.SharedMemory.Stream;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Mq.Ipc.SharedMemory.Mq
{
    public class ServerMq :  IServerMq
    {
        public ServerMq()
        {
            _mqConn.writeStream = writeStream;
            readStream.mqConn = _mqConn;
        }

        public Action<IMqConn> Conn_OnDisconnected { get; set; }
        public Action<IMqConn> Conn_OnConnected { get; set; }

        readonly MqConn _mqConn = new MqConn();
 
        public IEnumerable<IMqConn> ConnectedList =>new[]{ _mqConn };


        protected readonly ReadStream readStream = new ReadStream();
        protected readonly WriteStream writeStream = new WriteStream();


        /// <summary>
        /// 共享内存名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 共享内存节点个数
        /// </summary>
        public int nodeCount { get; set; } = 64;
        /// <summary>
        /// 共享内存节点大小
        /// </summary>
        public int nodeBufferSize { get; set; } = 10240;

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件          public delegate void OnReceiveData(AsyncUserToken token, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> Conn_OnGetFrame { set => readStream.OnReceiveMessage = value; get => readStream.OnReceiveMessage; }
    

        //public void SendMessageAsync(byte[] msg)
        //{
        //    writeStream.SendMessageAsync(msg);
        //}

        public void Stop()
        {
            try
            {
                writeStream.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                readStream.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public bool Start()
        {
            Logger.Info("[ServerMq] Ipc.SharedMemory,starting... name:" + name);

            readStream.OnDisconnected = (conn)=> 
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

            if (!writeStream.SharedMemory_Malloc(name + ".ServerToClient", nodeCount, nodeBufferSize))
            {
                Stop();
                return false;
            }

            if (!readStream.SharedMemory_Malloc(name + ".ClientToServer", nodeCount, nodeBufferSize))
            {
                Stop();
                return false;
            }



            if (!readStream.Start())
            {
                Stop();
                return false;
            }
            if (!writeStream.Start())
            {
                Stop();
                return false;
            }

            Logger.Info("[ServerMq] Ipc.SharedMemory,started.");
            return true;
        }
    }
}
