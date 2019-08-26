using System.Collections.Generic;
using System.Reflection;

namespace Sers.Core.Module.SersDiscovery
{
    public class SersDiscoveryMng
    {

        #region sersDiscoveries

        private readonly List<ISersDiscovery> sersDiscoveries = new List<ISersDiscovery>();
        public void AddDiscovery(ISersDiscovery sersDiscovery)
        {
            sersDiscoveries.Add(sersDiscovery);
        }
        #endregion


        public void Discovery(Assembly assembly, DiscoveryConfig config = null)
        {
            if (null == config) config = new DiscoveryConfig();
            config.assembly = assembly;
            Discovery(config);
        }

        public void Discovery(DiscoveryConfig config)
        {
            foreach (var item in sersDiscoveries)
            {
                item?.Discovery(config);
            } 
        }

    }
}
