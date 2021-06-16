using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Vit.Extensions
{
    public static class HttpRequestExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path.Value)
                .Append(request.QueryString.Value)
                .ToString();
        }
    }
}
