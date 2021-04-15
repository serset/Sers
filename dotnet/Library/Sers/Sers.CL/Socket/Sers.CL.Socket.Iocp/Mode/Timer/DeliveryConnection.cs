using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Sers.CL.Socket.Iocp.Base;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Extensions;

namespace Sers.CL.Socket.Iocp.Mode.Timer
{
    public class DeliveryConnection : DeliveryConnection_Base
    {

        #region Send

        ConcurrentQueue<ByteData> frameQueueToSend = new ConcurrentQueue<ByteData>();

        const int buffLength = 1024;
        ByteData[] buffer = new ByteData[buffLength];
        int[] bufferItemCount = new int[buffLength];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            frameQueueToSend.Enqueue(data);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlushSendFrameQueue()
        {
            int curIndex;
             
            try
            {
                while (true)
                {
                    curIndex = 0;
                    while (true)
                    {
                        if (frameQueueToSend.TryDequeue(out var item))
                        {
                            buffer[curIndex++] = item;

                            if (curIndex == buffLength)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (curIndex == 0) return;
                            break;
                        }
                    }

                    FlushData(curIndex);

                    if (curIndex < buffLength)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopIndex">不包含</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void FlushData(int stopIndex)
        {      
            int curCount;
            ByteData byteData;
            byte[] bytes;

            int sumCount = 0;
            int startIndex = 0;
            int curIndex = 0;
            while (true)
            {
                byteData = buffer[curIndex];

                //(x.1)get count
                curCount = 0;
                foreach (var item in byteData.byteArrayList)
                {
                    curCount += item.Count;
                }
                bufferItemCount[curIndex] = curCount;
                sumCount += curCount;

                curIndex++;


                //(x.2)
                if (curIndex == stopIndex)
                {
                    bytes = BufferToBytes(startIndex, curIndex, sumCount);
                    socket.SendAsync(bytes.BytesToArraySegmentByte(), SocketFlags.None);
                    return;
                }


                //(x.3)
                if (sumCount >= 1_000_000)
                {
                    bytes = BufferToBytes(startIndex, curIndex, sumCount);
                    socket.SendAsync(bytes.BytesToArraySegmentByte(), SocketFlags.None);

                    sumCount = 0;
                    startIndex = curIndex;
                }           
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="stopIndex">不包含</param>
        /// <param name="sumCount"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe byte[] BufferToBytes(int startIndex, int stopIndex,int sumCount)
        {
            var bytes = new byte[sumCount + (stopIndex - startIndex) * 4];

            int curLength;

            fixed (byte* pTarget = bytes)
            {
                int dataIndex = 0;
                for (int curIndex = startIndex; curIndex < stopIndex; curIndex++)
                {
                    var byteData = buffer[curIndex];

                    ((int*)(pTarget + dataIndex))[0] = curLength = bufferItemCount[curIndex];
                    dataIndex += 4;

                    foreach (var item in byteData.byteArrayList)
                    {
                        if (null == item.Array || item.Count == 0) continue;
                        fixed (byte* pSource = item.Array)
                        {
                            Buffer.MemoryCopy(pSource + item.Offset, pTarget + dataIndex, item.Count, item.Count);
                        }
                        dataIndex += item.Count;
                    }
                    _securityManager?.Encryption(new ArraySegment<byte>(bytes, dataIndex - curLength, curLength));
                }
            }
            return bytes;
        }

        #endregion

    }
}
