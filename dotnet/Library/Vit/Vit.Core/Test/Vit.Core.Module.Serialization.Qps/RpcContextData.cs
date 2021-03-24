using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Rpc
{
    public class RpcContextData
    {
        //public RpcContextData() 
        //{
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Serialize()
        {
            //return Vit.Core.Module.Serialization.Serialization.Instance.SerializeToString(this);
            return Vit.Core.Module.Serialization.Serialization_Text.Instance.SerializeToString(this);        
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToBytes() 
        {
            //return Vit.Core.Module.Serialization.Serialization.Instance.SerializeToBytes(this);
            return Vit.Core.Module.Serialization.Serialization_Text.Instance.SerializeToBytes(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData FromBytes(ReadOnlySpan<byte>data)
        {
            //return Vit.Core.Module.Serialization.Serialization.Instance.DeserializeFromSpan<RpcContextData>(data);
            return Vit.Core.Module.Serialization.Serialization_Text.Instance.DeserializeFromSpan<RpcContextData>(data);
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





        public string route;

        #region caller

        public Caller caller=new Caller();

        public class Caller 
        {
            public string rid;
            public List<string> callStack;
            public string source;

        }
        #endregion


        #region http
        public Http http=new Http();
        public class Http
        {
            public string url;
            public string method;
            public int? statusCode;
            public string protocol;
            public Dictionary<string, string> headers=new Dictionary<string, string>();

            public string GetHeader(string key) 
            {                
                if (headers.TryGetValue(key, out var value)) return value;
                return null;
            }

        }

        #endregion


        public object error;

        public object user;

 

    }
}
