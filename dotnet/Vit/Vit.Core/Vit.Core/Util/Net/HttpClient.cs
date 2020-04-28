using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vit.Core.Module.Serialization;
using Vit.Extensions;

namespace Vit.Core.Util.Net
{
    public class HttpClient
    {
        #region SendAsync

        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponse<T>> SendAsync<T>(HttpRequest request)
        {
            #region (x.1)构建请求            

            #region (x.x.1)创建对象           
            var url = request.url;
            if (request.urlParams != null)
            {
                url = UrlAddParams(url, request.urlParams);
            }
            HttpRequestMessage httpRequest = new HttpRequestMessage(new HttpMethod(request.httpMethod ?? "GET"), url);
            #endregion          


            #region (x.x.2)body
            if (request.body != null)
            {
                if (request.body is byte[] bytes)
                {
                    httpRequest.Content = new ByteArrayContent(bytes);
                }
                else
                {
                    var content = Serialization.Instance.SerializeToString(request.body);
                    httpRequest.Content = new StringContent(content, request.requestEncoding, "application/json");
                }
            }
            #endregion

            #region (x.x.3)headers
            if (request.headers != null)
            {
                if (httpRequest.Content != null)
                {
                    foreach (var item in request.headers)
                    {
                        httpRequest.Content.Headers.TryAddWithoutValidation(item.Key, item.Value);
                    }
                }
                else
                {
                    foreach (var item in request.headers)
                    {
                        httpRequest.Headers.TryAddWithoutValidation(item.Key, item.Value);
                        //httpRequest.Headers.Add(item.Key, item.Value);                   
                    }
                }
            }
            #endregion


            #endregion

            //(x.2)发送请求
            var response = await httpClient.SendAsync(httpRequest);

            #region (x.3)读取回应
            var httpResponse = new HttpResponse<T>();
            httpResponse.StatusCode = (int)response.StatusCode;

            httpResponse.headers = response.Content?.Headers?.AsEnumerable().ToDictionary(kv => kv.Key, kv => string.Join(",", kv.Value));

            if (response.IsSuccessStatusCode)
            {
                if (typeof(byte[]).IsAssignableFrom(typeof(T)))
                {
                    object data = await response.Content.ReadAsByteArrayAsync();
                    httpResponse.data = (T)data;
                }
                else
                {
                    string data = await response.Content.ReadAsStringAsync();
                    httpResponse.data = data.Deserialize<T>();
                }
            }
            return httpResponse;
            #endregion
        }





        #region  static UrlAddParams
        #region UrlAddParams
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



        #region UrlEncode
        /// <summary>
        ///  System.Web.HttpUtility.UrlEncode(param)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        static String UrlEncode(String param)
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


        #region FormatUrlParams

        /// <summary>
        /// 返回值demo： "a=4&b=2"
        /// </summary>
        /// <param name="parameters">可为string、IDictionary、JObject,例如："a=3&b=5"</param>
        /// <returns></returns>
        static String FormatUrlParams(Object parameters)
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
            else if (parameters is JObject jo)
            {
                return FormatJObject(jo);

            }
            else if (parameters is string)
            {
                return (string)parameters;
            }

            return FormatJObject(parameters.ConvertBySerialize<JObject>());


            #region FormatJObject
            string FormatJObject(JObject joParameters)
            {
                StringBuilder buff = new StringBuilder();
                foreach (var kv in joParameters)
                {
                    buff.Append(UrlEncode(kv.Key)).Append("=").Append(UrlEncode(kv.Value.ConvertToString())).Append("&");
                }
                if (buff.Length > 0) buff.Length--;
                return buff.ToString();
            }
            #endregion 
        }
        #endregion


        #endregion



        #endregion


        #region Send
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponse<ReturnType> Send<ReturnType>(HttpRequest request)
        {
            var task = SendAsync<ReturnType>(request);
            task.Wait();
            return task.Result;
        }

        #endregion


        #region config       
        public readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        public string BaseAddress
        {
            set
            {

                httpClient.BaseAddress = new Uri(value);
            }
        }

        public double TimeoutSeconds
        {
            set
            {

                httpClient.Timeout = TimeSpan.FromSeconds(value);
            }
        }

        #endregion
    }




    #region model

    /// <summary>
    /// http(s)的请求数据
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// 可为https请求。例如： "http://www.baidu.com" "https://api.253.com/open/i/ocr/id-ocr" "/api/getName"
        /// </summary>
        public string url;
        /// <summary>
        /// "GET"、"POST"、"DELETE" 等，默认"GET"
        /// </summary>
        public string httpMethod;
        /// <summary>
        /// 放到url中的参数。可为string、IDictionary、JObject。若为其他类型则自动转换为JObject在进行处理
        /// </summary>
        public Object urlParams;

        /// <summary>
        /// 请求正文。可为 byte[]、IDictionary、JObject/JArray/Object 等(除了byte[], requestContentType 强制为 "application/json")
        /// </summary>
        public Object body;

        /// <summary>
        /// http 请求头（可不指定，一般存放 Authorization Content-Type 等）
        /// </summary>
        public IDictionary<string, string> headers;


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

    }

    /// <summary>
    /// http(s)的回应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpResponse<T>
    {

        public T data;
        public int StatusCode = 200;

        public IDictionary<string, string> headers;
    }

    #endregion
}
