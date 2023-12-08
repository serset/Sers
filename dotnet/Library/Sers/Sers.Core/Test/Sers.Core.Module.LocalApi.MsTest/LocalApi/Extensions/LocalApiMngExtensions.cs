using System;
using System.Runtime.CompilerServices;
using System.Threading;

using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Message;

using Vit.Extensions;
using Vit.Extensions.Object_Serialize_Extensions;

namespace Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions
{

    public static class LocalApiMngExtensions
    {
        #region CallLocalApi

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> CallLocalApi(this LocalApiService data,string route, Object arg)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);

            ApiMessage apiReplyMessage=null;

            AutoResetEvent mEvent = new AutoResetEvent(false);
            mEvent.Reset();

            data.InvokeApiAsync(null, apiRequestMessage, (sender,_apiReplyMessage)=> 
            {
                apiReplyMessage = _apiReplyMessage;
                mEvent?.Set();
            });

  
            //TODO
            int millisecondsTimeout = 60000;
            mEvent.WaitOne(millisecondsTimeout);

            return apiReplyMessage.value_OriData;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReturnType CallLocalApi<ReturnType>(this LocalApiService data, string route, Object arg)
        {
            var returnValue = data.CallLocalApi(route, arg);
            return returnValue.DeserializeFromArraySegmentByte<ReturnType>();
        }

        #endregion

      

    }
}
