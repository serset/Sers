using System.Collections.Generic;
using Newtonsoft.Json;
using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Data
{
    public class PageData : PageData<object>
    {
        public PageData(PageInfo pageInfo = null):base(pageInfo)
        {            
        }
    }

    public class PageData<T> : PageInfo
    {

        public PageData(PageInfo pageInfo=null)
        {
            if (null != pageInfo)
            {
                pageSize = pageInfo.pageSize;
                pageIndex = pageInfo.pageIndex;
            }
        }

        /// <summary>
        /// rows
        /// </summary>
        [SsDescription("rows")]
        public List<T> rows;


        /// <summary>
        /// totalCount
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("1000")]
        [SsDescription("totalCount")]
        public int totalCount;
    }
}
