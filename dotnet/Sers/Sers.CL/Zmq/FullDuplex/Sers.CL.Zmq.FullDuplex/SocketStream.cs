using Sers.CL.Zmq.FullDuplex.Zmq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Zmq.FullDuplex
{
    public class SocketStream
    {
        ZSocket socketReader;
        ZSocket socketWriter;

 

        public Action<List<byte[]>> OnReceiveMessage;

        public Action BeforeStop { get; set; }
        public Action AfterStop { get; set; }

        bool isRunning = false;

        public void Start(ZSocket socketReader, ZSocket socketWriter)
        {
            if (isRunning) Stop();

            this.socketReader = socketReader;
            this.socketWriter = socketWriter;


            isRunning = true;
            StartBackThreadToReceiveMsg();
            StartBackThreadToSendMsg();
        }

        public void Stop()
        {
            if (!isRunning) return;
            isRunning = false;

            try
            {
                BeforeStop?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            try
            {
                taskToReceiveMsg.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                socketReader.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            try
            {
                taskToSendMsg.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                socketWriter.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                AfterStop?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
        }


        #region taskToReceiveMsg

        LongTaskHelp taskToReceiveMsg = new LongTaskHelp();
        void StartBackThreadToReceiveMsg()
        {
            taskToReceiveMsg.Stop();

            taskToReceiveMsg.threadName = "Sers.CL.Zmq.FullDuplex-taskToReceiveMsg";
            taskToReceiveMsg.threadCount = 1;
            taskToReceiveMsg.action = TaskToReceiveMsg;
            taskToReceiveMsg.Start();
        }

        void TaskToReceiveMsg()
        {
            //while (socketReader != null)
            //{
            try
            {
                while (socketReader != null)
                {
                    OnReceiveMessage(socketReader.ReceiveMessage());
                }
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                Logger.Error(ex);
                Stop();
            }
            //}
        }
        #endregion



        #region taskToSendMsg
        readonly BlockingCollection<byte[][]> msgQueueToSend = new BlockingCollection<byte[][]>();
        public void SendMessageAsync(params byte[][] msg)
        {
            msgQueueToSend.Add(msg);
        }


        LongTaskHelp taskToSendMsg = new LongTaskHelp();
        void StartBackThreadToSendMsg()
        {
            taskToSendMsg.Stop();

            taskToSendMsg.threadName = "Sers.CL.Zmq.FullDuplex-taskToSendMsg";
            taskToSendMsg.threadCount = 1;
            taskToSendMsg.action = TaskToSendMsg;
            taskToSendMsg.Start();
        }

        void TaskToSendMsg()
        {
            //while (socketWriter != null)
            //{
            try
            {
                while (true)
                {
                    socketWriter.SendMessage(msgQueueToSend.Take());
                }
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                Logger.Error(ex);
                Stop();
            }
            //}
        }
        #endregion

    }
}
