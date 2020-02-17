using Sers.Core.Module.Log;
using Sers.Core.Util.Threading;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace Sers.Mq.Zmq.ClrZmq.ThreadWait
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

            taskToReceiveMsg.Stop();
        }
        #endregion



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<ZMessage> OnReceiveMessage { get; set; }

        readonly ConcurrentQueue<ZMessage> messageToSend = new ConcurrentQueue<ZMessage>();
        //readonly BlockingCollection<ZMessage> messageToSend = new BlockingCollection<ZMessage>();

        public void SendMessageAsync(ZMessage data)
        {
            messageToSend.Enqueue(data);
            //messageToSend.Add(data);

 

        }
    

        /// <summary>
        /// 初始化并开启后台线程
        /// </summary>
        /// <param name="socket"></param>
        public void Start(ZSocket socket)
        {
            this.socket = socket;

            taskToReceiveMsg.Stop();

            taskToReceiveMsg.threadName = "Sers.Mq.Socket.ThreadWait-taskToReceiveMsg";
            taskToReceiveMsg.threadCount = 1;
            taskToReceiveMsg.action = TaskToReceiveMsg;
            taskToReceiveMsg.Start(); 
        }



        #region 后台收消息线程      
 

        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();
       
        void TaskToReceiveMsg()
        {
            ZPollItem pollInItem = ZPollItem.CreateReceiver();
            ZError error;

            var poollForSender = ZPollItem.CreateSender();

            
 

            while (socket != null)
            {
                try
                {
                    while (socket != null)
                    {

                         

                        //var msg=socket.ReceiveMessage();
                        //OnReceiveMessage(msg);



                        //TODO:不合理,需优化

                        // PollOut
                        while (true)
                        {
                            if (!messageToSend.TryDequeue(out var msgToSend)) break;

                            using (msgToSend)
                            {
                                socket.PollOut(poollForSender, msgToSend, out error, TimeSpan.Zero);
                            }
                        }
                    


                        // PollIn      


                        if (socket.PollIn(pollInItem, out var msg, out error, TimeSpan.Zero))
                        {
                            if (msg != null)
                                OnReceiveMessage(msg);
                        }
                    
 
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion

 



    }
}
