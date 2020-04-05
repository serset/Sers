using Sers.Core.Module.Api.LocalApi.Event;
using Vit.Ioc;

namespace Vit.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static class UseIocExtensions
    {
        static bool eventIsAdded = false;
        public static void UseIoc(this LocalApiEventMng data)
        {
            if (eventIsAdded) return;
            eventIsAdded = true;

            data.AddEvent_OnCreateScope(IocHelp.CreateScope);
        }

    }
}
