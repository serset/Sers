using System;
using System.Runtime.CompilerServices;

using Sers.CL.Socket.Iocp.Base;

using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Timer;

namespace Sers.CL.Socket.Iocp.Mode.Timer
{
    public class DeliveryClient : DeliveryClient_Base<DeliveryConnection>
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



        #region Connect Close

        public override bool Connect()
        {
            try
            {
                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connecting", new { host, port });


                if (!base.Connect())
                {
                    return false;
                }

                //(x.4)
                Send_timer.intervalMs = sendFlushInterval;
                Send_timer.timerCallback = Send_Flush;
                Send_timer.Start();


                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connected");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;


        }


        public override void Close()
        {
            try
            {
                Send_timer.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            base.Close();
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
                _conn.FlushSendFrameQueue();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion






    }
}
