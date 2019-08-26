using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sers.Core.Module.Message;
using Sers.Core.Extensions;

namespace Sers.Core.Module.PubSub.ShareEndpoint.Sys
{
    public class PubSubClient
    {
        public static readonly PubSubClient Instance = new PubSubClient();
        #region mq

        public void OnReceiveMessage(ArraySegment<byte> messageData)
        {
            DealMessageFrame(new SersFile().Unpack(messageData));
        }
        Action<List<ArraySegment<byte>>> _OnSendMessage;

        //消息延迟发送机制
        public Action<List<ArraySegment<byte>>> OnSendMessage
        {
            get => _OnSendMessage;
            set
            {
                _OnSendMessage = value;
                if (null != _OnSendMessage)
                {
                    List<List<ArraySegment<byte>>> list=null;
                    lock (this)
                    {
                        if (null != listToSend)
                        {
                            list = listToSend;
                            listToSend = null;
                        }                       
                    }
                    if (null != list)
                    {
                        Task.Run(()=> 
                        {
                            foreach (var frame in list)
                            {
                                try
                                {
                                    _OnSendMessage(frame);
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
        }
        #endregion

        #region 延迟发送队列
        List<List<ArraySegment<byte>>> listToSend;
        void AddFrameToQueue(List<ArraySegment<byte>> frame)
        {
            lock (this)
            {
                if (null == listToSend) listToSend = new List<List<ArraySegment<byte>>>();
                listToSend.Add(frame);
            }
        }

       
        void SendFrame(List<ArraySegment<byte>> frame)
        {
            if (null == _OnSendMessage)
            {
                AddFrameToQueue(frame);
            }
            else
            {
                _OnSendMessage(frame);
            }
           
        }
        #endregion


        void DealMessageFrame(SersFile frame)
        {
            try
            {
                byte msgType = frame.GetFile(0).AsSpan()[0];
                string msgTitle;
                switch (msgType)
                {
                    case (byte)EFrameType.message:
                        msgTitle = Serialization.Serialization.Instance.Deserialize<string>(frame.GetFile(1));                 
                        Message_Consume?.Invoke(msgTitle, frame.GetFile(2));
                        break;                   
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public Action<string, ArraySegment<byte>> Message_Consume { get; set; }
         

        public void Message_Publish(string msgTitle, ArraySegment<byte> msgData)
        {
            //publish,msgTitle,msgData 
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.publish }.BytesToArraySegmentByte(),
                 Serialization.Serialization.Instance.Serialize(msgTitle).BytesToArraySegmentByte(),
                 msgData
                ).Package();
            SendFrame(frame);
        }

        public void Message_Subscribe(string msgTitle)
        {
            //subscribe,msgTitle
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.subscribe }.BytesToArraySegmentByte(),
                 Serialization.Serialization.Instance.Serialize(msgTitle).BytesToArraySegmentByte()
                ).Package();
            SendFrame(frame);
        }

        public void Message_SubscribeCancel(string msgTitle)
        {
            //subscribeCancel,msgTitle
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.subscribeCancel }.BytesToArraySegmentByte(),
                 Serialization.Serialization.Instance.Serialize(msgTitle).BytesToArraySegmentByte()
                ).Package();
            SendFrame(frame);
        }
    }
}
