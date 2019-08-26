using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Health
{
    public interface IHealthInfo
    {
        void GetHealthInfo(JObject info,string keyPrefix=null);
    }
}
