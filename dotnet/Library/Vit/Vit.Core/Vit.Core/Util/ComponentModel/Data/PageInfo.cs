using Newtonsoft.Json;
using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Data
{
    public class PageInfo
    {
        /// <summary>
        /// pageSize
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("10")]
        [SsDescription("pageSize")]
        public int pageSize;

        /// <summary>
        /// pageIndex, starting from 1
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("1")]
        [SsDescription("pageIndex, starting from 1")]
        public int pageIndex;     
       
    }



  }
