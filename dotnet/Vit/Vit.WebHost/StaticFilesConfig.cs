using System;
using Microsoft.AspNetCore.Builder;
using System.IO;
using Vit.Core.Util.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.StaticFiles;
using Vit.Core.Util.ConfigurationManager;

namespace Vit.WebHost
{

    #region StaticFiles
    [JsonObject(MemberSerialization.OptIn)]
    public class StaticFilesConfig
    {

        #region rootPath       
 
        private string _rootPath = null;

        /// <summary>
        /// 静态文件路径。可为相对路径或绝对路径。若为空或空字符串则默认为入口程序所在目录下的wwwroot文件夹。demo:"wwwroot/demo"
        /// </summary>
        [JsonProperty]
        public string rootPath
        {
            get => _rootPath;
            set
            {    
                if (string.IsNullOrEmpty(value))
                {
                    _rootPath = null;
                    return;
                }
                else
                {
                    _rootPath = CommonHelp.GetAbsPath(value);
                }
            }
        }
        #endregion


        /// <summary>
        /// 回应静态文件时额外添加的http回应头。可不指定。
        /// </summary>       
        [JsonProperty]
        public IDictionary<string,string> responseHeaders { get; set; }



        #region contentTypeProvider
        internal IContentTypeProvider contentTypeProvider { get; set; }

        /// <summary>
        /// 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"contentTypeMap.json"。若不指定（或指定的文件不存在）则不指定文件类型映射配置
        /// </summary>
        [JsonProperty]
        public string contentTypeMapFile
        {            
            set
            {
                if (string.IsNullOrWhiteSpace(value)) 
                {
                    contentTypeProvider = null;
                    return;
                }

                var jsonFile = new JsonFile(value);
                if (File.Exists(jsonFile.configPath))
                {
                    var provider = new FileExtensionContentTypeProvider();
                    contentTypeProvider = provider;

                    if (jsonFile.root is JObject jo)
                    {
                        var map = provider.Mappings;
                        foreach (var item in jo)
                        {
                            map.Remove(item.Key);
                            map[item.Key] = item.Value.Value<string>();
                        }
                    }
                }
         
            }
        }

        #endregion


        /// <summary>
        /// 初始化 wwwroot静态文件配置的操作
        /// </summary>
        public Action<StaticFileOptions> OnInitStaticFileOptions { get; set; }


    }
    #endregion

}
