using System;

namespace Vit.Core.Module.Serialization
{
    public interface ISerialization
    {

        #region (x.1)object <--> String

        /// <summary>
        /// T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        string Serialize<T>(T value);

        /// <summary>
        /// T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string Serialize(object value, Type type);



        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        T Deserialize<T>(string value);

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(string value, Type type);

        #endregion



        #region (x.2)object <--> bytes

        /// <summary>
        /// T 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] SerializeToBytes<T>(T obj);

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        byte[] SerializeToBytes(object value, Type type);

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T DeserializeFromBytes<T>(byte[] bytes);

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object DeserializeFromBytes(byte[] bytes, Type type);

        #endregion



        #region (x.3)object <--> Span

        //T DeserializeFromSpan<T>(ReadOnlyMemory<byte> bytes);

        //object DeserializeFromSpan(ReadOnlyMemory<byte> bytes, Type type);

        #endregion



        #region (x.4)object <--> ArraySegmentByte

        T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes);

        object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type);

        #endregion

    }
}
