using Sers.Core.Extensions;
using Sers.Core.Module.PubSub.ShareEndpoint.Sys;


namespace Sers.Core.Module.PubSub.ShareEndpoint
{
    public class Publisher
    {

        public static void Publish(string msgTitle,object msgBody)
        {
            EndpointManage.Instance.Message_Publish(msgTitle, Serialization.Serialization.Instance.Serialize(msgBody).BytesToArraySegmentByte());
        }

        public static void Publish(string msgTitle, byte[] msgBody)
        {
            EndpointManage.Instance.Message_Publish(msgTitle, msgBody.BytesToArraySegmentByte());
        }
    }
}
