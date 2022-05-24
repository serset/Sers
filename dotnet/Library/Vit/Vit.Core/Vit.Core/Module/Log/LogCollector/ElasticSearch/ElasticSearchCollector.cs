using Newtonsoft.Json.Linq;

using System;

using Vit.Extensions;

namespace Vit.Core.Module.Log.LogCollector.ElasticSearch
{
    public class ElasticSearchCollector : ILogCollector
    {
        public JObject config;
        public void Init(JObject config)
        {
            if (config == null) return;

            this.config = config;

            client = config["client"]?.Deserialize<LogClient>();
            appInfo = config["appInfo"]?.Deserialize<object>();
            client?.Init();
        }

        internal LogClient client;
        public object appInfo;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(Log.LogMessage msg)
        {
            if (msg.metadata != null && msg.metadata.Length == 0) msg.metadata = null;

            var record = new LogMessage
            {
                Time = DateTime.UtcNow,
                level = msg.level.ToString(),
                message = msg.message,
                metadata = msg.metadata,
                appInfo = appInfo
            };
            client.SendAsync(record);
        }


    }
}
