
using Sers.Core.Module.Rpc.Serialization;
using Sers.Core.Module.Rpc.Serialization.Fast;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vit.Core.Util.ConfigurationManager;

namespace Sers.Core.Module.Rpc
{

    [MessagePack.MessagePackFormatter(typeof(Sers.Core.Module.Rpc.Serialization.MessagePack_RpcContextData))]
    public class RpcContextData
    {

        #region Serialization
        public static IRpcSerialize Serialization;

        static RpcContextData()
        {
            string rpcDataSerializeMode = ConfigurationManager.Instance.GetByPath<string>("Sers.RpcDataSerializeMode");


            switch (rpcDataSerializeMode) 
            {
                case "BytePointor": Serialization = BytePointor_RpcContextData.Instance; break;


                //case "Newtonsoft": Serialization = Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance; break;
                case "MessagePack": Serialization = MessagePack_RpcContextData.Instance; break;
                case "StringBuilder": Serialization = StringBuilder_RpcContextData.Instance; break;
                case "Text": Serialization = Text_RpcContextData.Instance; break;
                default: Serialization = Text_RpcContextData.Instance; break;
            }
        }

        #endregion







        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public string Serialize()
        //{    
        //    return StringBuilder_RpcContextData.SerializeToString(this);
        //}

       


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToBytes() 
        {
            return Serialization.SerializeToBytes(this);     
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData FromBytes(ArraySegment<byte>data)
        {
            return Serialization.DeserializeFromBytes(data);
        }


        /*
        {
        "route": "/DemoStation/v1/api/5/rpc/2",
        "caller": {
            "rid": "8320becee0d945e9ab93de6fdac7627a",
            "callStack": ["xxxx","xxxxxx"],    // parentRequestGuid array
            "source": "Outside"
        },
        "http": {
            "url": "https://127.0.0.1:6000/DemoStation/v1/api/5/rpc/2?a=1",
            "method":"GET",
            "statusCode":400,
            "protocol":"HTTP/2.0",
            "headers": {
                "Cache-Control": "no-cache",
                "Connection": "keep-alive",
                "Content-Type": "application/javascript",
                "Accept": "* / *",
                "Accept-Encoding": "gzip, deflate",
                "Authorization": "bearer",
                "Host": "127.0.0.1:6000",
                "User-Agent": "PostmanRuntime/7.6.0",
                "Postman-Token": "78c5a1cb-764f-4e04-b2ae-514924a40d5a"
            }
        },
	    "error":{SsError},
        "user": {"userInfo":{} }
    }             
         
         */

        /* ApiClient
          {
               "route": "/DemoStation/v1/api/5/rpc/2",
               "caller": {
                   "rid": "8320becee0d945e9ab93de6fdac7627a",
                   "source": "Outside"
               },
               "http": {
                   "url": "https://127.0.0.1:6000/DemoStation/v1/api/5/rpc/2?a=1",
                   "method":"GET"            
               }
           }
            */




        public string route;

        #region caller

        public Caller caller=new Caller();

        public class Caller 
        {
            public string rid;
            public List<string> callStack;

            /// <summary>
            /// Internal、Outside
            /// </summary>
            public string source;

        }
        #endregion


        #region http
        public Http http = new Http();
        public class Http
        {
            public string url;
            public string method;
            public int? statusCode;
            public string protocol;


            public Dictionary<string, string> headers;


            #region headers ext

            /// <summary>
            /// 获取headers,确保不会为null
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Dictionary<string, string> Headers()
            {
                if (headers == null) headers = new Dictionary<string, string>();
                return headers;
            }

            /// <summary>
            /// 获取headers,确保不会为null
            /// </summary>
            /// <param name="capacity"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Dictionary<string, string> Headers(int capacity)
            {
                if (headers == null) headers = new Dictionary<string, string>(capacity);
                return headers;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string GetHeader(string key)
            {
                string value = null;
                if (true == headers?.TryGetValue(key, out value)) return value;
                return null;
            }
            #endregion

        }

        #endregion


        public object error;

        [MessagePack.MessagePackFormatter(typeof(Vit.Core.Module.Serialization.MessagePack_Newtonsoft_Object))]
        public object user;




    }
}
