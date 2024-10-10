namespace Vit.Core.Util.ComponentModel.SsError
{
    public partial class SsError
    {

        /// <summary>
        /// 400 Bad Request.
        /// The server cannot or will not process the request due to something that is perceived to be a client error.
        /// </summary>
        public static readonly SsError Err_400BadRequest = new SsError { errorCode = 400, errorMessage = "Bad Request" };


        /// <summary>
        /// 401 Unauthorized.
        /// Although the HTTP standard specifies "unauthorized", semantically this response means "unauthenticated". 
        /// </summary>
        public static readonly SsError Err_401Unauthorized = new SsError { errorCode = 401, errorMessage = "Unauthorized" };


        /// <summary>
        /// 403 Forbidden.
        /// The client does not have access rights to the content.
        /// </summary>
        public static readonly SsError Err_403Forbidden = new SsError { errorCode = 403, errorMessage = "Forbidden" };

        /// <summary>
        /// 404 Not Found.
        /// The server cannot find the requested resource. In the browser, this means the URL is not recognized. 
        /// </summary>
        public static readonly SsError Err_404NotFound = new SsError { errorCode = 404, errorMessage = "Not Found" };



        /// <summary>
        /// 405 Method Not Allowed.
        /// The request method is known by the server but is not supported by the target resource.
        /// </summary>
        public static readonly SsError Err_405MethodNotAllowed = new SsError { errorCode = 405, errorMessage = "Method Not Allowed" };



        /// <summary>
        /// 408 Request Timeout.
        /// This response is sent on an idle connection by some servers, even without any previous request by the client. 
        /// </summary>
        public static readonly SsError Err_408RequestTimeout = new SsError { errorCode = 408, errorMessage = "Request Timeout" };



        /// <summary>
        /// 429 Too Many Requests.
        /// The user has sent too many requests in a given amount of time ("rate limiting"). 
        /// </summary>
        public static readonly SsError Err_429TooManyRequests = new SsError { errorCode = 429, errorMessage = "Too Many Requests" };


        /// <summary>
        /// 500 Internal Server Error.
        /// The server has encountered a situation it does not know how to handle.
        /// </summary>
        public static readonly SsError Err_500InternalServerError = new SsError { errorCode = 500, errorMessage = "Internal Server Error" };



        /// <summary>
        /// 501 Not Implemented.
        /// The request method is not supported by the server and cannot be handled.
        /// </summary>
        public static readonly SsError Err_501NotImplemented = new SsError { errorCode = 501, errorMessage = "Not Implemented" };


        /// <summary>
        /// 503 Service Unavailable.
        /// The server is not ready to handle the request. Common causes are a server that is down for maintenance or that is overloaded.
        /// </summary>
        public static readonly SsError Err_503ServiceUnavailable = new SsError { errorCode = 503, errorMessage = "Service Unavailable" };


        /// <summary>
        /// 404 Not Found.
        /// The server cannot find the requested resource. In the browser, this means the URL is not recognized. 
        /// </summary>
        public static SsError Err_ApiNotExists => Err_404NotFound;
        /// <summary>
        /// 405 Method Not Allowed.
        /// The request method is known by the server but is not supported by the target resource.
        /// </summary>
        public static SsError Err_InvalidParam => Err_405MethodNotAllowed;

        /// <summary>
        /// 408 Request Timeout.
        /// This response is sent on an idle connection by some servers, even without any previous request by the client. 
        /// </summary>
        public static SsError Err_Timeout => Err_408RequestTimeout;

        /// <summary>
        /// 500 Internal Server Error.
        /// The server has encountered a situation it does not know how to handle.
        /// </summary>
        public static SsError Err_SysErr => Err_500InternalServerError;
        /// <summary>
        /// 429 Too Many Requests.
        /// The user has sent too many requests in a given amount of time ("rate limiting"). 
        /// </summary>
        public static SsError Err_RateLimit_Refuse => Err_429TooManyRequests;



    }
}
