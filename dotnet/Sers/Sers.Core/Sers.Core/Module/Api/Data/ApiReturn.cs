using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.Api.Data
{
    public class ApiReturn
    {
        /// <summary>
        /// 构建成功的返回数据
        /// </summary>
        public ApiReturn()
        {
        }

        public ApiReturn(bool success)
        {
            this.success = success;
        }


        /// <summary>
        /// 构造失败的返回数据
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorTag">自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"</param>
        /// <param name="errorDetail"></param>
        public ApiReturn(int? errorCode = null, string errorMessage = null, string errorTag = null, JObject errorDetail = null)
        {
            success = false;
            error = new SsError (errorCode, errorMessage, errorTag, errorDetail);
        }



        /// <summary>
        /// 是否成功
        /// </summary>
        [SsExample("true")]
        [SsDescription("是否成功")]
        public bool success=true;

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsDescription("错误信息")]
        public SsError error;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public static implicit operator ApiReturn(SsError error)
        {
            return new ApiReturn() { success = false, error = error };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        public static implicit operator ApiReturn(bool success)
        {
            return new ApiReturn(success);
        }

        public static implicit operator ApiReturn(Exception ex)
        {
            return (SsError)ex;
            //SsError error = ex;
            //return error;
        }
    }


    public class ApiReturn<T>: ApiReturn
    {

        public ApiReturn() { }

        public ApiReturn(T data) { this.data = data; }


        /// <summary>
        /// 数据
        /// </summary>
        [SsDescription("数据")]
        public T data;
                                   



        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiReturn<T>(T value)
        {
            return new ApiReturn<T>(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public static implicit operator ApiReturn<T>(SsError error)
        {
            return new ApiReturn<T>(){ success = false, error = error };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        public static implicit operator ApiReturn<T>(bool success)
        {
            return new ApiReturn<T>() { success=success};
        }
    }
}
