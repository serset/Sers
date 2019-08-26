#region << 版本注释 - v11 >>
/*
 * ========================================================================
 * 版本：v11
 * 时间：190305
 * 作者：Lith   
 * Q  Q：755944120
 * 邮箱：litsoft@126.com
 * 
 * ========================================================================
*/
#endregion



using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace FrameWork.Net
{
    #region class HttpUtil

    
    /// <summary>
    /// http发送请求
    /// </summary>
    public class HttpUtil
    {

        #region 静态工具函数

        #region CloneStruct
        /// <summary>
        /// 从source按照Type类型拷贝数据
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Type CloneStruct<Type>(object source)
        {
            string v = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<Type>(v);
        }
        #endregion

        #region static 用以初始化https相关配置


        /// <summary>
        /// 设置请求类型为Https
        /// </summary>
        /// <param name="request"></param>
        private static void SetToHttps(HttpWebRequest request)
        {

            //if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))


            request.ProtocolVersion = HttpVersion.Version11;
            request.KeepAlive = false;

            //request.Method = "POST";    //使用get方式发送数据
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.Referer = null;
            //request.AllowAutoRedirect = true;
            //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            //request.Accept = "*/*";

            //byte[] data = Encoding.UTF8.GetBytes(postData);
            //Stream newStream = request.GetRequestStream();
            //newStream.Write(data, 0, data.Length);
            //newStream.Close();

            ////获取网页响应结果
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream stream = response.GetResponseStream();
            ////client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //string result = string.Empty;
            //using (StreamReader sr = new StreamReader(stream))
            //{
            //    result = sr.ReadToEnd();
            //}

            //return result;
        }



        #region 静态初始化器，用以初始化https相关配置
        /// <summary>
        /// 静态初始化器，用以初始化https相关配置
        /// </summary>
        static HttpUtil()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

            // 这里设置了协议类型。
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 

            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
        }
        #endregion

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        #endregion

        #region UrlEncode
        /// <summary>
        ///  System.Web.HttpUtility.UrlEncode(param)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static String UrlEncode(String param)
        {
            if (String.IsNullOrEmpty(param))
                return "";
            try
            {
                param = System.Web.HttpUtility.UrlEncode(param);
                //HttpUtility.UrlEncode(String, System.Text.Encoding.GetEncoding(936))
                //param = Microsoft.JScript.GlobalObject.encodeURIComponent(param);
            }
            catch
            {
            }
            return param;
        }
        #endregion

        #region UrlDecode
        /// <summary>
        ///  System.Web.HttpUtility.UrlDecode(param)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static String UrlDecode(String param)
        {
            if (String.IsNullOrEmpty(param))
                return "";
            try
            {
                param = System.Web.HttpUtility.UrlDecode(param);
                //HttpUtility.UrlEncode(String, System.Text.Encoding.GetEncoding(936))
                //param = Microsoft.JScript.GlobalObject.encodeURIComponent(param);
            }
            catch
            {
            }
            return param;
        }
        #endregion


        #region GetTimeStampTen
        /// <summary> 
        /// 获取时间戳 10位
        /// </summary> 
        /// <returns></returns> 
        public static string GetTimeStampTen()
        {
            return ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }
        #endregion




        #region  static FormatUrlParams
        /// <summary>
        /// 返回值demo： "a=4&b=2"
        /// </summary>
        /// <param name="parameters">可为string、IDictionary、JObject,例如："a=3&b=5"</param>
        /// <returns></returns>
        public static String FormatUrlParams(Object parameters)
        {

            if (null == parameters)
            {
                return null;
            }


            if (parameters is IDictionary)
            {
                StringBuilder buff = new StringBuilder();
                foreach (DictionaryEntry kv in (IDictionary)parameters)
                {
                    buff.Append(UrlEncode(kv.Key.ToString())).Append("=").Append(UrlEncode(kv.Value.ToString())).Append("&");
                }
                if (buff.Length > 0) buff.Length--;
                return buff.ToString();
            }
            else if (parameters is JObject)
            {
                StringBuilder buff = new StringBuilder();
                foreach (var kv in (JObject)parameters)
                {
                    buff.Append(UrlEncode(kv.Key.ToString())).Append("=").Append(UrlEncode(kv.Value.Value<string>())).Append("&");
                }
                if (buff.Length > 0) buff.Length--;
                return buff.ToString();
            }
            else if (parameters is string)
            {
                return (string)parameters;
            }
            throw new Exception("Url_BuildParam,不支持的url参数格式[" + parameters.GetType().Name + "]");
        }



        #endregion

        #region static UrlAddParams
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">不可为null,demo："http://www.a.com"、"http://www.a.com?a=1&b=2"</param>
        /// <param name="parameters">可为string、IDictionary、JObject</param>
        /// <returns></returns>
        public static string UrlAddParams(string url, Object parameters)
        {
            string urlParams = FormatUrlParams(parameters);
            if (string.IsNullOrEmpty(urlParams)) return url;

            if (0 < url?.IndexOf('?'))
            {
                return url + "&" + urlParams;
            }
            else
            {
                return url + "?" + urlParams;
            }
        }
        #endregion

        #endregion

               

        #region GetRequest
        /// <summary>
        /// 获取或设置请求超时之前的时间长度（以毫秒为单位，默认1分钟）。
        /// </summary>
        public int Timeout = 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual WebRequest GetRequest(String url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = Timeout;


            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                SetToHttps(request as HttpWebRequest);
            }

            return request;
        }
        #endregion









        #region GetContentEncoding

        #region defaultEncoding
        /// <summary>
        /// 
        /// </summary>
        protected Encoding _defaultEncoding;
        /// <summary>
        /// 
        /// </summary>
        public Encoding defaultEncoding
        {
            get { return _defaultEncoding ?? Encoding.Default; }
            set { _defaultEncoding = value; }
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected Encoding GetResponseEncoding(HttpWebResponse response)
        {
            String strEncoding = response.CharacterSet;
            if (String.IsNullOrEmpty(strEncoding))
            {
                strEncoding = response.ContentEncoding;
                if (String.IsNullOrEmpty(strEncoding))
                {
                    return defaultEncoding;
                }
            }
            return Encoding.GetEncoding(strEncoding);
        }
        #endregion


        #region SendRequestBody

        static void SendRequestBody(WebRequest request,string _contentType, byte[] _dataToWrite)
        {
            //(x.1)
            if (!String.IsNullOrEmpty(_contentType))
                request.ContentType = _contentType;

            //(x.2)
            if (null != _dataToWrite)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(_dataToWrite, 0, _dataToWrite.Length);
                }
            }
            
        }

        #endregion


        #region GetRequest

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        protected virtual WebRequest GetRequest(RequestParam requestParam)
        {
            var url = UrlAddParams(requestParam.url, requestParam.urlParams);
            WebRequest request = GetRequest(url);

            if (!string.IsNullOrEmpty(requestParam.Method))
                request.Method = requestParam.Method;

            requestParam.GetRequestBodyData(out string contentType, out byte[] dataToWrite);            
            SendRequestBody(request, contentType, dataToWrite);


            #region Headers
            if (null != requestParam.headers)
            {
                foreach (DictionaryEntry kv in requestParam.headers)
                {
                    request.Headers.Add("" + kv.Key, "" + kv.Value);
                }
            }
            #endregion


            return request;
        }

        #endregion

        #region GetResponse
        protected virtual HttpWebResponse GetResponse(RequestParam requestParam)
        {
            return GetRequest(requestParam).GetResponse() as HttpWebResponse;
        }
        #endregion


        #region 从Response中获取数据

        /// <summary>
        /// 从response读取返回的字符串
        /// </summary>
        /// <param name="response"></param>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public virtual String Response_GetString(HttpWebResponse response, RequestParam requestParam)
        {
            using (System.IO.Stream respStream = response.GetResponseStream())
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, requestParam.responseEncoding??GetResponseEncoding(response)))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        /// <summary>
        /// 转换读取的字符串为指定的类型
        /// </summary>
        /// <param name="response"></param>
        /// <param name="data"></param>
        /// <param name="responseContentType">可为"json",若不指定有效值则默认为'string'</param>
        /// <returns></returns>
        public virtual Object FormatResponseData(HttpWebResponse response, String data,string responseContentType)
        {            
            if (0 <= responseContentType.IndexOf("json"))
            {
                return JsonConvert.DeserializeObject(data);
            }
            return data;
        }

        #endregion


        #region Ajax
        /// <summary>
        /// 发起http(s)请求
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public virtual object Ajax(RequestParam requestParam)
        {
            var response = GetResponse(requestParam);

            var responseContentType = requestParam.responseContentType;
            if (string.IsNullOrEmpty(responseContentType))
                responseContentType = response.ContentType.ToLower();

            return FormatResponseData(response, Response_GetString(response, requestParam), responseContentType);
        }
        #endregion

        #region Ajax

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public virtual string Ajax_String(RequestParam requestParam)
        {
            return Response_GetString(GetResponse(requestParam), requestParam);
        }
        /// <summary>
        /// 发起http(s)请求,返回的数据先转换为string再转换为T类型（T可为 JObject/JArray等）
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public virtual T Ajax<T>(RequestParam requestParam)
        {          
            return JsonConvert.DeserializeObject<T>(Ajax_String(requestParam)); 
        }
        #endregion

        #region Ajax_GetByte
        public virtual byte[] Ajax_GetByte(RequestParam requestParam)
        {
            return Response_GetByte(GetResponse(requestParam));
        }

        #region Response_GetByte

        /// <summary>
        /// 下载byte数据
        /// 从response 下载byte数据
        /// </summary>
        /// <param name="response"></param> 
        protected virtual byte[] Response_GetByte(HttpWebResponse response)
        {
            using (System.IO.Stream stream = response.GetResponseStream())
            {
                long length;
                try
                {
                    length = response.ContentLength;
                }
                catch (Exception)
                {
                    length = stream.Length;
                }

                byte[] bytes = new byte[length];
                int readedLen = 0;
                while (readedLen < length)
                {
                    readedLen += stream.Read(bytes, readedLen, bytes.Length - readedLen);
                }              
                return bytes;
            }
        }
        #endregion
        #endregion   

        #region Ajax_DownloadFile
        public virtual void Ajax_DownloadFile(RequestParam requestParam, String filePath)
        {
            Response_DownloadFile(GetResponse(requestParam), filePath);
        }

        #region Response_DownloadFile

        /// <summary>
        /// 下载文件
        /// 从response 下载数据放到文件filePath中。
        /// </summary>
        /// <param name="response"></param>
        /// <param name="filePath"></param>
        protected virtual void Response_DownloadFile(HttpWebResponse response, String filePath)
        {
            using (System.IO.Stream si = response.GetResponseStream())
            {
                using (System.IO.Stream so = new System.IO.FileStream(filePath, System.IO.FileMode.CreateNew))
                {
                    long totalDownloadedByte = 0;
                    byte[] by = new byte[1024];
                    int osize;
                    while (0 < (osize = si.Read(by, 0, (int)by.Length)))
                    {
                        totalDownloadedByte += osize;
                        so.Write(by, 0, osize);
                    }
                }
            }
        }
        #endregion

        #endregion




        #region Ajax_Get
        /// <summary>
        /// 发起http(s) get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlParams">放到url中的参数。可为string、IDictionary、JObject</param>
        /// <returns></returns>

        public virtual object Ajax_Get(string url, Object urlParams)
        {
            return Ajax(new RequestParam() { url = url, urlParams = urlParams });
        }


        #endregion

        #region Ajax_Post
        /// <summary>
        /// 发起http(s) post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="getParameters">放到url中的参数。可为string、IDictionary、JObject </param>
        /// <param name="postParameters">post请求参数（消息主体）。可为string、IDictionary、JObject/JArray </param>
        /// <returns></returns>
        public virtual object Ajax_Post(string url, Object getParameters, Object postParameters)
        {
            return Ajax(new RequestParam() { url = url, Method = "POST", urlParams = getParameters, body = postParameters });
        }
        /// <summary>
        /// 发起http(s) post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParameters">post请求参数（消息主体）。可为string、IDictionary、JObject/JArray </param>
        /// <returns></returns>
        public virtual object Ajax_Post(string url, Object postParameters)
        {
            return Ajax(new RequestParam() { url = url, Method = "POST", body = postParameters });
        }

        #endregion


       


        #region 异步
        /*/

        #region delegate


        /// <summary>
        ///  delegate to get data
        /// 获取数据的 delegate
        /// </summary>
        /// <returns></returns>
        public delegate Object DelGetData();


        /// <summary>
        /// what to do after  geted data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="e"></param>
        public delegate void DelOnGetData(Object data, Exception e);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public delegate void DelOnSuccess(Object data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public delegate void DelOnFail(Exception e);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public delegate void DelOnException(Exception e);
        /// <summary>
        /// 
        /// </summary>
        public delegate void DelOnFinally();

        #endregion

                #region GetDel
        /// <summary>
        /// example: DelOnGetData del = GetDelOnGetData((Object data) => { }, (Exception e) => { }, (Exception e) => { }, () => { });
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <param name="onException"></param>
        /// <param name="onFinally"></param>
        /// <returns></returns>
        public static DelOnGetData GetDelOnGetData(DelOnSuccess onSuccess, DelOnFail onFail, DelOnException onException, DelOnFinally onFinally)
        {

            return (Object data, Exception e) =>
            {
                try
                {
                    if (null != e)
                    {
                        onFail(e);
                    }
                    else
                    {
                        onSuccess(data);
                    }
                }
                catch (Exception ee)
                {
                    onException(ee);
                }
                finally
                {
                    onFinally();
                }
            };
        }

        #endregion


        #region DoInThreadPool
        /// <summary>
        /// 在线程池中做。向服务器发送Http请求，并处理返回结果
        /// </summary>
        /// <param name="delGetData"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public virtual bool DoInThreadPool(DelGetData delGetData, DelOnGetData run)
        {
            try
            {
                ThreadPool.QueueUserWorkItem((Object state) =>
                {
                    try
                    {
                        run(delGetData(), null);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            run(null, e);
                        }
                        catch { }
                    }
                });
                return true;
            }
            catch { }
            return false;
        }
        #endregion

        #region Ajax
        public virtual bool Ajax(RequestParam requestParam, DelOnGetData run)
        {
            return DoInThreadPool(
                () => { return Ajax(requestParam); }
            , run);
        }
        #endregion

        #region Ajax_DownloadFile

        /// <summary>
        /// 通过Get方法从服务器下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="filePath">要保持到本地的文件的全路径</param>
        /// <param name="run"></param>
        /// <returns></returns>
        public virtual bool Ajax_DownloadFile(RequestParam requestParam, String filePath, DelOnGetData run)
        {
            return DoInThreadPool(
                () => { Ajax_DownloadFile(requestParam, filePath); return true; }
            , run);
        }

        #endregion

        //*/

        #endregion


        /*
 规范把 HTTP 请求分为三个部分：状态行、请求头、消息主体。
  类似于下面这样：
BASH<method> <request-URL> <version>
<headers>
<entity-body>

协议规定 POST 提交的数据必须放在消息主体（entity-body）中，但协议并没有规定数据必须使用什么编码方式。实际上，开发者完全可以自己决定消息主体的格式，只要最后发送的 HTTP 请求满足上面的格式就可以。

服务端通常是根据请求头（headers）中的 Content-Type 字段来获知请求中的消息主体是用何种方式编码，再对主体进行解析。所以说到 POST 提交数据方案，包含了 Content-Type 和消息主体编码方式两部分。   

     */

    }

    #endregion


    #region class RequestParam
    /// <summary>
    /// http(s)的请求配置
    /// </summary>
    public class RequestParam
    {
        /// <summary>
        /// 可为https请求。例如： "http://www.baidu.com" "https://api.253.com/open/i/ocr/id-ocr"
        /// </summary>
        public string url;
        /// <summary>
        /// "GET"、"POST"、"DELETE" 等，默认"GET"
        /// </summary>
        public string Method;
        /// <summary>
        /// 放到url中的参数。可为string、IDictionary、JObject
        /// </summary>
        public Object urlParams;
        /// <summary>
        /// 请求实体参数（消息主体，多为post参数）。可为 byte[]、string/IDictionary(requestContentType:"application/x-www-form-urlencoded")、JObject/JArray/Object(requestContentType:"application/json")
        /// </summary>
        public Object body;
        /// <summary>
        /// 请求体的类型(若不指定则根据body类型获取)，例如"application/x-www-form-urlencoded"、"application/json"、"text/xml; charset=utf-8" 等
        /// </summary>
        public string requestContentType;
        /// <summary>
        /// 强制指定返回数据类型（可为"json"、"string",若不指定有效值则默认为"string"）
        /// </summary>
        public string responseContentType;

        #region headers
        /// <summary>
        /// http 请求头（可不指定，一般存放 Authorization Content-Type 等）
        /// </summary>
        public IDictionary headers;
        #endregion

        #region requestEncoding
        /// <summary>
        /// 
        /// </summary>
        private Encoding _requestEncoding;
        /// <summary>
        /// 请求编码
        /// </summary>
        public Encoding requestEncoding
        {
            get { return _requestEncoding ?? Encoding.Default; }
            set { _requestEncoding = value; }
        }
        #endregion


        #region responseEncoding
        /// <summary>
        /// 
        /// </summary>
        public Encoding responseEncoding;
        #endregion


        #region GetRequestBodyData
        /// <summary>
        /// 获取请求Body
        /// </summary>
        /// <returns></returns>
        internal void GetRequestBodyData(out string contentType, out  byte[] dataToWrite)
        {
            contentType = null;
            dataToWrite = null;
            if (null == body)
            {               
                return;
            }       

            switch (body)
            {
                case IDictionary param:
                    dataToWrite = requestEncoding.GetBytes(HttpUtil.FormatUrlParams(param));
                    contentType = "application/x-www-form-urlencoded";
                    break;

                case string param:
                    dataToWrite = requestEncoding.GetBytes(param);
                    contentType = "application/x-www-form-urlencoded";
                    break;

                case JObject param:
                    dataToWrite = requestEncoding.GetBytes(body.ToString());
                    contentType = "application/json";
                    break;
                case JArray param:
                    dataToWrite = requestEncoding.GetBytes(body.ToString());
                    contentType = "application/json";
                    break;

                case byte[] param:
                    dataToWrite = param;
                    break;
                case Object param:       
                    dataToWrite = requestEncoding.GetBytes(JsonConvert.SerializeObject(param));
                    contentType = "application/json";
                    break;
            }
            contentType = requestContentType ?? contentType;
        }
        #endregion
    }

    #endregion
}
