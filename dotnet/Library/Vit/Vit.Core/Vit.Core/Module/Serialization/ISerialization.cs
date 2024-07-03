using System;

namespace Vit.Core.Module.Serialization
{
    public interface ISerialization
    {

        #region #1 object <--> String
        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get "null" if value is null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string Serialize(object value, Type type = null);



        /// <summary>
        /// Deserialize to T, T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get default value of T if value is null or whiteSpace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        T Deserialize<T>(string value);

        /// <summary>
        /// Deserialize to type, T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get default value of T if value is null or whiteSpace
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(string value, Type type);

        #endregion



        #region #2 object <--> bytes

        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        byte[] SerializeToBytes(object value, Type type = null);

        /// <summary>
        /// T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T DeserializeFromBytes<T>(byte[] bytes);

        /// <summary>
        /// type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object DeserializeFromBytes(byte[] bytes, Type type);

        #endregion



        #region #3 object <--> ArraySegmentByte
        T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes);
        object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type);
        #endregion



        #region #4 object <--> Span

        //T DeserializeFromSpan<T>(ReadOnlyMemory<byte> bytes);

        //object DeserializeFromSpan(ReadOnlyMemory<byte> bytes, Type type);

        #endregion

    }
}
