using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;

using Vit.Core.Module.Log;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Object_Serialize_Extensions;

namespace Sers.Core.Module.PubSub
{
    public class MessageClient
    {
        public static readonly MessageClient Instance = new MessageClient();



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnGetMessage(IOrganizeConnection conn, ArraySegment<byte> messageData)
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

        Action<Vit.Core.Util.Pipelines.ByteData> _OnSendMessage;

        //消息延迟发送机制
        public Action<Vit.Core.Util.Pipelines.ByteData> OnSendMessage
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
        readonly ConcurrentQueue<Vit.Core.Util.Pipelines.ByteData> msgFrameToSend = new ConcurrentQueue<Vit.Core.Util.Pipelines.ByteData>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SendFrame(Vit.Core.Util.Pipelines.ByteData frame)
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Message_Publish(string msgTitle, ArraySegment<byte> msgData)
        {
            //EFrameType.publish, msgTitle, msgData 
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.publish }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte(),
                 msgData
                ).Package();
            SendFrame(frame);
        }

        public void Message_Subscribe(string msgTitle)
        {
            //EFrameType.subscribe, msgTitle
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.subscribe }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte()
                ).Package();
            SendFrame(frame);
        }

        public void Message_UnSubscribe(string msgTitle)
        {
            //EFrameType.unSubscribe, msgTitle
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.unSubscribe }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte()
                ).Package();
            SendFrame(frame);
        }



        #region static 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish(string msgTitle, object msgBody)
        {
            Instance.Message_Publish(msgTitle, msgBody.SerializeToArraySegmentByte());
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish(string msgTitle, byte[] msgBody)
        {
            Instance.Message_Publish(msgTitle, msgBody.BytesToArraySegmentByte());
        }
        #endregion
    }
}
