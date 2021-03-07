using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vit.Extensions;

namespace Vit.Core.Util.Pipelines
{
    /// <summary>
    /// 
    /// .net core中的高效动态内存管理方案
    /// https://www.cnblogs.com/TianFang/p/10084049.html
    /// 
    /// .net core中的System.Buffers名字空间
    /// https://www.cnblogs.com/TianFang/p/9193881.html
    /// 
    /// 
    /// Span＜T＞ —— .NET Core 高效运行的新基石
    /// https://blog.csdn.net/wnvalentin/article/details/93485572
    /// 
    /// 
    /// </summary>
    public class ByteData : IEnumerable<ArraySegment<byte>>
    {

        public readonly List<ArraySegment<byte>> byteData;

        public ByteData()
        {
            byteData = new List<ArraySegment<byte>>();
        }

        public ByteData(int capacity)
        {
            byteData = new List<ArraySegment<byte>>(capacity);
        }

        public ByteData(List<ArraySegment<byte>> byteData)
        {
            this.byteData = byteData;
        }


        #region implicit
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ByteData(List<ArraySegment<byte>> byteData)
        {
            return new ByteData(byteData);
        }
        #endregion






        #region List




        #region Add
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteData Add(ArraySegment<byte> data)
        {
            byteData.Add(data);
            return this;
        }
        #endregion



        #region []    

        public ArraySegment<byte> this[int index]
        {
            get
            {
                return byteData[index];
            }

            set
            {
                byteData[index] = value;
            }
        }
        #endregion



        #region AddRange
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<ArraySegment<byte>> collection)
        {
            byteData.AddRange(collection);
        }
        #endregion



        #region Insert
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, ArraySegment<byte> item)
        {
            byteData.Insert(index, item);
        }
        #endregion



        #region IEnumerable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<ArraySegment<byte>> GetEnumerator()
        {
            return byteData.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return byteData.GetEnumerator();
        }
        #endregion



        #endregion


               



        #region Count

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            if (byteData.Count == 0) return 0;
            int count = 0;

            foreach (var item in byteData)
            {
                if (null != item)
                {
                    count += item.Count;
                }
            }
            return count;
        }
        #endregion




        #region ToBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToBytes()
        {
            int count = 0;
            foreach (var item in byteData)
            {
                count += item.Count;
            }

            var bytes = new byte[count];

            int curIndex = 0;
            foreach (var item in byteData)
            {
                if (null == item.Array || item.Count == 0) continue;

                //item.CopyTo(bytes, curIndex);
                //Buffer.BlockCopy(item.Array, item.Offset, bytes, curIndex, item.Count);

                unsafe
                {
                    fixed (byte* pSource = item.Array, pTarget = bytes)
                    {
                        Buffer.MemoryCopy(pSource + item.Offset, pTarget + curIndex, item.Count, item.Count);
                    }
                }

                curIndex += item.Count;
            }
            return bytes;
        }
        #endregion



        #region ToArraySegment

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(ToBytes());
        }

        #endregion


        #region ByteDataToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ByteDataToString()
        {
            return ToBytes().BytesToString();
        }
        #endregion
    }
}
