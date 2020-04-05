using Sers.Core.Module.Api.LocalApi.ApiTrace;
using Sers.Core.Module.Api.LocalApi.Event;
using Vit.Core.Util.ConfigurationManager;


namespace Vit.Extensions
{
    public static partial class UseApiTraceLogExtensions
    {
        static bool eventIsAdded = false;
        /// <summary>
        /// txt log  ("2018-01-01apitrace.log")
        /// </summary>
        /// <param name="data"></param>
        public static void UseApiTraceLog(this LocalApiEventMng data)
        {
            if (eventIsAdded) return;
            eventIsAdded = true;

            if (true != ConfigurationManager.Instance.GetByPath<bool?>("Sers.LocalApiService.PrintTrace"))
                return;

            data.AddEvent_OnCreateScope(() => new ApiTraceLog());
        }



    }
}
