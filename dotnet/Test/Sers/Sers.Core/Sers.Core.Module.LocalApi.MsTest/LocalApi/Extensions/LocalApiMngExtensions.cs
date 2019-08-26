using System;
using System.Threading;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Serialization;

namespace Sers.Core.Extensions
{

    public static class LocalApiMngExtensions
    {
        #region CallLocalApi

        public static ArraySegment<byte> CallLocalApi(this LocalApiService data,string route, Object arg)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);

            ApiMessage apiReplyMessage=null;

            AutoResetEvent mEvent = new AutoResetEvent(false);
            mEvent.Reset();

            data.CallApiAsync(null, apiRequestMessage, (sender,_apiReplyMessage)=> 
            {
                apiReplyMessage = _apiReplyMessage;
                mEvent?.Set();
            });

  
            //TODO
            int millisecondsTimeout = 60000;
            mEvent.WaitOne(millisecondsTimeout);

            return apiReplyMessage.value_OriData;
        }



        public static ReturnType CallLocalApi<ReturnType>(this LocalApiService data, string route, Object arg)
        {
            var returnValue = data.CallLocalApi(route, arg);
            return returnValue.DeserializeFromBytes<ReturnType>();
        }

        #endregion

      

    }
}
