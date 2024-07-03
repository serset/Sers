using Newtonsoft.Json.Linq;

namespace Vit.Core.Module.Log.LogCollector
{
    public interface ILogCollector
    {
        void Init(JObject config);
        void Write(LogMessage msg);
    }
}
