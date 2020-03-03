using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vit.Core.Util.ComponentModel.Model;
using Vit.Extensions;

namespace Vit.Core.Util.ComponentModel.SsError
{
    public partial class SsError
    {
        public SsError()
        {
        }

        public SsError(Exception ex)
        {
            LoadFromException(ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorTag">自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"</param>
        /// <param name="errorDetail"></param>
        public SsError(int? errorCode = null, string errorMessage = null, string errorTag = null, JObject errorDetail=null)
        {
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
            this.errorTag = errorTag;
            this.errorDetail = errorDetail;
        }



        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("1000")]
        public int? errorCode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("操作出现异常")]
        public string errorMessage { get; set; }


        /// <summary>
        /// 自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("150721_lith_1")]
        [SsDescription("自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如：\"150721_lith_1\"")]
        public string errorTag { get; set; }



        /// <summary>
        /// 错误详情（json类型）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("{}")]
        [SsDescription("错误详情（json类型）")]
        public JObject errorDetail { get; set; }



        public virtual SsError LoadFromException(Exception ex)
        {
            errorCode = ex.ErrorCode_Get();
            errorMessage = ex.ErrorMessage_Get();
            errorTag = ex.ErrorTag_Get();
            errorDetail = ex.ErrorDetail_Get();
            return this;
        }

        public virtual Exception SetErrorToException(Exception ex)
        {
            if (null != errorCode) ex.ErrorCode_Set(errorCode);
            if (null != errorMessage) ex.ErrorMessage_Set(errorMessage);
            if (null != errorTag) ex.ErrorTag_Set(errorTag);
            if (null != errorDetail) ex.ErrorDetail_Set(errorDetail);            
            return ex;
        }



        public static implicit operator SsError(Exception ex)
        {
           return new SsError().LoadFromException(ex);
        }
    }
}
