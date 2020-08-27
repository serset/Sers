using Newtonsoft.Json.Linq;

namespace Sers.Core.Module.App.AppEvent
{
    public interface  IAppEvent
    {
        void InitEvent(JObject arg);

        void BeforeStart();

        void OnStart();

        void AfterStart();

        void BeforeStop();

        void AfterStop();
    }
}
