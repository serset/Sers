using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Mq.Ipc.SharedMemory.Stream;
using System;

namespace Sers.Mq.Ipc.SharedMemory.Mq
{
    public class ClientMq : IClientMq
    {

        public ClientMq()
        {
            _mqConn.writeStream = writeStream;

            readStream.mqConn = _mqConn;
        }

        public string secretKey { get; set; }
        public Action<IMqConn> Conn_OnDisconnected { get; set; }

        readonly MqConn _mqConn = new MqConn();
        public IMqConn mqConn => _mqConn;



        //-------------------------------------------------------



        protected readonly ReadStream readStream = new ReadStream();
        protected readonly WriteStream writeStream = new WriteStream();

        /// <summary>
        /// 共享内存名称
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件          public delegate void OnGetFrame(AsyncUserToken token, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> OnGetFrame { set => readStream.OnReceiveMessage = value; get => readStream.OnReceiveMessage; }

        public void SendMessageAsync(byte[] msg)
        {
            writeStream.SendMessageAsync(msg);
        }

        public void Close()
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

        public bool Connect()
        {

            Logger.Info("[ClientMq] Ipc.SharedMemory,connecting... name:" + name);

            readStream.OnDisconnected = Conn_OnDisconnected;

            if (!writeStream.SharedMemory_Attach(name + ".ClientToServer"))
            {
                Close();
                return false;
            }

            if (!readStream.SharedMemory_Attach(name + ".ServerToClient"))
            {
                Close();
                return false;
            }


            if (!readStream.Start())
            {
                Close();
                return false;
            }
            if (!writeStream.Start())
            {
                Close();
                return false;
            }
            Logger.Info("[ClientMq] Ipc.SharedMemory,connected.");
            return true;
        }
    }
}
