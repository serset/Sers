﻿using System;

using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Core.Module.Log
{

    public partial class LogMng
    {

        #region 对外 基础

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Info(string message)
        {
            Log(Level.info, message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Info(string message, params object[] metadata)
        {
            Log(Level.info, message, metadata);
        }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Debug(string message)
        {
            Log(Level.debug, message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Debug(string message, params object[] metadata)
        {
            Log(Level.debug, message, metadata);
        }




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(string message)
        {
            Log(Level.error, message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(string message, params object[] metadata)
        {
            Log(Level.error, message, metadata);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Warn(string message)
        {
            Log(Level.warn, message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Warn(string message, params object[] metadata)
        {
            Log(Level.warn, message, metadata);
        }


        #endregion

        #region 对外 扩展

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Info(object message)
        {
            Info(null, message);
        }



        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="ex"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(Exception ex)
        {
            Error(ex.Message, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssError"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Error(SsError ssError)
        {
            Error(ssError.errorMessage, ssError);
        }

        #endregion

    }
}
