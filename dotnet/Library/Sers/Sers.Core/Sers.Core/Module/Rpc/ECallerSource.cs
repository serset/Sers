using System;

namespace Sers.Core.Module.Rpc
{
    /// <summary>
    /// 调用来源
    /// </summary>
    [Flags]
    public enum ECallerSource : byte
    {
        /// <summary>
        /// 内部调用
        /// </summary>
        Internal = 1,
        /// <summary>
        /// 外部调用(通过网关调用)
        /// </summary>
        OutSide = 2
    }
}
