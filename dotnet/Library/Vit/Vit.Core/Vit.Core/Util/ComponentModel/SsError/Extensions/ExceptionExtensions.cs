using System;
using Newtonsoft.Json.Linq;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;

namespace Vit.Extensions
{
    public static partial class ExceptionExtensions 
    {
        #region Data

        /// <summary>
        /// SsError
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="ex"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Data_Set<Type>(this Exception ex, string key,Type data)
        {
            if (ex == null)
            {              
                throw new ArgumentNullException(nameof(ex));
            }
            ex.Data[key] = data;
        }


        /// <summary>
        /// SsError
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="ex"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Type Data_Get<Type>(this Exception ex, string key)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }
            return (ex.Data[key]).ConvertBySerialize<Type>() ;
        }
        #endregion


        #region ErrorCode

        /// <summary>
        /// SsError
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ErrorCode"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Exception ErrorCode_Set(this Exception ex,int? ErrorCode)
        {
            ex.Data_Set("ErrorCode", ErrorCode);
            return ex;
        }

        /// <summary>
        /// SsError
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int? ErrorCode_Get(this Exception ex)
        {
            return ex.Data_Get<int?>("ErrorCode");
        }
        #endregion


        #region ErrorMessage

        /// <summary>
        /// SsError
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Exception ErrorMessage_Set(this Exception ex, string ErrorMessage)
        {
            ex.Data_Set("ErrorMessage", ErrorMessage);
            return ex;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string ErrorMessage_Get(this Exception ex)
        {
            return ex.Data_Get<string>("ErrorMessage")??ex.Message;
        }
        #endregion


        #region ErrorTag
        /// <summary>
        /// SsError。自定义ErrorTag。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ErrorTag"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Exception ErrorTag_Set(this Exception ex, string ErrorTag)
        {
            ex.Data_Set("ErrorTag", ErrorTag);
            return ex;
        }

        /// <summary>
        /// SsError。自定义ErrorTag。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string ErrorTag_Get(this Exception ex)
        {
            return ex.Data_Get<string>("ErrorTag");
        }
        #endregion


        #region ErrorDetail
        /// <summary>
        /// SsError。设置ErrorDetail
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ErrorDetail"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Exception ErrorDetail_Set<Type>(this Exception ex, Type ErrorDetail)
        {
            ex.Data_Set("ErrorDetail", ErrorDetail);
            return ex;
        }
        /// <summary>
        /// SsError。获取ErrorDetail，可能为null
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Type ErrorDetail_Get<Type>(this Exception ex)
        {
            return ex.Data_Get<Type>("ErrorDetail");
        }

        /// <summary>
        /// SsError。获取ErrorDetail，可能为null
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static object ErrorDetail_Get(this Exception ex)
        {
            return ex.ErrorDetail_Get<object>();
        }
        #endregion     



        #region SsError
        /// <summary>
        /// SsError
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ssError"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Exception SsError_Set(this Exception ex, SsError ssError)
        {
            ssError?.SetErrorToException(ex);
            return ex;
        }

        /// <summary>
        /// SsError
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SsError ToSsError(this Exception ex)
        {
            return new SsError().LoadFromException(ex);
        }
        #endregion


    }
}
