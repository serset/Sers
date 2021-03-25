using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

namespace Sers.Core.Module.Rpc
{
    public class RpcContextData
    { 


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

        public JObject joUser;


    }
}
