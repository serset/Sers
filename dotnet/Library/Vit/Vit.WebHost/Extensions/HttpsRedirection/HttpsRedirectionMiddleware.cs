using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using System.Threading.Tasks;

namespace Vit.WebHost.Extensions.HttpsRedirection
{
    public class HttpsRedirectionMiddleware
    {

        private readonly HttpsRedirectionConfig config;

        private readonly RequestDelegate _next;

        public HttpsRedirectionMiddleware(RequestDelegate next, HttpsRedirectionConfig config)
        {
            _next = next;
            this.config = config;
        }


        public Task Invoke(HttpContext context)
        {
            if (context.Request.IsHttps)
            {
                return _next(context);
            }

            HttpRequest request = context.Request;

            var host = config.host ?? request.Host.Host;
            var port = config.port ?? request.Host.Port ?? 443;

            string text = UriHelper.BuildAbsolute("https", new HostString(host, port), request.PathBase, request.Path, request.QueryString);

            context.Response.StatusCode = config.statusCode;
            context.Response.Headers["Location"] = text;

            return Task.CompletedTask;
        }
    }
}
