using Sers.Core.Module.App;
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

        #region (x.0) static init
        static UserSubController()
        {
            SersApplication.onStart += SubscriberDemo.Subscribe;
        }
        #endregion



        public UserSubController() : base("AuthCenter.User.Login")
        {
        }

        public override void Handle(MsgItem msgBody)
        {
            Console.WriteLine("[PubSub/Controller][AuthCenter.User.Login] name:" + msgBody.name);
        }
    }
}
