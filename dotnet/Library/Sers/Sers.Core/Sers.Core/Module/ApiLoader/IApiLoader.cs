using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Api.LocalApi;

namespace Sers.Core.Module.ApiLoader
{
    public interface IApiLoader
    {
        IEnumerable<IApiNode> LoadApi(JObject config);
    }
}
