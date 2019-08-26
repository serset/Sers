using Sers.Core.Module.Rpc;

namespace Sers.Core.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class IRpcContextDataExtensions
    {

        public static string Bearer_Get(this IRpcContextData context)
        {
            //http.headers.Authorization = "Bearer atxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            var headers = context?.oriJson?["http"]?["headers"];
            if (null == headers) return null;

            var Authorization =   (headers["Authorization"] ?? headers["authorization"])?.ConvertToString() ;
            if (null == Authorization) return null;
            var bear = (Authorization+ " ").Split(' ')[1];           
            return bear;
        }

        public static void Bearer_Set(this IRpcContextData context,string value)
        {
            if (null == context || string.IsNullOrWhiteSpace(value)) return;        

            context.oriJson.GetOrCreateJObject("http").GetOrCreateJObject("headers")["Authorization"] = "Bearer "+value;
        } 
   

    }
}
