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
                    var bytes = ByteDataArrayToBytes(buffer, curIndex);
                    try
                    {
                        socket.SendAsync(bytes.BytesToArraySegmentByte(), SocketFlags.None);
                        //socket.SendAsync(data, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Close();
                    }

                    if (curIndex < buffLength)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe byte[] ByteDataArrayToBytes(ByteData[] byteDataArray, int arrayCount)
        {
            int sumCount = 0;
            int curCount;
            int arrayIndex;

            //(x.1)get count
            for (arrayIndex = 0; arrayIndex < arrayCount; arrayIndex++)
            {
                var byteData = byteDataArray[arrayIndex];
                curCount = 0;
                foreach (var item in byteData.byteArrayList)
                {
                    curCount += item.Count;
                }
                bufferItemCount[arrayIndex] = curCount;
                sumCount += curCount;
            }


            //(x.2)copy data
            var bytes = new byte[sumCount + arrayCount * 4];
            arrayIndex = 0;
            curCount = 0;

            int curLength;

            fixed (byte* pTarget = bytes)
            {
                for (arrayIndex = 0; arrayIndex < arrayCount; arrayIndex++)
                {
                    var byteData = byteDataArray[arrayIndex];
                    ((int*)(pTarget + curCount))[0] = curLength = bufferItemCount[arrayIndex];
                    curCount += 4;

                    foreach (var item in byteData.byteArrayList)
                    {
                        if (null == item.Array || item.Count == 0) continue;
                        fixed (byte* pSource = item.Array)
                        {
                            Buffer.MemoryCopy(pSource + item.Offset, pTarget + curCount, item.Count, item.Count);
                        }
                        curCount += item.Count;
                    }
                    _securityManager?.Encryption(new ArraySegment<byte>(bytes, curCount- curLength, curLength));
                    
                }
            }
            return bytes;
        }

        #endregion

    }
}
