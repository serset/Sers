using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Serialization
{
    /// <summary>
    /// 序列化字符编码。默认 UTF8。只可为 UTF7,UTF8,UTF32,ASCII,Unicode。
    /// </summary>
    public enum EEncoding
    {
        UTF7,
        UTF8,
        UTF32,
        ASCII,
        Unicode
    }
}
