using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Api.Rpc
{
    /// <summary>
    /// 调用来源
    /// </summary>
    public enum ECallerSource:byte
    {
        /// <summary>
        /// 内部调用
        /// </summary>
        Internal,
        /// <summary>
        /// 外部调用(通过网关调用)
        /// </summary>
        Outside
    }
}
