﻿using Newtonsoft.Json;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Api.Data
{
    public class PageInfo
    {
        /// <summary>
        /// 每页数据个数
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("10")]
        [SsDescription("每页数据个数")]
        public int? pageSize;

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("1")]
        [SsDescription("页码，从1开始")]
        public int? pageIndex;     
       
    }


    public class PageData<T> : PageInfo
    {

        public PageData() { }

        public PageData(PageInfo pageInfo)
        {
            if (null != pageInfo)
            {
                pageSize = pageInfo.pageSize;
                pageIndex = pageInfo.pageIndex;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        [SsDescription("数据")]
        public List<T> rows;


        /// <summary>
        /// 总数据个数
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("1000")]
        [SsDescription("总数据个数")]
        public int? totalCount;
    }
  }
