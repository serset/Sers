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
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception ToException(this SsError error,string message=null)
        {
            var ex = new Exception(message?? error.errorMessage??"Error");
            error?.SetErrorToException(ex);
            return ex;            
        }
      
        #endregion


    }
}
