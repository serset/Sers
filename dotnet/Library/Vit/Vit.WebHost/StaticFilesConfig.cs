using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

using Newtonsoft.Json;

using Vit.Core.Util.Common;

namespace Vit.WebHost
{

    #region StaticFiles
    [JsonObject(MemberSerialization.OptIn)]
    public class StaticFilesConfig
    {

        /// <summary>
        /// 请求路径（可不指定）。demo："/file/static"。The relative request path that maps to static resources
        /// </summary>       
        [JsonProperty]
        public string requestPath { get; set; }

        #region rootPath       

        private string _rootPath = null;

        /// <summary>
        /// 静态文件路径。可为相对路径或绝对路径。若为空或空字符串则为默认路径（wwwroot）。demo:"wwwroot"
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
        /// 默认页面（可不指定）。An ordered list of file names to select by default. List length and ordering  may affect performance
        /// </summary>       
        [JsonProperty]
        public List<string> defaultFileNames { get; set; }


        /// <summary>
        /// 是否可浏览目录(default false)。Enables directory browsing
        /// </summary>       
        [JsonProperty]
        public bool? useDirectoryBrowser { get; set; }



        /// <summary>
        /// 回应静态文件时额外添加的http回应头。可不指定。
        /// </summary>       
        [JsonProperty]
        public IDictionary<string, string> responseHeaders { get; set; }



        #region contentTypeProvider
        [JsonIgnore]
        public IContentTypeProvider contentTypeProvider { get; set; }

        /// <summary>
        /// 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"contentTypeMap.json"。若不指定（或指定的文件不存在）则不指定文件类型映射配置
        /// </summary>
        [JsonProperty]
        public string contentTypeMapFile
        {
            set
            {
                contentTypeProvider = WebHostHelp.BuildContentTypeProvider(value);
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
