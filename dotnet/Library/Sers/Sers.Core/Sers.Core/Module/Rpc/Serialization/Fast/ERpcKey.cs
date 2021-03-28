using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Rpc.Serialization.Fast
{
    enum ERpcKey : byte
    {
        route,

        caller_rid,
        caller_source,

        http_url,
        http_method,
    }
}
