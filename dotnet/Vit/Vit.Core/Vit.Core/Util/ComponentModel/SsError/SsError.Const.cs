namespace Vit.Core.Util.ComponentModel.SsError
{
    public partial class SsError
    {



        /// <summary>
        /// 100 系统出错
        /// </summary>
        public static readonly SsError Err_SysErr = new SsError { errorCode = 100, errorMessage = "系统出错" };


        /// <summary>
        /// 101 请求的api不存在
        /// </summary>
        public static readonly SsError Err_ApiNotExists = new SsError { errorCode = 101, errorMessage = "请求的api不存在" };


        /// <summary>
        /// 102 请求超时
        /// </summary>
        public static readonly SsError Err_Timeout = new SsError { errorCode = 102, errorMessage = "请求超时" };


        /// <summary>
        /// 103 操作超时被强制中断
        /// </summary>
        public static readonly SsError Err_HandleTimeout = new SsError { errorCode = 103, errorMessage = "操作超时被强制中断" };

        /// <summary>
        /// 110 服务限流限制
        /// </summary>
        public static readonly SsError Err_RateLimit_Refuse = new SsError { errorCode = 110, errorMessage = "服务限流限制" };

        /// <summary>
        /// 120 请求参数不合法
        /// </summary>
        public static readonly SsError Err_InvalidParam = new SsError { errorCode = 120, errorMessage = "请求参数不合法" };



        /// <summary>
        /// 404 404 Not Found：请求资源不存在
        /// </summary>
        public static readonly SsError Err_404 = new SsError { errorCode = 404, errorMessage = "404 Not Found：请求资源不存在" };

        /// <summary>
        /// 405 权限限制(没有权限)
        /// </summary>
        public static readonly SsError Err_NotAllowed = new SsError { errorCode = 405, errorMessage = "权限限制(没有相应权限)" };



        




    }
}
