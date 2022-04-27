using System;

using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Core.Util.ComponentModel.Data
{
    public class ApiReturn
    {
        /// <summary>
        /// construct with success result
        /// </summary>
        public ApiReturn()
        {
        }

        public ApiReturn(bool success)
        {
            this.success = success;
        }


        /// <summary>
        /// construct with error result
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorTag">demo: "150721_lith_1"</param>
        /// <param name="errorDetail"></param>
        public ApiReturn(int? errorCode = null, string errorMessage = null, string errorTag = null, object errorDetail = null)
        {
            success = false;
            error = new SsError.SsError(errorCode, errorMessage, errorTag, errorDetail);
        }



        /// <summary>
        /// whether success
        /// </summary>
        [SsExample("true")]
        [SsDescription("whether success")]
        public bool success = true;

        /// <summary>
        /// error info
        /// </summary>
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [SsDescription("error info")]
        public SsError.SsError error;




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn(bool success)
        {
            return new ApiReturn(success);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn(SsError.SsError error)
        {
            return new ApiReturn() { success = false, error = error };
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn(Exception ex)
        {
            return (SsError.SsError)ex;
            //SsError error = ex;
            //return error;
        }
    }


    public class ApiReturn<T> : ApiReturn
    {
        public ApiReturn() { }

        public ApiReturn(T data) { this.data = data; }


        /// <summary>
        /// data
        /// </summary>
        [SsDescription("data")]
        public T data;



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(T value)
        {
            return new ApiReturn<T>(value);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(bool success)
        {
            return new ApiReturn<T>() { success = success };
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(SsError.SsError error)
        {
            return new ApiReturn<T>() { success = false, error = error };
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator ApiReturn<T>(Exception ex)
        {
            return (SsError.SsError)ex;
        }
    }
}
