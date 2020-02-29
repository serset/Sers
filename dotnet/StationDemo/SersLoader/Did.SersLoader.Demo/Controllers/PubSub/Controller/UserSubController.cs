using System;

namespace Did.SersLoader.Demo.Controllers.PubSub.Controller
{
    public class MsgItem
    {
        public string name;
        public DateTime time;

    }
    public class UserSubController : Sers.Core.Module.PubSub.Controller.SubscriberController<MsgItem>
    {
        public UserSubController() : base("AuthCenter.User.Login")
        {
        }

        public override void Handle(MsgItem msgBody)
        {
            Console.WriteLine("[PubSub/Controller][AuthCenter.User.Login] name:" + msgBody.name);
        }
    }
}
