using System;

using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Extensions
{
    public static partial class SsError_Extensions
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Exception ToException(this SsError data, string defaultMessage = null)
        {
            var ex = new Exception(data?.errorMessage ?? defaultMessage ?? "Error");
            data?.SetErrorToException(ex);
            return ex;
        }




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ApiReturn ToApiReturn(this SsError data)
        {
            ApiReturn apiRet = data;
            return apiRet;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ApiReturn<T> ToApiReturn<T>(this SsError data)
        {
            ApiReturn<T> apiRet = data;
            return apiRet;
        }

    }
}
