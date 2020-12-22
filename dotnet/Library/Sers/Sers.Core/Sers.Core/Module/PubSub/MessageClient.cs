using Vit.Core.Module.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sers.Core.Module.Message;
using Vit.Extensions;
using System.Collections.Concurrent;
using Sers.Core.CL.MessageOrganize;

namespace Sers.Core.Module.PubSub
{
    public class MessageClient
    {
        public static readonly MessageClient Instance = new MessageClient();

        

        public void OnGetMessage(IOrganizeConnection conn,ArraySegment<byte> messageData)
        {
            SersFile frame = new SersFile().Unpack(messageData);

            try
            {
                //byte msgType = frame.GetFile(0).AsSpan()[0];
                var file0 = frame.GetFile(0);
                byte msgType = file0.Array[file0.Offset];

                string msgTitle;
                switch (msgType)
                {
                    case (byte)EFrameType.message:
                        msgTitle = frame.GetFile(1).ArraySegmentByteToString();
                        Message_Consumer?.Invoke(msgTitle, frame.GetFile(2));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        Action<List<ArraySegment<byte>>> _OnSendMessage;

        //消息延迟发送机制
        public Action<List<ArraySegment<byte>>> OnSendMessage
        {
            //get => _OnSendMessage;
            set
            {
                _OnSendMessage = value;
                if (null != _OnSendMessage)
                {
                    Task.Run(() =>
                    {
                        while (msgFrameToSend.TryDequeue(out var msgFrame))
                        {
                            try
                            {
                                _OnSendMessage(msgFrame);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        }
                    });
                }
            }
        }
 

        #region 延迟发送队列
        readonly ConcurrentQueue<List<ArraySegment<byte>>> msgFrameToSend = new ConcurrentQueue<List<ArraySegment<byte>>>();       
       
        void SendFrame(List<ArraySegment<byte>> frame)
        {
            if (null == _OnSendMessage)
            {
                msgFrameToSend.Enqueue(frame);           
            }
            else
            {
                _OnSendMessage(frame);
            }           
        }
        #endregion

               

        internal Action<string, ArraySegment<byte>> Message_Consumer { get; set; }
         

        public void Message_Publish(string msgTitle, ArraySegment<byte> msgData)
        {
            //publish,msgTitle,msgData 
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.publish }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte(),
                 msgData
                ).Package();
            SendFrame(frame);
        }

        public void Message_Subscribe(string msgTitle)
        {
            //subscribe,msgTitle
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.subscribe }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte()
                ).Package();
            SendFrame(frame);
        }

        public void Message_SubscribeCancel(string msgTitle)
        {
            //subscribeCancel,msgTitle
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.subscribeCancel }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte()
                ).Package();
            SendFrame(frame);
        }



        #region static 

        public static void Publish(string msgTitle, object msgBody)
        {
            Instance.Message_Publish(msgTitle, msgBody.SerializeToArraySegmentByte());
        }


        public static void Publish(string msgTitle, byte[] msgBody)
        {
            Instance.Message_Publish(msgTitle, msgBody.BytesToArraySegmentByte());
        }
        #endregion
    }
}
