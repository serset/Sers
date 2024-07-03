

namespace Sers.Core.Module.Rpc.Serialization.Fast
{

    enum ERpcPropertyName : byte
    {
        route = 10,

        caller_rid = 20,
        caller_callStack = 21,
        caller_source = 22,

        http_url = 30,
        http_method = 31,
        http_statusCode = 32,
        http_protocol = 33,
        http_headers = 34,


        error = 40,
        user = 50,
    }
}
