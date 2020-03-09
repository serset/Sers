using System.IO;
using Vit.Core.Util.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sers.Core.Module.Api.LocalApi.StaticFileTransmit
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
            get => _rootPath?? CommonHelp.GetAbsPath("wwwroot");
            set
            {
                #region (x.1) get fullPath
                string fullPath = value;
                if ("" == fullPath)
                {
                    fullPath = "wwwroot";
                }

                if (fullPath != null)
                {
                    fullPath = CommonHelp.GetAbsPath(fullPath);

                    var dir = new DirectoryInfo(fullPath);
                    if (dir.Exists)
                    {
                        fullPath = dir.FullName;
                    }
                    else
                    {
                        fullPath = null;
                    }
                }
                #endregion

                _rootPath = fullPath;
            }
        }
        #endregion


        /// <summary>
        /// 回应静态文件时额外添加的http回应头。可不指定。
        /// </summary>       
        [JsonProperty]
        public IDictionary<string,string> responseHeaders { get; set; }



        #region contentTypeProvider
 

        /// <summary>
        /// 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"mappings.json"。若不指定（或指定的文件不存在）则不指定文件类型映射配置
        /// </summary>
        [JsonProperty]
        public string contentTypeMapFile { get; set; }

        #endregion


 


    }
    #endregion

}
