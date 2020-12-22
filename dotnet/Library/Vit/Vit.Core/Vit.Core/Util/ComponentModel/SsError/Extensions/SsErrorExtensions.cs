using System;
using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Extensions
{
    public static partial class SsErrorExtensions 
    {



        #region ToException
        /// <summary>
        /// error可为null（若为null,则返回空Exception）
        /// </summary>
        /// <param name="error"></param>
        /// <param name="defaultMessage"></param>
        /// <returns></returns>
        public static Exception ToException(this SsError error,string defaultMessage=null)
        {
            var ex = new Exception(error?.errorMessage ?? defaultMessage ?? "Error");
            error?.SetErrorToException(ex);
            return ex;            
        }
      
        #endregion


    }
}
