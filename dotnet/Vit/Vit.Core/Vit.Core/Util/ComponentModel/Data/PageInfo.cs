using Newtonsoft.Json;
using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Data
{
    public class PageInfo
    {
        /// <summary>
        /// 每页数据条数
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("10")]
        [SsDescription("每页数据条数")]
        public int pageSize;

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("1")]
        [SsDescription("页码，从1开始")]
        public int pageIndex;     
       
    }



  }
