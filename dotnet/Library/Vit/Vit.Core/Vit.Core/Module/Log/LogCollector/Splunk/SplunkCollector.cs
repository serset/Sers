using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector.Splunk
{
    public class SplunkCollector : ILogCollector
    {
        public JObject config;
        public void Init(JObject config)
        {
            if (config == null) return;

            this.config = config;


            client = config["client"]?.Deserialize<LogClient>();
            message = config["message"]?.Deserialize<LogMessage>();
            appInfo = config["appInfo"]?.Deserialize<object>();
            client?.Init();
        }



        internal LogClient client;
        internal LogMessage message;
        public object appInfo;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(Log.LogMessage msg)
        {
            var record = new LogMessage
            {
                Time = DateTime.UtcNow,
                index = message?.index,
                host = message?.host ?? Environment.MachineName,
                source = message?.source,
                sourcetype = message?.sourcetype,

                @event = new Event
                {
                    level = msg.level.ToString(),
                    message = msg.message,
                    metadata = msg.metadata,
                    appInfo = appInfo
                }
            };

            if (record.@event.metadata != null && record.@event.metadata.Length == 0) record.@event.metadata = null;

            client.SendAsync(record);
        }








    }
}
