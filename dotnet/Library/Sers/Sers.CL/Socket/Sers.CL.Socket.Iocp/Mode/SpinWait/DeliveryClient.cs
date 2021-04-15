using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Sers.CL.Socket.Iocp.Base;
using Sers.CL.Socket.Iocp.Mode.Timer;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Socket.Iocp.Mode.SpinWait
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
                Send_task.threadCount = 1;
                Send_task.action = Send_Flush;
                Send_task.Start();


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
                Send_task.Stop();
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

        LongTaskHelp Send_task = new LongTaskHelp();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Send_Flush()
        {
            while (true) 
            {
                try
                {
                    _conn.FlushSendFrameQueue();
                    global::System.Threading.SpinWait.SpinUntil(() => false, sendInterval);
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }           
        }

        #endregion






    }
}
