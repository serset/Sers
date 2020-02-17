using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.LocalApi;
using System.Collections.Generic;

namespace Sers.Core.Module.ApiLoader
{
    public interface IApiLoader
    {     
        IEnumerable<IApiNode> LoadApi(JObject config);
    }
}
