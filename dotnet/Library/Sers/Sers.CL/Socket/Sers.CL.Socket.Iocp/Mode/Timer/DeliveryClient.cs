using System;
using System.Runtime.CompilerServices;
using Sers.CL.Socket.Iocp.Base;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Socket.Iocp.Mode.Timer
{
    public class DeliveryClient : DeliveryClient_Base<DeliveryConnection>
    {

        #region Connect Close

        public override bool Connect()
        {
            try
            {
                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connecting... host:" + host + " port:" + port);


                if (!base.Connect())
                {
                    return false;
                }

                //(x.4)
                Send_timer.intervalMs = sendInterval;
                Send_timer.timerCallback = Send_Flush;
                Send_timer.Start();


                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connected.");
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
        /// 单位：毫秒
        /// </summary>
        public int sendInterval = 1;

        SersTimer_SingleThread Send_timer = new SersTimer_SingleThread();


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
