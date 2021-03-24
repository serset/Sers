using System;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Vit.Core.Module.Serialization
{
    /// <summary>
    /// 参考 https://github.com/neuecc/MessagePack-CSharp
    /// </summary>
    public class Serialization_MessagePack : Serialization
    {

        public new static readonly Serialization_MessagePack Instance = new Serialization_MessagePack();



        #region (x.2)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string SerializeToString<T>(T value)
        {
            //var bin = MessagePackSerializer.Serialize(
            // value,
            // MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return MessagePackSerializer.ConvertToJson(SerializeToBytes(value));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string SerializeToString(object value,Type type)
        {     
            return MessagePackSerializer.SerializeToJson(value);
        }

        #endregion

        #region DeserializeFromString

   

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object DeserializeFromString(string value, Type type)
        {
            throw new NotImplementedException();
            //return MessagePackSerializer.Deserialize(type, value);
        }

        #endregion

        #endregion



        #region (x.3)object <--> bytes

        #region SerializeToBytes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] SerializeToBytes<T>(T value)
        {
            return MessagePackSerializer.Serialize<T>(value,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);          
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] SerializeToBytes(object value,Type type)
        {
            return MessagePackSerializer.Serialize(type,value,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override T DeserializeFromBytes<T>(byte[] bytes)
        {   
            return MessagePackSerializer.Deserialize<T>(bytes,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object DeserializeFromBytes(byte[] bytes, Type type)
        {
            return MessagePackSerializer.Deserialize(type,bytes,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }
        #endregion

        #endregion




 


 

    }
}
