using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Extensions
{
    public static partial class ByteDataExtensions
    {

        #region ByteDataCount

        
        public static int ByteDataCount(this List<ArraySegment<byte>> byteData)
        {
            if (null == byteData || byteData.Count == 0) return 0;
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


        #region ByteData <--> Bytes

        public static byte[] ByteDataToBytes(this List<ArraySegment<byte>> byteData)
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
                     
                item.CopyTo(bytes, curIndex);

                curIndex += item.Count;
            }
            return bytes;
        }


        #endregion

        #region ByteData <--> ArraySegment

        public static ArraySegment<byte> ByteDataToArraySegment(this List<ArraySegment<byte>> byteData)
        {
            var bytes = byteData?.ByteDataToBytes();
            return bytes==null? ArraySegmentByteExtensions.Null:new ArraySegment<byte>(bytes);
        }


        #endregion


        #region ByteData <--> String

        public static string ByteDataToString(this List<ArraySegment<byte>> data)
        {
            return data.ByteDataToBytes().BytesToString();
        }


        public static List<ArraySegment<byte>> StringToByteData(this string data)
        {
            return new List<ArraySegment<byte>> { data.StringToArraySegmentByte() };
        }
        #endregion



        #region ByteDataPopInt32

        /// <summary>
        /// data的长度 必须大于等于4
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ByteDataPopInt32(this List<ArraySegment<byte>> data)
        {
            return data.ByteDataPopBytes(new byte[4]).BytesToInt32();
        }
        #endregion



        #region ByteDataPopByteData

        /// <summary>
        /// data 的长度 必须大于等于 dataToPop 的长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataToPop"></param>
        /// <returns></returns>
        public static byte[] ByteDataPopBytes(this List<ArraySegment<byte>> data, byte[] dataToPop)
        {
            int lenToPop = dataToPop.Length;
          

            int copyedIndex = 0;
         
            while (copyedIndex < lenToPop)
            {
                int leftCount = lenToPop - copyedIndex;

                var cur = data[0];
                if (cur.Count <= leftCount)
                {
                    Array.Copy(cur.Array, cur.Offset, dataToPop, copyedIndex, cur.Count);
                    copyedIndex += cur.Count;
                    data.RemoveAt(0);                 
                }
                else
                {
                   
                    Array.Copy(cur.Array, cur.Offset, dataToPop, copyedIndex, leftCount);
                    copyedIndex += leftCount;
                    data[0] = cur.Slice(leftCount);
                }
            }
            return dataToPop;
        }
        #endregion



        #region ByteDataPopByteData

        /// <summary>
        /// data 的长度 必须大于等于lenToPop
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lenToPop"></param>
        /// <returns></returns>
        public static List<ArraySegment<byte>> ByteDataPopByteData(this List<ArraySegment<byte>> data, int lenToPop)
        {
            List<ArraySegment<byte>> dataToPop = new List<ArraySegment<byte>>();

            int leftCount = lenToPop;
            while (leftCount > 0)
            {
                var cur = data[0];
                if (cur.Count <= leftCount)
                {
                    leftCount -= cur.Count;
                    data.RemoveAt(0);
                    dataToPop.Add(cur);
                }
                else
                {
                    dataToPop.Add(cur.Slice(0, leftCount));
                    data[0] = cur.Slice(leftCount);
                    leftCount = 0;
                }
            }
            return dataToPop;
        }
        #endregion
    }
}
