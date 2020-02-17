using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Sers.Core.CL.MessageOrganize
{
    public interface IOrganizeClientBuilder
    {
        void Build(List<IOrganizeClient> organizeList,JObject config);
    }
}
