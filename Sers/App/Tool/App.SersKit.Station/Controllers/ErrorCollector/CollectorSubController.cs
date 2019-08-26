using System;
using Dapper.Contrib.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Util.Common;
using Sers.Framework.Orm.Dapper.SqlHelp;

namespace App.SersKit.Station.Controllers.ErrorCollector
{
  
    public class CollectorSubController : Sers.Core.Module.PubSub.ShareEndpoint.SersDiscovery.SubscriberController<ErrorItem>
    {
        public CollectorSubController() : base("SersKit.Error")
        {
        }

        static readonly string dbPath = CommonHelp.GetAbsPathByRealativePath("Data", "SersKit", "ErrorCollector.db");

        public override void Handle(ErrorItem item)
        {
             
            try
            {
                item.collectTime = DateTime.Now;
                using (var conn = SqliteHelp.GetOpenConnection(dbPath))
                {
                    conn.Insert(item);
                }               
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }

        }
    }
}
