using System;
using System.Runtime.CompilerServices;

using Sers.CL.Socket.Iocp.Base;

using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Timer;

namespace Sers.CL.Socket.Iocp.Mode.Timer
{
    public class DeliveryServer : DeliveryServer_Base<DeliveryConnection>
    {

        #region NewConnection

        /// <summary>
        /// 发送缓冲区数据块的最小大小（单位：byte,默认 1000000）
        /// </summary>
        public int sendBufferSize = 1_000_000;

        /// <summary>
        /// 发送缓冲区个数（默认1024）
        /// </summary>
        public int sendBufferCount = 1024;
        public override DeliveryConnection NewConnection()
        {
            var conn = base.NewConnection();
            conn.SetConfig(sendBufferSize, sendBufferCount);
            return conn;
        }
        #endregion




        #region Start Stop

        public override bool Start()
        {
            try
            {
                Logger.Info("[CL.DeliveryServer] Socket.Iocp,starting", new { host, port });

                if (!base.Start())
                {
                    return false;
                }

                //(x.2)               
                Send_timer.intervalMs = sendFlushInterval;
                Send_timer.timerCallback = Send_Flush;
                Send_timer.Start();

                Logger.Info("[CL.DeliveryServer] Socket.Iocp,started");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }



        /// <summary>
        /// 停止服务
        /// </summary>
        public override void Stop()
        {
            try
            {
                Send_timer.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Stop();
        }
        #endregion





        #region Send
        /// <summary>
        /// 发送缓冲区刷新间隔（单位：毫秒,默认：1）
        /// </summary>
        public int sendFlushInterval = 1;

        VitTimer_SingleThread Send_timer = new VitTimer_SingleThread();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Send_Flush(object state)
        {
            try
            {
                foreach (var conn in connMap.Values)
                {
                    conn.FlushSendFrameQueue();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion



    }
}
