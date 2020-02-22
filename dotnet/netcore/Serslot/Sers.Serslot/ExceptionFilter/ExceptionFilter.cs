﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;

namespace Sers.Serslot.ExceptionFilter
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// 发生异常时进入
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                ApiReturn apiRet= (SsError) context.Exception ;

                context.Result = new ContentResult
                {
                    Content = apiRet.Serialize(),//这里是把异常抛出。也可以不抛出。
                    StatusCode = StatusCodes.Status200OK,
                    //ContentType = "text/html;charset=utf-8"
                };
            }
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// 异步发生异常时进入
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }

    }
}
