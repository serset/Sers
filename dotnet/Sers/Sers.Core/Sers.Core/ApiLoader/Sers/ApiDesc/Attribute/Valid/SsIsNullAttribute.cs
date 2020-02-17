using Vit.Extensions;

namespace Sers.ApiLoader.Sers.Attribute
{
 
    public class SsIsNullAttribute : SsRpcVerifyAttribute
    {

        #region path        
        string _path;
        public string path
        {
            get => _path;
            set
            {
                _path = value;

                // { "condition":{ "type":"Not","path":"caller.source"  },    "value": { "type":"_", ssError} }
                condition = new { type = "Not", path = path }.Serialize();              
            }
        }
        #endregion


 
    }
}
