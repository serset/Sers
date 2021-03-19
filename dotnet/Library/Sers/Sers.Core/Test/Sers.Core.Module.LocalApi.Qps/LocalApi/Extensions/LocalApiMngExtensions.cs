using System;
using System.Runtime.CompilerServices;
using System.Threading;

using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Message;

using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions
{

    public static class LocalApiMngExtensions
    {
        #region CallLocalApi


        #region static curAutoResetEvent      
        public static AutoResetEvent curAutoResetEvent =>
            _curAutoResetEvent.Value ?? (_curAutoResetEvent.Value = new AutoResetEvent(false));

        static AsyncCache<AutoResetEvent> _curAutoResetEvent = new AsyncCache<AutoResetEvent>();
        #endregion



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> CallLocalApi(this LocalApiService data,string route, Object arg)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);

            ApiMessage apiReplyMessage=null;

            AutoResetEvent mEvent = curAutoResetEvent;
            mEvent.Reset();

            data.CallApiAsync(null, apiRequestMessage, (sender,_apiReplyMessage)=> 
            {
                apiReplyMessage = _apiReplyMessage;
                mEvent?.Set();
            });

  
            //TODO
            int millisecondsTimeout = 60000;
            mEvent.WaitOne(millisecondsTimeout);
            mEvent = null;

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
