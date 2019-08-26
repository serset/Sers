using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace Sers.Core.Module.Api.ApiDesc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SsModel
    {       


        public Func<ArraySegment<byte>, object[]> OnDeserialize;

        public object[] Deserialize(ArraySegment<byte> bytes)
        {
            return OnDeserialize(bytes);
        }






        #region JsonProperty




        /// <summary>
        /// 数据类型。 可为 object、string、int32、int64、float、double、bool、datetime 或 SsModelEntity的name
        /// </summary>
        [JsonProperty]
        [SsExample("string")]
        [SsDescription("数据类型。 可为 object、string、int32、int64、float、double、bool、datetime 或 SsModelEntity的name")]
        public string type;

        /// <summary>
        ///  数据模式。只可为 value、object、array
        /// </summary>
        [JsonProperty]
        [SsExample("value")]
        [SsDescription("数据模式。只可为 value、object、array")]
        public string mode;

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty]
        [SsExample("")]
        [SsDescription("描述")]
        public string description;

        /// <summary>
        /// 默认值
        /// </summary>
        [JsonProperty]
        [SsExample("")]
        [SsDescription("默认值")]
        public object defaultValue;

        /// <summary>
        /// 示例值
        /// </summary>
        [JsonProperty]
        [SsExample("")]
        [SsDescription("示例值")]
        public object example;


        [JsonProperty] 
        [SsDescription("实体")]
        public List<SsModelEntity> models;

        #endregion



    }
}
