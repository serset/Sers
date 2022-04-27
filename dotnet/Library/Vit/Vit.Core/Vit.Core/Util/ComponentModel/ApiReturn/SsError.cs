using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Vit.Core.Util.ComponentModel.Data;
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
        /// <param name="errorTag">demo: "150721_lith_1"</param>
        /// <param name="errorDetail"></param>
        public SsError(int? errorCode = null, string errorMessage = null, string errorTag = null, object errorDetail = null)
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
        [SsExample("error occurred in the operation")]
        public string errorMessage { get; set; }


        /// <summary>
        /// demo: "150721_lith_1"
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("150721_lith_1")]
        public string errorTag { get; set; }



        /// <summary>
        /// errorDetail(json)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsExample("{}")]
        [SsDescription("errorDetail(json)")]
        public object errorDetail { get; set; }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual SsError LoadFromException(Exception ex)
        {
            errorCode = ex.ErrorCode_Get();
            errorMessage = ex.ErrorMessage_Get();
            errorTag = ex.ErrorTag_Get();
            errorDetail = ex.ErrorDetail_Get();
            return this;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual Exception SetErrorToException(Exception ex)
        {
            if (null != errorCode) ex.ErrorCode_Set(errorCode);
            if (null != errorMessage) ex.ErrorMessage_Set(errorMessage);
            if (null != errorTag) ex.ErrorTag_Set(errorTag);
            if (null != errorDetail) ex.ErrorDetail_Set(errorDetail);            
            return ex;
        }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator SsError(Exception ex)
        {
           return new SsError().LoadFromException(ex);
        }


        #region ToApiReturn

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ApiReturn ToApiReturn()
        {
            return this;
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ApiReturn<T> ToApiReturn<T>()
        {
            return this;
        }
        #endregion


        #region ToException

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Exception ToException(string defaultMessage = null)
        {
            var ex = new Exception(errorMessage ?? defaultMessage ?? "Error");
            SetErrorToException(ex);
            return ex;
        }
        #endregion
    }
}
