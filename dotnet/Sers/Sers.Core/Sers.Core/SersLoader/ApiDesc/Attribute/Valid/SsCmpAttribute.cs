using Sers.SersLoader.ApiDesc.Attribute.RpcVerify;
using Vit.Extensions;

namespace Sers.SersLoader.ApiDesc.Attribute.Valid
{
    /// <summary>
    /// demo:
    /// [SsCmp(path = "http.method", type = "==" , value="PUT")]
    /// </summary>
    public class SsCmpAttribute : SsRpcVerifyAttribute
    {

        #region path        
        string _path;
        public string path
        {
            get => _path;
            set
            {
                _path = value;
                FlushCondition();
            }
        }
        #endregion

        #region value

        string _value;
        public string value
        {
            get => _value;
            set
            {
                _value = value;
                FlushCondition();
            }
        }
        #endregion


        /// <summary>
        /// 可为 == !=  &gt;  &gt;=  &lt; &lt;= 等
        /// </summary>
        public string type { get; set; }

        protected virtual void FlushCondition()
        {
            //  { "type":"==","path":"caller.source",  "value":SsExp  }  
            condition = new { type= type, path= path, value = value }.Serialize();
        }
 
         

     

    }
}
