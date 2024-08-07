﻿using System.Runtime.CompilerServices;

using Sers.Core.Module.Rpc;

namespace Vit.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ECallerSourceExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnumToString(this ECallerSource data)
        {
            //return data.ToString(); 相对比较慢
            switch (data)
            {
                case ECallerSource.Internal: return "Internal";
                case ECallerSource.OutSide: return "OutSide";
            }
            return null;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ECallerSource ToECallerSource(this string data)
        {
            switch (data)
            {
                case "Internal": return ECallerSource.Internal;
                case "OutSide": return ECallerSource.OutSide;
            }
            return default;
        }

    }
}
