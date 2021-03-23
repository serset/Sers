using System;
using System.Runtime.CompilerServices;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Message;
using Vit.Extensions;

namespace Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions
{

    public static class LocalApiMngExtensions
    {
        #region CallLocalApi 


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CallLocalApi(this LocalApiService data,string route, Object arg,Action<ApiMessage> onSuc)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);                   

            data.CallApiAsync(null, apiRequestMessage, (sender, apiReplyMessage) => 
            {               
                onSuc(apiReplyMessage);
            });

        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CallLocalApi<ReturnType>(this LocalApiService data, string route, Object arg,Action<ReturnType> onSuc)
        {
            data.CallLocalApi(route, arg, replyMessage =>
              {
                  var returnBytes = replyMessage.value_OriData;
                  var returnValue = returnBytes.DeserializeFromArraySegmentByte<ReturnType>();
                  onSuc(returnValue);
              });
        }

        #endregion



    }
}
