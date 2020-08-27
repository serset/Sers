using System;
using System.Collections.Generic;

namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeConnection
    {
        string connTag { get; set; }

        void SendMessageAsync(List<ArraySegment<byte>> message);


        void SendRequestAsync(Object sender, List<ArraySegment<byte>> requestData, Action<object, List<ArraySegment<byte>>> callback);
        bool SendRequest(List<ArraySegment<byte>> requestData, out List<ArraySegment<byte>> replyData);


        void Close();
    }
}