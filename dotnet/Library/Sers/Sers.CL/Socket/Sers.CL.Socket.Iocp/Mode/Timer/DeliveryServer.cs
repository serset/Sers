//  https://freshflower.iteye.com/blog/2285272 

using System;
using System.Runtime.CompilerServices;
using Sers.CL.Socket.Iocp.Base;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Socket.Iocp.Mode.Timer
{
    public class DeliveryServer : DeliveryServer_Base<DeliveryConnection>
    {


        #region Start Stop

        public override bool Start()
        {
            try
            {
                Logger.Info("[CL.DeliveryServer] Socket.Iocp,starting... host:" + host + " port:" + port);

                if (!base.Start())
                {
                    return false;
                }

                //(x.2)               
                Send_timer.intervalMs = sendInterval;
                Send_timer.timerCallback = Send_Flush;
                Send_timer.Start();

                Logger.Info("[CL.DeliveryServer] Socket.Iocp,started.");
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
        /// 单位：毫秒
        /// </summary>
        public int sendInterval = 1;

        SersTimer_SingleThread Send_timer = new SersTimer_SingleThread();


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
