using System;
using System.Collections.Generic;

namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeConnection
    {
        string connTag { get; set; }

        void SendMessageAsync(Vit.Core.Util.Pipelines.ByteData message);


        void SendRequestAsync(Object sender, Vit.Core.Util.Pipelines.ByteData requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback);
        bool SendRequest(Vit.Core.Util.Pipelines.ByteData requestData, out Vit.Core.Util.Pipelines.ByteData replyData);


        void Close();
    }
}