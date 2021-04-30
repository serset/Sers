using System;  
using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Data
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
        public ApiReturn(int? errorCode = null, string errorMessage = null, string errorTag = null, Newtonsoft.Json.Linq.JObject errorDetail = null)
        {
            success = false;
            error = new SsError.SsError (errorCode, errorMessage, errorTag, errorDetail);
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
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [SsDescription("错误信息")]
        public SsError.SsError error;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn(SsError.SsError error)
        {
            return new ApiReturn() { success = false, error = error };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn(bool success)
        {
            return new ApiReturn(success);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn(Exception ex)
        {
            return (SsError.SsError)ex;
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(T value)
        {
            return new ApiReturn<T>(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(SsError.SsError error)
        {
            return new ApiReturn<T>(){ success = false, error = error };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(bool success)
        {
            return new ApiReturn<T>() { success=success};
        }
    }
}
