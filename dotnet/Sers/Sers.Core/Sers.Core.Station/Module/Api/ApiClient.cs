using System;
using System.Collections.Generic;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.SsError;
using Sers.Core.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Sers.Core.Module.Api
{
    public class ApiClient
    {

        #region ApiClient


        public ApiClient(ClientMqMng mqMng)
        {
            Init(mqMng.SendRequest);       
        }

        public ApiClient()
        {
            Init();
        }

        private void Init(Func<List<ArraySegment<byte>>, ArraySegment<byte>> SendRequest = null)
        { 
            this.SendRequest = SendRequest ?? Static_SendRequest?? throw new ArgumentException("ApiClient尚未初始化消息队列");
        }


        public static Func<List<ArraySegment<byte>>, ArraySegment<byte>> Static_SendRequest;





        #region CallApi

        public ReturnType CallApi<ReturnType>(string route, Object arg)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);

            var apiReplyMessage = CallApi(apiRequestMessage); 

            return Serialization.Serialization.Instance.Deserialize<ReturnType>(apiReplyMessage.value_OriData);       
        }


        public async Task<ReturnType> CallApiAsync<ReturnType>(string route, Object arg)
        {
            ReturnType ret = default(ReturnType);
            await Task.Run(() => { ret = CallApi<ReturnType>(route, arg); });
            return ret;
        }





        public string CallApi(string route, Object arg)
        {
            return CallApi<string>(route, arg);
        }

     
         
        #endregion


        private Func<List<ArraySegment<byte>>,ArraySegment<byte>> SendRequest;

        #region CallApi 

        public ApiMessage CallApi(ApiMessage request)
        {   
            try
            {
                var reply = CallApi(request.Package());
                if (null == reply || reply.Count == 0)
                {
                    //Logger.Error(new Exception().SsError_Set(SsError.Err_Timeout));
                    //返回请求超时，无回应数据
                    return new ApiMessage().InitByError(SsError.Err_Timeout);
                }
                return new ApiMessage(reply);              
            }
            catch (Exception ex) when (!(ex is ThreadInterruptedException))
            {
                ex = ex.GetBaseException();

                Logger.Error(ex);

                SsError error = ex;
                return new ApiMessage().InitByError(error);
            }          
        }

        

        public ArraySegment<byte> CallApi(List<ArraySegment<byte>> reqOri)
        {
            return SendRequest(reqOri);
        }
        #endregion

        #endregion



        #region static
        static ApiClient _Instance;
        public static ApiClient Instance
        {
            set { _Instance = value; }
            get
            {
                if (null == _Instance)
                {
                    _Instance = new ApiClient();
                }
                return _Instance;
            }
        }

        public static string CallRemoteApi(string route, string arg)
        {
            return Instance.CallApi(route, arg);
        }

       

        public static ApiMessage CallRemoteApi(ApiMessage request)
        {
            return Instance.CallApi(request); 
        }

        public static string CallRemoteApi(string route, Object arg)
        {
            return Instance.CallApi(route, arg);
        }


        public static ReturnType CallRemoteApi<ReturnType>(string route, Object arg)
        {
            return Instance.CallApi<ReturnType>(route, arg);
        }


        public static Task<ReturnType> CallRemoteApiAsync<ReturnType>(string route, Object arg)
        {
            return Instance.CallApiAsync<ReturnType>(route, arg);
        }
        #endregion
    }
}
