using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Core.Util.Threading.Worker
{
    internal class WorkerHelp
    {
        public static SsError Error_CannotChangeThreadCountWhileRunning =>
          new SsError { errorMessage = "can't change threadCount while tasks is running.", errorTag = "lith_190223_01" };

        public static SsError Error_CannotStartWhileRunning =>
            new SsError
            {
                errorMessage = "can't  start while task is running.",
                errorTag = "lith_190223_02"
            };

    }
}
