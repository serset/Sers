using System;
using System.Collections.Generic;

namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeClient
    {
        IOrganizeConnection conn { get; }

        bool Start();

        void Stop();

        

        Action<IOrganizeConnection> Conn_OnDisconnected { set; }
        /// <summary>
        /// 会在内部线程中被调用 
        /// (conn,sender,requestData,callback)
        /// </summary>
        Action<IOrganizeConnection,object, ArraySegment<byte>, Action<object, List<ArraySegment<byte>>>> conn_OnGetRequest { set; }

        Action<IOrganizeConnection,ArraySegment<byte>> conn_OnGetMessage { set; }

    }
}