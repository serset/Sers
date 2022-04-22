using System;

namespace Vit.Core.Util.ConfigurationManager
{

    /// <summary>
    /// use Vit.Core.Util.ConfigurationManager.Appsetting instead.
    /// </summary>
    //[Obsolete("use Vit.Core.Util.ConfigurationManager.Appsetting instead.")]
    public class ConfigurationManager
    {

        /// <summary>
        /// use Vit.Core.Util.ConfigurationManager.Appsetting.json instead.
        /// </summary>
        //[Obsolete("use Vit.Core.Util.ConfigurationManager.Appsetting.json instead.")]
        public static JsonFile Instance => Appsettings.json;

    }
}
