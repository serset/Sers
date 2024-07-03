namespace Vit.Core.Util.Threading.Worker
{
    public enum ETaskFinishStatus : byte
    {
        success,
        error,
        timeout,
        /// <summary>
        /// 服务超载（一般为瞬时请求量过大）
        /// </summary>
        overload
    }
}
