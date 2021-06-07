using System;
using System.Diagnostics;
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
        /// 发送缓冲区刷新间隔（单位：毫秒,默认：1）
        /// </summary>
        public int sendFlushInterval = 1;

        LongTaskHelp Send_task = new LongTaskHelp();

 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Send_Flush()
        {
            while (true) 
            {
                try
                {
                    _conn.FlushSendFrameQueue();
 
                    //global::System.Threading.SpinWait.SpinUntil(() => false, sendFlushInterval);
                    Thread.Sleep(sendFlushInterval); 
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
