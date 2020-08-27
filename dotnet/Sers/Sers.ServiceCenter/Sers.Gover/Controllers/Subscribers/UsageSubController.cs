using Sers.Core.Module.Env;
using Sers.Core.Module.PubSub.Controller;
using Sers.Gover.Base;

namespace Sers.Gover.Controllers.Subscribers
{
    public class UsageSubController : SubscriberController<EnvUsageInfo>
    {
        public UsageSubController() : base(UsageReporter.Pubsub_UsageInfoReportTitle)
        {
        }

        public override void Handle(EnvUsageInfo item)
        {
            GoverManage.Instance.PublishUsageInfo(item);
        }
    }
}
