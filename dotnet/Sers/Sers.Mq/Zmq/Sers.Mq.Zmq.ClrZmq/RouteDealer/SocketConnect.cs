using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Module.Log;
using Sers.Core.Util.Threading;
using ZeroMQ;

namespace Sers.Mq.Zmq.ClrZmq.RouteDealer
{
    class SocketConnect:IDisposable
    {

        public const string MqVersion = "Sers.Mq.Zmq.V1";

 

        public bool IsConnected => socketConnect != null;



        /// <summary>
        /// 初始化并开启后台线程
        /// </summary>
        /// <param name="socketConnect"></param>
        /// <param name="workThreadCount">后台处理消息的线程个数</param>
        public void Init(ZSocket socketConnect, int workThreadCount)
        {
            this.socketConnect= socketConnect;

            taskBehind.Init(socketConnect, workThreadCount);
            taskBehind.Start();
        } 


 


        ZSocket socketConnect;


        #region Close Dispose 析构函数


        ~SocketConnect()
        {
            Dispose();
        }


        public void Dispose()
        {
            Close();

            taskBehind?.Dispose();
        }
        public void Close()
        {       
            if(!IsConnected) return;         


            try
            {
                if (null != socketConnect)
                {
                    socketConnect.Close();
                    socketConnect.Dispose();
                    socketConnect = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            try
            {
                taskBehind.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        #endregion


        #region 收到消息 和 发送消息

        public Action<ZMessage> OnGetMsg
        {
            set => taskBehind.OnGetMsg = value;
            get=> taskBehind.OnGetMsg ;
        }
 

        public void SendMsg(ZMessage data)
        {
            taskBehind.SendMsg(data);
        }
        #endregion


        #region 后台收发消息线程
        readonly  TaskBehind taskBehind = new TaskBehind();

        #region TaskBehind

        class TaskBehind  :IDisposable
        {

            LongTaskHelp thread_zmq_poll = new LongTaskHelp();
            LongTaskHelp thread_DealMsg = new LongTaskHelp();
            public TaskBehind()
            {
                thread_zmq_poll.action = Thread_Zmq_Poll;

                thread_DealMsg.action = ThreadToDealMsg;                 
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="socketConnect"></param>
            /// <param name="workThreadCount">后台处理消息的线程个数</param>
            public void Init(ZSocket socketConnect, int workThreadCount)
            {
                this.socketConnect = socketConnect;
                thread_DealMsg.threadCount = workThreadCount;
            }


            public void Start()
            {
                thread_zmq_poll.Start();
                thread_DealMsg.Start();
            }
            public void Stop()
            {
                thread_zmq_poll.Stop();
                thread_DealMsg.Stop();
            }
            public void Dispose()
            {
                thread_zmq_poll.Dispose();
                thread_DealMsg.Dispose();

            }




            public Action<ZMessage> OnGetMsg;


            #region MsgToDeal

         
            ConcurrentQueue<ZMessage> msgToDealQueue = new ConcurrentQueue<ZMessage>();
            AutoResetEvent autoResetEvent_OnReveiveMsg = new AutoResetEvent(false);


            void ThreadToDealMsg()
            {
                while (true)
                {
                    try
                    {
                        #region ThreadToDealMsg                        
                        while (true)
                        {

                            if (msgToDealQueue.TryDequeue(out var msg))
                            {
                                using (msg)
                                {
                                    OnGetMsg(msg);
                                }
                            }
                            else
                            {
                                autoResetEvent_OnReveiveMsg.WaitOne(100);
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex) when (!(ex is ThreadInterruptedException))
                    {
                    }
                }
            }

            #region CahceReceivedMsg
            void CahceReceivedMsg(ZMessage msg)
            {
                msgToDealQueue.Enqueue(msg);
                autoResetEvent_OnReveiveMsg.Set(); 
            }
            #endregion
            #endregion
           


            #region MsgToSend
            bool hasMsgToSend = false;
            ConcurrentQueue<ZMessage> msgToSendQueue = new ConcurrentQueue<ZMessage>();
            public void SendMsg(ZMessage data)
            {
                msgToSendQueue.Enqueue(data);
                hasMsgToSend = true;
            }

            ZMessage GetMsgToSend()
            {
                if (msgToSendQueue.TryDequeue(out var data))
                {
                    if (msgToSendQueue.IsEmpty) hasMsgToSend = false;
                    return data;
                }
                return null;
            }
            #endregion



            #region zmq           

            ZSocket socketConnect;

            void Thread_Zmq_Poll()
            {

                var pollInItem = ZPollItem.CreateReceiver();

                bool flag;
                ZError error;
                ZMessage message;

                while (true)
                {
                    try
                    {
                        while (true)
                        {

                            //TODO:不合理,需优化

                            #region PollOut
                            if (hasMsgToSend)
                            {
                                message = GetMsgToSend();
                                if (null != message)
                                {
                                    using (message)
                                    {
                                        flag = socketConnect.PollOut(ZPollItem.CreateSender(), message, out error, TimeSpan.Zero);
                                    }

                                }
                            }
                            #endregion


                            #region PollIn      

                            if (socketConnect.PollIn(pollInItem, out message, out error, TimeSpan.Zero))
                            {
                                CahceReceivedMsg(message);                             
                            }
                            #endregion

                           
                        }
                    }
                    catch(Exception ex) when (!(ex is ThreadInterruptedException))
                    {
                    }
                }              
            }

           

            #endregion


        }
        #endregion


        #endregion



    }
}
