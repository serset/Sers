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

        readonly BlockingCollection<ZMessage> mqMessageQueueFromMq = new BlockingCollection<ZMessage>();

        public void SendMessageAsync(ZMessage data)
        {
            //mqMessageQueueFromMq.Add(data);
       
            try
            {

                //todo lock?
                lock (socket)
                    socket.Send(data);

                    //lock (socket)
                    //    if (!socket.Send(data, ZSocketFlags.DontWait, out var error))
                    //{
                    //    //if (error == ZError.EAGAIN)
                    //    //{
                    //    //    //Console.WriteLine(new ZException(error));


                    //    //    /* Usually when reaching EAGAIN, I would do
                    //    //    Thread.Sleep(1);
                    //    //    continue; /**/
                    //    //}
                    //    //if (error == ZError.ETERM)
                    //    //    return; // Interrupted
                    //    throw new ZException(error);
                    //}

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            //socket.SendMessage(data);
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

            //Task.Run(()=>{
            //    while (this.socket!=null)
            //    {
            //        try
            //        {
            //            using (var msg= mqMessageQueueFromMq.Take())
            //                socket.SendMessage(msg);
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.Error(ex);
            //        }

            //    }


            //});
        }



        #region 后台收消息线程      
     

        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();
       
        void TaskToReceiveMsg()
        {
            ZPollItem pollInItem = ZPollItem.CreateReceiver();
            ZError error;

            while (socket != null)
            {
                try
                {
                    while (socket != null)
                    {
                        var msg = socket.ReceiveMessage();
                        OnReceiveMessage(msg);

                        //if (socket.PollIn(pollInItem, out var msg, out error))
                        //{
                        //    if (msg != null)
                        //        OnReceiveMessage(msg);
                        //}


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
