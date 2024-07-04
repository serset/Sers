using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Vit.Extensions
{
    internal static partial class HttpRequestFeature_InitForSerslot_Extensions
    {

        internal static void InitForSerslot(this HttpRequestFeature requestFeature, string pairingToken, out FeatureCollection features)
        {

            if (requestFeature.Headers == null)
                requestFeature.Headers = new HeaderDictionary();

            //var header = "{\"Cache-Control\":\"max-age=0\",\"Connection\":\"Keep-Alive\",\"Accept\":\"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\",\"Accept-Encoding\":\"gzip, deflate\",\"Accept-Language\":\"zh-CN,zh;q=0.8\",\"Host\":\"localhost:44308\",\"User-Agent\":\"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 SE 2.X MetaSr 1.0\",\"Upgrade-Insecure-Requests\":\"1\",\"X-Forwarded-For\":\"127.0.0.1:53093\",\"X-Forwarded-Proto\":\"https\"}";
            //header = "{\"Host\":\"localhost:44308\",\"X-Forwarded-For\":\"127.0.0.1:53093\",\"X-Forwarded-Proto\":\"https\"}";


            // An item with the same key has already been added. Key: "X-Forwarded-Proto"
            if (pairingToken != null)
                requestFeature.Headers["MS-ASPNETCORE-TOKEN"] = pairingToken;
            requestFeature.Headers["X-Forwarded-Proto"] = "https";

            features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(requestFeature);

            var _responseFeature = new HttpResponseFeature() { Body = new MemoryStream() };
            features.Set<IHttpResponseFeature>(_responseFeature);
            features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(_responseFeature.Body));
        }

    }
}
