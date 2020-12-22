using System;
using System.Collections.Concurrent;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using Vit.Core.Util.Threading;
using ZeroMQ;

namespace Sers.CL.ClrZmq.ThreadWait
{
    public class SocketPoller : IDisposable
    {

        ZSocket socket;


        #region Close Dispose 析构函数


        ~SocketPoller()
        {
            Dispose();
        }


        public void Dispose()
        {
            Close(); 
        }
        public void Close()
        {
            taskToReceiveMsg.Stop();

            if (socket != null)
            {
                try
                {
                    socket.Close();
                  
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                socket = null;
            }

            if (signalForSend_puller != null)
            {
                try
                {
                    signalForSend_puller.Close();

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                signalForSend_puller = null;
            }

            if (signalForSend_pusher != null)
            {
                try
                {
                    signalForSend_pusher.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                signalForSend_pusher = null;
            }
            


        }
        #endregion



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<ZMessage> OnReceiveMessage { get; set; }

        readonly BlockingCollection<ZMessage> zMessageQueueFromMq = new BlockingCollection<ZMessage>();

        byte[] zfDataForSignal =  new byte[1];
        public void SendMessageAsync(ZMessage data)
        {            
            try
            {
                zMessageQueueFromMq.Add(data);
                lock (signalForSend_pusher)                   
                        signalForSend_pusher.Send(new ZFrame(zfDataForSignal));

                //using (data)
                //lock (signalForSend_pusher)                   
                //    signalForSend_pusher.Send(data);        

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    

        /// <summary>
        /// 初始化并开启后台线程
        /// </summary>
        /// <param name="socket"></param>
        public void Start(ZSocket socket)
        {
            Close();

            this.socket = socket;
        
      
            signalForSend_puller = new ZSocket(ZSocketType.PULL);
            signalForSend_pusher = new ZSocket(ZSocketType.PUSH);

            string inprocEndpoint = "inproc://Zmq.ClrZmq.ThreadWait_signalForSend_"+CommonHelp.NewGuid();
            signalForSend_puller.Bind(inprocEndpoint);
            signalForSend_pusher.Connect(inprocEndpoint);


            taskToReceiveMsg.threadName = "Sers.CL.ClrZmq.ThreadWait-taskToReceiveMsg";
            taskToReceiveMsg.threadCount = 1;
            taskToReceiveMsg.action = TaskToReceiveMsg;
            taskToReceiveMsg.Start();      

        }
        ZSocket signalForSend_puller;
        ZSocket signalForSend_pusher;


        #region 后台收消息线程      


        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();
       
        void TaskToReceiveMsg()
        {            
            ZError error;
            ZMessage msg;

            var sockets = new[] { socket, signalForSend_puller };
            var items = new[] { ZPollItem.CreateReceiver(), ZPollItem.CreateReceiver() };
            while (socket != null)
            {
                try
                {
                    while (socket != null)
                    {
                        if (sockets.PollIn(items, out var msgs, out error,TimeSpan.FromMilliseconds(1000)))
                        {
                            if (error != null)
                            {
                                Logger.Error("zmq.PollIn error : "+ error.ToString());                                
                            }

                            if (msgs[0] != null)
                            {
                                OnReceiveMessage(msgs[0]);
                            }
                            if (msgs[1] != null)
                            {
                                while (zMessageQueueFromMq.TryTake(out msg))
                                {
                                    using (msg)
                                        socket.Send(msg);
                                }                                                            
                            }
                        }
                    }
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
