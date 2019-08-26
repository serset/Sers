using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Extensions
{
    public static partial class ByteDataExtensions
    {

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


    }
}
