using System;

namespace Vit.Core.Module.Serialization
{
    public interface ISerialization
    {

        #region (x.1)object <--> String

        string SerializeToString<T>(T value);
        string SerializeToString(object value, Type type);



        T DeserializeFromString<T>(string value);
        object DeserializeFromString(string value, Type type);

        #endregion



        #region (x.2)object <--> bytes

        byte[] SerializeToBytes<T>(T obj);
        byte[] SerializeToBytes(object value, Type type);


        T DeserializeFromBytes<T>(byte[] bytes);
        object DeserializeFromBytes(byte[] bytes, Type type);

        #endregion



        #region (x.3)DeserializeFromSpan

        //T DeserializeFromSpan<T>(ReadOnlyMemory<byte> bytes);

        //object DeserializeFromSpan(ReadOnlyMemory<byte> bytes, Type type);

        #endregion



        #region (x.4)DeserializeFromArraySegmentByte

        T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes);

        object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type);

        #endregion

    }
}
