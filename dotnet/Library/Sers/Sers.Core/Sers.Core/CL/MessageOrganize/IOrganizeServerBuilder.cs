using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeServerBuilder
    {
        void Build(List<IOrganizeServer> organizeList, JObject config);
    }
}
