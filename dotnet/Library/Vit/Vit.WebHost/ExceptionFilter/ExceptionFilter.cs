using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions.Serialize_Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Vit.WebHost.ExceptionFilter
{

    public class ExceptionFilter : IExceptionFilter
    {

        static readonly string Response_ContentType_Json = ("application/json; charset=" + Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance.charset);


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                Logger.Error(context.Exception);
                SsError error = (SsError)context.Exception;
                ApiReturn apiRet = error;

                context.Result = new ContentResult
                {
                    Content = apiRet.Serialize(),//这里是把异常抛出。也可以不抛出。
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = Response_ContentType_Json
                };

                context.HttpContext.Response.Headers.Add("responseState", "fail");
                context.HttpContext.Response.Headers.Add("responseError_Base64", error?.SerializeToBytes()?.BytesToBase64String());
            }
            context.ExceptionHandled = true;
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }

    }
}
