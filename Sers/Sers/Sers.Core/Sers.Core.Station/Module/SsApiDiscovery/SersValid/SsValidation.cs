using Newtonsoft.Json.Linq;
using Sers.Core.Util.SsError;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.SsApiDiscovery.SersValid
{
    public class SsValidation
    {
        //  {"path":"user.userType","ssError":{}, "ssValid":{"type":"Equal","value":"Logined"} }

        public string path;
        public SsError ssError;
        public JObject ssValid;
    }
}
