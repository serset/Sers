using System;

namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeConnection
    {
        void SendMessageAsync(Vit.Core.Util.Pipelines.ByteData message);


        void SendRequestAsync(Object sender, Vit.Core.Util.Pipelines.ByteData requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback);


        void Close();
    }
}