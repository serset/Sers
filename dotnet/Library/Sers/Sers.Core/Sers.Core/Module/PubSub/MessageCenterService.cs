using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;

using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;

using Vit.Core.Module.Log;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.Core.Module.PubSub
{
    public class MessageCenterService
    {
        public static readonly MessageCenterService Instance = new MessageCenterService();


        public void Conn_OnDisconnected(IOrganizeConnection conn)
        {
            //移除conn的所有订阅
            foreach (var msgTitle in subscriberMap.Keys.ToList())
            {
                SubscribeCancel(conn, msgTitle);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnGetMessage(IOrganizeConnection conn, ArraySegment<byte> messageData)
        {
            SersFile frame = new SersFile().Unpack(messageData);

            try
            {

                //byte msgType = frame.GetFile(0).AsSpan()[0];
                var file0 = frame.GetFile(0);
                byte msgType = file0.Array[file0.Offset];

                string msgTitle = frame.GetFile(1).ArraySegmentByteToString();
                switch (msgType)
                {
                    case (byte)EFrameType.publish:
                        Publish(msgTitle, frame.GetFile(2));
                        break;
                    case (byte)EFrameType.subscribe:
                        Subscribe(conn, msgTitle);
                        break;
                    case (byte)EFrameType.unSubscribe:
                        SubscribeCancel(conn, msgTitle);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }







        /// <summary>
        /// 消息订阅者   msgTitle ->    connList
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<int, IOrganizeConnection>> subscriberMap = new ConcurrentDictionary<string, ConcurrentDictionary<int, IOrganizeConnection>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Publish(string msgTitle, ArraySegment<byte> msgData)
        {
            if (!subscriberMap.TryGetValue(msgTitle, out var connList)) return;

            //message,msgTitle,msgData
            var frame = new SersFile().SetFiles(
                (new[] { (byte)EFrameType.message }).BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte(),
                 msgData
                ).Package().ToBytes();

            foreach (var conn in connList.Values)
            {
                conn.SendMessageAsync(new Vit.Core.Util.Pipelines.ByteData(frame.BytesToArraySegmentByte()));
            }
        }


        public void Subscribe(IOrganizeConnection conn, string msgTitle)
        {
            lock (this)
            {
                var connList = subscriberMap.GetOrAdd(msgTitle, (key) => new ConcurrentDictionary<int, IOrganizeConnection>());

                connList.TryAdd(conn.GetHashCode(), conn);
            }
        }

        public void SubscribeCancel(IOrganizeConnection conn, string msgTitle)
        {
            lock (this)
            {
                if (!subscriberMap.TryGetValue(msgTitle, out var connList)) return;
                connList.TryRemove(conn.GetHashCode(), out _);
                if (connList.IsEmpty)
                {
                    subscriberMap.TryRemove(msgTitle, out _);
                }
            }
        }
    }
}
