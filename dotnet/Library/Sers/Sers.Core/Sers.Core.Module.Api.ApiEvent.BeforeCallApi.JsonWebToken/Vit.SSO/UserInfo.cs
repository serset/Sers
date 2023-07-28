#region << version 1.3 >>
/* ========================================================================
 * Author  : Lith
 * Version : 1.3
 * Date    : 2023-06-18
 * Email   : serset@yeah.net
 * ======================================================================== */
#endregion

using System;
using System.Collections.Generic;

namespace Vit.SSO.Model
{
    public class UserInfo
    {
        public Dictionary<string, string> Claims { get; set; }
        public IEnumerable<string> Audiences { get; set; }
        public string Issuer { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
