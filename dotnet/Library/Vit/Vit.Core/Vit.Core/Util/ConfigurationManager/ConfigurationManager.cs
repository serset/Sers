
namespace Vit.Core.Util.ConfigurationManager
{

    /// <summary>
    /// please repace this with Vit.Core.Util.ConfigurationManager.Appsetting
    /// </summary>
    //[Obsolete("use Vit.Core.Util.ConfigurationManager.Appsetting instead.")]
    public class ConfigurationManager
    {

        /// <summary>
        /// please repace this with Vit.Core.Util.ConfigurationManager.Appsetting.json
        /// </summary>
        //[Obsolete("use Vit.Core.Util.ConfigurationManager.Appsetting.json instead.")]
        public static JsonFile Instance => Appsettings.json;

    }
}
