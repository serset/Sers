using System;
using Sers.Core.Module.Log;
using Sers.Core.Module.PubSub;

namespace Sers.Gover.Core.Service.SersEvent
{
    /// <summary>
    /// 目前有 SersEvent.ServiceStation.Start  、 SersEvent.ServiceStation.Pause、 SersEvent.ServiceStation.Add、 SersEvent.ServiceStation.Remove
    /// </summary>
    public class SersEventService
    {

        public const string Event_ServiceStation_Start = "Start";
        public const string Event_ServiceStation_Pause = "Pause";
        public const string Event_ServiceStation_Add = "Add";
        public const string Event_ServiceStation_Remove = "Remove";


        public static void Publish(string eventName,object msgBody)
        {
            try
            {
                MessageClient.Publish("SersEvent.ServiceStation."+ eventName, msgBody);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }           
        }

    }
 
}
