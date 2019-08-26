using Sers.Core.Extensions;


namespace Sers.Core.Module.ApiDesc.Attribute
{
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
        /// == !=  等
        /// </summary>
        public string type { get; set; }

        protected virtual void FlushCondition()
        {
            // { "condition":{ "type":"==","path":"caller.source",  "value":SsExp  },    "value": { "type":"_", ssError} }
            condition = new { type= type, path= path, value = value }.Serialize();
        }
 
         

     

    }
}
