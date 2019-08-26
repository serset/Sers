using Sers.Core.Module.Env;
using Sers.Core.Module.PubSub.SersDiscovery;
using Sers.ServiceCenter.ApiCenter.Gover.Core;

namespace Sers.Gover.Controller.Subscribers
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
