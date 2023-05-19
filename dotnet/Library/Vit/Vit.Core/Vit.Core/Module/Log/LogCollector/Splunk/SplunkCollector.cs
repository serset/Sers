using Newtonsoft.Json.Linq;

using System;

using Vit.Core.Module.Log.LogCollector.Splunk.Client;
using Vit.Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Vit.Core.Module.Log.LogCollector.Splunk
{
    public class SplunkCollector : ILogCollector
    {
        public JObject config;
        public void Init(JObject config)
        {
            if (config == null) return;

            this.config = config;

            client = config["server"]?.Deserialize<SplunkClient>();
            hostInfo = config["hostInfo"]?.Deserialize<SplunkRecord>();
            appInfo = config["appInfo"]?.Deserialize<object>();
            client?.Init();
        }



        internal SplunkClient client;
        internal SplunkRecord hostInfo;
        public object appInfo;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(Log.LogMessage msg)
        {
            var recordEvent = new LogEvent
            {
                level = msg.level.ToString(),
                message = msg.message,
                metadata = msg.metadata,
                appInfo = appInfo
            };
            if (recordEvent.metadata != null && recordEvent.metadata.Length == 0) recordEvent.metadata = null;
            var record = new SplunkRecord
            {
                Time = DateTime.UtcNow,
                host = hostInfo?.host ?? Environment.MachineName,
                source = hostInfo?.source,
                sourcetype = hostInfo?.sourcetype,

                @event = recordEvent
            };

            client.SendAsync(record);
        }


 
    }
}
