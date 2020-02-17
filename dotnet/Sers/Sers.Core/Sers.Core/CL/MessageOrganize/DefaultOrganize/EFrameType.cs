namespace Sers.Core.CL.MessageOrganize.DefaultOrganize
{
    public enum EFrameType : byte
    {
        /// <summary>
        /// request
        /// </summary>
        request = 1,
        /// <summary>
        /// reply
        /// </summary>
        reply = 2,
        /// <summary>
        /// 单向数据
        /// </summary>
        message=3
    }
}
