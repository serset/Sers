using System.IO;

using Microsoft.AspNetCore.StaticFiles;

using Newtonsoft.Json.Linq;

using Vit.Core.Util.ConfigurationManager;

namespace Vit.WebHost
{
    public class WebHostHelp
    {

        #region BuildContentTypeProvider
        /// <summary>
        /// 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"contentTypeMap.json"。若不指定（或指定的文件不存在）则返回null
        /// </summary>
        /// <param name="contentTypeMapFile"></param>
        /// <returns></returns>
        public static FileExtensionContentTypeProvider BuildContentTypeProvider(string contentTypeMapFile)
        {
            if (string.IsNullOrWhiteSpace(contentTypeMapFile))
            {
                return null;
            }

            var jsonFile = new JsonFile(contentTypeMapFile);
            if (File.Exists(jsonFile.configPath))
            {
                var provider = new FileExtensionContentTypeProvider();

                if (jsonFile.root is JObject jo)
                {
                    var map = provider.Mappings;
                    foreach (var item in jo)
                    {
                        map.Remove(item.Key);
                        map[item.Key] = item.Value.Value<string>();
                    }
                }
                return provider;
            }

            return null;
        }
        #endregion

    }
}
