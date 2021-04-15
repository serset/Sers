using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Sers.CL.Socket.Iocp.Base;
using Sers.CL.Socket.Iocp.Mode.Timer;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Socket.Iocp.Mode.SpinWait
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
                Send_task.threadCount = 1;
                Send_task.action = Send_Flush;
                Send_task.Start();


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
                Send_task.Stop();
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

        LongTaskHelp Send_task = new LongTaskHelp();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Send_Flush()
        {
            while (true)
            {
                try
                {
                    foreach (var conn in connMap.Values)
                    {
                        conn.FlushSendFrameQueue();
                    }
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
