using Sers.SersLoader.ApiDesc.Attribute.RpcVerify;

using Vit.Extensions.Json_Extensions;

namespace Sers.SersLoader.ApiDesc.Attribute.Valid
{

    /// <summary>
    /// demo:
    /// [SsNotNull(path = "http.headers.Authorization",   errorMessage = "必须指定Authorization")]
    /// </summary>
    public class SsNotNullAttribute : SsRpcVerifyAttribute
    {  

        #region path        
        string _path;
        public string path
        {
            get => _path;
            set
            {
                _path = value;

                // { "type":"NotNull","path":"caller.source"  } 
                condition = new { type = "NotNull", path = path }.Serialize();              
            }
        }
        #endregion


 
    }
}
