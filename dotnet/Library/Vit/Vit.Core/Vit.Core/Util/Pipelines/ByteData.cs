using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Vit.Extensions.Serialize_Extensions;

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
    /// ReadOnlySequenceSegment .net core中的高效动态内存管理方案
    /// https://www.cnblogs.com/TianFang/p/10084049.html
    /// 
    /// </summary>
    public class ByteData /*: IEnumerable<ArraySegment<byte>>*/
    {

        public readonly List<ArraySegment<byte>> byteArrayList;

        //~ByteData()
        //{
        //    //byteData.Clear();      
        //    //ObjectPool<List<ArraySegment<byte>>>.Shared.Push(byteData);
        //}


        public ByteData()
        {
            byteArrayList = new List<ArraySegment<byte>>();
            //byteArrayList = ObjectPool<List<ArraySegment<byte>>>.Shared.Pop();
        }

        //public ByteData(int capacity)
        //{
        //    byteArrayList = new List<ArraySegment<byte>>(capacity);
        //}

        public ByteData(ArraySegment<byte> data) : this()
        {
            byteArrayList.Add(data);
        }

        public ByteData(byte[] bytes) : this(bytes.BytesToArraySegmentByte())
        {
        }
        //public ByteData(List<ArraySegment<byte>> byteArrayList)
        //{
        //    this.byteArrayList = byteArrayList;
        //}


        #region //implicit
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static implicit operator ByteData(List<ArraySegment<byte>> byteArrayList)
        //{
        //    return new ByteData(byteArrayList);
        //}
        #endregion






        #region List



        #region Add
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteData Add(ArraySegment<byte> data)
        {
            byteArrayList.Add(data);
            return this;
        }
        #endregion 



        #region AddRange
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<ArraySegment<byte>> collection)
        {
            byteArrayList.AddRange(collection);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(ByteData byteData)
        {
            this.byteArrayList.AddRange(byteData.byteArrayList);
        }
        #endregion



        #region Insert
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, ArraySegment<byte> item)
        {
            byteArrayList.Insert(index, item);
        }
        #endregion




        #region //[]
        //public ArraySegment<byte> this[int index]
        //{
        //    get
        //    {
        //        return byteData[index];
        //    }

        //    set
        //    {
        //        byteData[index] = value;
        //    }
        //}
        #endregion



        #region //IEnumerable

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public IEnumerator<ArraySegment<byte>> GetEnumerator()
        //{
        //    return byteData.GetEnumerator();
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return byteData.GetEnumerator();
        //}
        #endregion



        #endregion





        #region Count

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            if (byteArrayList.Count == 0) return 0;
            int count = 0;

            foreach (var item in byteArrayList)
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
            return byteArrayList.ByteDataToBytes();
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
