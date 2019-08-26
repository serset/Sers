using Newtonsoft.Json.Linq;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.SsApiDiscovery.SersValid
{
    public class SsValidation
    {
        //  {"path":"user.userType","ssError":{}, "ssValid":{"type":"Equal","value":"Logined"} }

        /// <summary>
        /// 数据的路径
        /// </summary>
        [SsExample("caller.source")]
        [SsDescription("数据的路径")]
        public string path;
        /// <summary>
        /// 验证不通过时提示的错误
        /// </summary>
        [SsDescription("验证不通过时提示的错误")]
        public SsError ssError;
        /// <summary>
        /// 验证方式
        /// </summary>
        [SsExample("{\"type\":\"Equal\",\"value\":\"Logined\"}")]  
        [SsDescription("验证方式")]
        public JObject ssValid;
    }
}
