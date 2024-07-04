using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using Vit.Core.Module.Log.LogCollector.ElasticSearch.Client;
using Vit.Extensions;
using Vit.Extensions.Serialize_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Vit.Core.Module.Log.LogCollector.ElasticSearch
{
    public class ElasticSearchCollector : ILogCollector
    {
        public JObject config;
        public void Init(JObject config)
        {
            if (config == null) return;

            this.config = config;

            client = config["server"]?.Deserialize<ElasticSearchClient>();
            appInfo = config["appInfo"]?.Deserialize<object>();
            client?.Init();
        }

        internal ElasticSearchClient client;
        public object appInfo;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Write(Log.LogMessage msg)
        {
            var record = new LogRecord
            {
                Time = DateTime.UtcNow,
                level = msg.level.ToString(),
                message = msg.message,
                metadata = msg.metadata,
                appInfo = appInfo
            };

            if (record.metadata != null)
            {
                if (record.metadata.Length == 0)
                    record.metadata = null;
                else
                {
                    record.metadata = record.metadata.Select(m => m?.IsValueTypeOrStringType() == true ? new { value = m } : m).ToArray();
                }
            }

            client.SendAsync(record);
        }


    }
}
