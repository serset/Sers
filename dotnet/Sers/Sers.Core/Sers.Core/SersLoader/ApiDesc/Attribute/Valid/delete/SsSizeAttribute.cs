using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    /// <summary>
    /// 长度限制，可以用于Array,Collection,Map,String 等
    /// </summary>
    public class SsSizeAttribute : SsValidationAttribute
    {
        private int _min = -1;
        /// <summary>
        /// 最小长度
        /// </summary>
        public int min { get { return _min; }  set { _min = value; } }

        private int _max = -1;
        /// <summary>
        /// 最大长度
        /// </summary>
        public int  max { get { return _max; } set { _max = value; } }
 

        public SsSizeAttribute()
        { 
        }
    }
}
