﻿using System;
namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeServer
    {

        bool Start();

        void Stop();


        Action<IOrganizeConnection> Conn_OnConnected { set; }

        Action<IOrganizeConnection> Conn_OnDisconnected { set; }
        /// <summary>
        /// 会在内部线程中被调用 
        /// (conn,sender,requestData, callback)
        /// </summary>
        Action<IOrganizeConnection, object, ArraySegment<byte>, Action<object, Vit.Core.Util.Pipelines.ByteData>> conn_OnGetRequest { set; }

        Action<IOrganizeConnection, ArraySegment<byte>> conn_OnGetMessage { set; }
    }
}