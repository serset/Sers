using System.Collections.Generic;
using System.IO;
using System.Web;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Rpc;

using Vit.Core.Util.Common;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.Core.Module.Api.LocalApi.StaticFileTransmit
{

    //Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(responseFileName, Encoding.UTF8));
    //Response.AddHeader("Content-Length", new FileInfo(filePath).Length.ToString());
    //Response.ContentType = (ContentType ?? MimeMapping(Path.GetExtension(responseFileName)));
    //Response.ContentEncoding = Encoding.UTF8;
    //Response.Charset = "utf-8";

    // https://www.cnblogs.com/lonelyxmas/p/12441606.html
    // https://www.cnblogs.com/Leo_wl/p/6059349.html

    //Response.Headers.Append("Cache-Control", "public,max-age=600");
    //(小知识: max - age：表示当访问此网页后的max - age秒内再次访问不会去服务器请求，其功能与Expires类似，只是Expires是根据某个特定日期值做比较。一但缓存者自身的时间不准确.则结果可能就是错误的，而max - age,显然无此问题.。Max - age的优先级也是高于Expires的。)



    public class StaticFileMap
    {

        #region fileBasePath
        private string _fileBasePath;
        /// <summary>
        /// D://fold1/wwwroot
        /// 静态文件绝对路径
        /// </summary>
        public string fileBasePath
        {
            get => _fileBasePath;
            set
            {
                #region (x.1) get fullPath
                string fullPath = value;

                if (string.IsNullOrEmpty(fullPath))
                {
                    fullPath = "wwwroot";
                }
                fullPath = CommonHelp.GetAbsPath(fullPath);
                #endregion

                _fileBasePath = fullPath;
            }
        }
        #endregion




        #region contentTypeProvider       
        readonly FileExtensionContentTypeProvider contentTypeProvider = new FileExtensionContentTypeProvider();

        #region LoadContentTypeFromFile
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool LoadContentTypeFromFile(string filePath)
        {
            var jsonFile = new JsonFile(filePath);
            if (File.Exists(jsonFile.configPath))
            {

                var map = contentTypeProvider.Mappings;
                foreach (var item in (jsonFile.root as JObject))
                {
                    map.Remove(item.Key);
                    map[item.Key] = item.Value.Value<string>();
                }
                return true;
            }
            return false;
        }
        #endregion
        #endregion


        #region responseHeaders
        public IDictionary<string, string> responseHeaders { get; set; }
        #endregion


        /// <summary>
        /// fileBasePath：静态文件路径。可为相对路径或绝对路径。若不指定（null或空字符串）则默认为入口程序所在目录下的wwwroot文件夹。
        ///   demo  D://fold1/wwwroot 
        /// </summary>
        /// <param name="fileBasePath"></param>
        public StaticFileMap(string fileBasePath = null)
        {
            this.fileBasePath = fileBasePath;
        }

        public StaticFileMap(StaticFilesConfig config)
        {
            this.fileBasePath = config.rootPath;
            responseHeaders = config.responseHeaders;
            LoadContentTypeFromFile(config.contentTypeMapFile);
        }



        /// <summary>
        /// 获取当前url对应的相对文件路径(若不合法，则返回null)。demo:"rpc/2.html"
        /// <para>（若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则 relativePath为"1/2.html"）</para>
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public string GetRelativePath()
        {
            return RpcContext.RpcData.http_url_RelativePath_Get();
        }

        /// <summary>
        /// 获取当前url映射到文件系统中的绝对路径。demo:"/root/netapp/FileStorage/wwwroot/fold2/a.html"
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public string GetAbsFilePath()
        {
            var relativePath = GetRelativePath();

            if (string.IsNullOrWhiteSpace(relativePath)) return null;

            string absFilePath = Path.Combine(fileBasePath, relativePath);
            return absFilePath;
        }




        #region TransmitFile

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public byte[] TransmitFile()
        {
            return TransmitFile(GetAbsFilePath());
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public byte[] TransmitFile(string absFilePath)
        {
            var fileInfo = new FileInfo(absFilePath);
            if (!fileInfo.Exists)
            {
                return SsError.Err_404NotFound.SerializeToBytes();
            }

            #region reply header
            var replyRpcData = new RpcContextData();
            var rpcHeaders = replyRpcData.http.Headers();

            if (responseHeaders != null)
            {
                foreach (var item in responseHeaders)
                {
                    rpcHeaders[item.Key] = item.Value;
                }
            }

            if (contentTypeProvider.TryGetContentType(absFilePath, out var contentType))
            {
                rpcHeaders["Content-Type"] = contentType;
            }

            rpcHeaders["Content-Length"] = fileInfo.Length.ToString();

            //rpcHeaders["Cache-Control"] = "public,max-age=6000";

            RpcContext.Current.apiReplyMessage.rpcContextData_OriData = replyRpcData.ToBytes().BytesToArraySegmentByte();
            #endregion


            return File.ReadAllBytes(absFilePath);

        }

        #endregion






        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public byte[] DownloadFile(string absFilePath, string fileName = null)
        {
            if (string.IsNullOrEmpty(absFilePath))
            {
                absFilePath = GetAbsFilePath();
            }

            contentTypeProvider.TryGetContentType(absFilePath, out var contentType);

            return DownloadFile(absFilePath, contentType, fileName);
        }


        #region static DownloadFile


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte[] DownloadFile(string absFilePath, string contentType, string fileName = null)
        {
            var fileInfo = new FileInfo(absFilePath);
            if (!fileInfo.Exists)
            {
                return SsError.Err_404NotFound.SerializeToBytes();
            }

            #region reply header
            var replyRpcData = new RpcContextData();

            var headers = replyRpcData.http.Headers();

            if (!string.IsNullOrEmpty(contentType))
            {
                headers["Content-Type"] = contentType;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFileName(absFilePath);
            }

            #region 填充文件头
            headers["Content-Disposition"] = "attachment;filename=" + HttpUtility.UrlEncode(fileName, Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance.encoding);
            headers["Content-Length"] = fileInfo.Length.ToString();
            #endregion


            RpcContext.Current.apiReplyMessage.rpcContextData_OriData = replyRpcData.ToBytes().BytesToArraySegmentByte();
            #endregion


            return File.ReadAllBytes(absFilePath);

        }
        #endregion


    }
}
