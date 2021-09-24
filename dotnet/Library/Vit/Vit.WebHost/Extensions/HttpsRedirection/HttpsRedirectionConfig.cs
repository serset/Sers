namespace Vit.WebHost.Extensions.HttpsRedirection
{
    public class HttpsRedirectionConfig
    {
        /// <summary>
        /// 重定向的地址。若不指定，则使用发起请求的host
        /// </summary>
        public string host;

        /// <summary>
        /// 重定向的端口号。若不指定，则使用发起请求的port
        /// </summary>
        public int? port;

        /// <summary>
        ///  The status code used for the redirect response. The default is 307.
        /// </summary>
        public int statusCode { get; set; } = 307;
    }

}
