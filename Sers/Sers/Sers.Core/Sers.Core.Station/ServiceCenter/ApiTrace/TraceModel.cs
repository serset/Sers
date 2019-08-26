using System;

namespace Sers.Core.ServiceCenter.ApiTrace
{
    public class TraceModel
    {

        public static Action<TraceModel> OnPublish;

        public void Publish()
        {
            try
            {
                OnPublish?.Invoke(this);
            }
            catch
            {
            }
        }

        public string requestId;
        public string parentRequestId;
        public string rootRequestId;


        /// <summary>
        /// 建议为 DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        /// </summary>
        public DateTime startTime;

        /// <summary>
        /// 建议为 DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        /// </summary>
        public DateTime endTime;

        public string route;





    }
}
