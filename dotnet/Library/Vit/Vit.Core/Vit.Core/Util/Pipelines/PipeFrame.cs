using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Extensions;

namespace Vit.Core.Util.Pipelines
{
    /// <summary>
    /// 线程不安全
    /// </summary>
    public class PipeFrame
    {

        int buffLen = 0;

        //public int BuffLen => buffLen;

        Queue<ArraySegment<byte>> queueBuff = new Queue<ArraySegment<byte>>();

        int QueueBuff_dataLenOfRemoved = 0;


        public Action<ArraySegment<byte>> OnDequeueData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ArraySegment<byte> data)
        {
            //Interlocked.Add(ref buffLen, data.Count);
            buffLen += data.Count;

            queueBuff.Enqueue(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead(ArraySegment<byte> data)
        {

            int lenToPop = data.Count;
            if (lenToPop == 0) return true;

            if (buffLen < lenToPop) return false;

            var dataToPop = data.Array;
            var offset_dataToPop = data.Offset;



            //Interlocked.Add(ref buffLen, -lenToPop);
            buffLen -= lenToPop;

            int copyedIndex = 0;

            while (copyedIndex < lenToPop)
            {
                int leftCount = lenToPop - copyedIndex;

                var cur = queueBuff.Peek();
                if (QueueBuff_dataLenOfRemoved != 0)
                {
                    //cur = new ArraySegment<byte>(cur.Array, cur.Offset + QueueBuff_dataLenOfRemoved, cur.Count - QueueBuff_dataLenOfRemoved);
                    cur = cur.Slice(QueueBuff_dataLenOfRemoved);
                }

                if (cur.Count <= leftCount)
                {
                    //dataToPop 数据长
                    Array.Copy(cur.Array, cur.Offset, dataToPop, copyedIndex + offset_dataToPop, cur.Count);
                    copyedIndex += cur.Count;
                    QueueBuff_dataLenOfRemoved = 0;

                    var item = queueBuff.Dequeue();
                    OnDequeueData?.Invoke(item);
                }
                else
                {
                    //queueBuff 数据长
                    Array.Copy(cur.Array, cur.Offset, dataToPop, copyedIndex + offset_dataToPop, leftCount);
                    copyedIndex += leftCount;
                    QueueBuff_dataLenOfRemoved += leftCount;
                }
            }
            return true;
        }


        #region TryRead_SersFile
        ArraySegment<byte> fileLen_bytes = new ArraySegment<byte>(new byte[4]);
        int fileLen = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead_SersFile(out ArraySegment<byte> data)
        {
            if (fileLen < 0)
            {
                if (!TryRead(fileLen_bytes))
                {
                    return false;
                }
                fileLen = fileLen_bytes.Array.BytesToInt32();
            }

            if (buffLen < fileLen) return false;

            data = new ArraySegment<byte>(new byte[fileLen]);
            fileLen = -1;
            TryRead(data);
            return true;
        }
        #endregion
    }

}
