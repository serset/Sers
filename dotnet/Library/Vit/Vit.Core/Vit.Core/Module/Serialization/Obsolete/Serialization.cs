


using System;

namespace Vit.Core.Module.Serialization
{
    /// <summary>
    /// please use Vit.Core.Module.Serialization.Json instead
    /// </summary>
    [Obsolete("use Vit.Core.Module.Serialization.Json instead")]
    public class Serialization
    {
        /// <summary>
        /// please use Vit.Core.Module.Serialization.Json.Instance instead
        /// </summary>
        [Obsolete("use Vit.Core.Module.Serialization.Json.Instance instead")]
        public static ISerialization Instance => Json.Instance;
    }
}
