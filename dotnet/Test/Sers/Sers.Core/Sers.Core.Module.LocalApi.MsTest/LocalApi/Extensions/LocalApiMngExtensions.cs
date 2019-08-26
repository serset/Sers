using System;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Serialization;

namespace Sers.Core.Extensions
{

    public static class LocalApiMngExtensions
    {
        #region CallLocalApi

        public static ArraySegment<byte> CallLocalApi(this LocalApiMng data,string route, Object arg)
        {
            var apiRequestMsg = new ApiMessage().InitAsApiRequestMessage(route, arg);

            var apiReplyMessage = data.CallLocalApi(apiRequestMsg);

            return apiReplyMessage.value_OriData;
        }



        public static ReturnType CallLocalApi<ReturnType>(this LocalApiMng data, string route, Object arg)
        {
            var returnValue = data.CallLocalApi(route, arg);
            return Serialization.Instance.Deserialize<ReturnType>(returnValue);
        }

        #endregion

      

    }
}
