using Newtonsoft.Json;
using System;

namespace Sers.Core.Extensions
{
    public static partial class ObjectSerializeExtensions
    {


        #region SerializeToBytes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SerializeToBytes(this Object value)
        {
            return Sers.Core.Module.Serialization.Serialization.Instance.Serialize(value);
        }
        #endregion

        #region DeserializeFromBytes

        public static T DeserializeFromBytes<T>(this ArraySegment<byte> value)
        {
            return Sers.Core.Module.Serialization.Serialization.Instance.Deserialize<T>(value);
        }
        #endregion
        
    }
}
