using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

using Vit.Core.Module.Log;



namespace Vit.Extensions
{
    internal static partial class HttpRequestFeature_InitForSerslot_Extensions
    {

        static Type Type_IResponseBodyFeature = Vit.Core.Util.Reflection.ObjectLoader.GetType("Microsoft.AspNetCore.Http.Features.IHttpResponseBodyFeature", assemblyName: "Microsoft.AspNetCore.Http.Features");

        static Type Type_ResponseBodyFeature = Vit.Core.Util.Reflection.ObjectLoader.GetType("Microsoft.AspNetCore.Http.StreamResponseBodyFeature", assemblyName: "Microsoft.AspNetCore.Http");

        internal static void InitForSerslot(this HttpRequestFeature requestFeature, string pairingToken, out HttpResponseFeature _responseFeature, out FeatureCollection features)
        {

            if (requestFeature.Headers == null)
                requestFeature.Headers = new HeaderDictionary();

            //var header = "{\"Cache-Control\":\"max-age=0\",\"Connection\":\"Keep-Alive\",\"Accept\":\"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\",\"Accept-Encoding\":\"gzip, deflate\",\"Accept-Language\":\"zh-CN,zh;q=0.8\",\"Host\":\"localhost:44308\",\"User-Agent\":\"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 SE 2.X MetaSr 1.0\",\"Upgrade-Insecure-Requests\":\"1\",\"X-Forwarded-For\":\"127.0.0.1:53093\",\"X-Forwarded-Proto\":\"https\"}";
            //header = "{\"Host\":\"localhost:44308\",\"X-Forwarded-For\":\"127.0.0.1:53093\",\"X-Forwarded-Proto\":\"https\"}";


            //使用Add可能报错 An item with the same key has already been added. Key: X-Forwarded-Proto"
            //requestFeature.Headers.Add("MS-ASPNETCORE-TOKEN", pairingToken);
            //requestFeature.Headers.Add("X-Forwarded-Proto", "https");

            requestFeature.Headers["MS-ASPNETCORE-TOKEN"] = pairingToken;
            requestFeature.Headers["X-Forwarded-Proto"] = "https";

            features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(requestFeature);

            //var _responseFeature = new SerslotResponseFeature() { Body = new MemoryStream() };
            _responseFeature = new HttpResponseFeature() { Body = new MemoryStream() };
            features.Set<IHttpResponseFeature>(_responseFeature);


            //IHttpResponseBodyFeature
            if (Type_IResponseBodyFeature != null)
            {
                features[Type_IResponseBodyFeature] = Activator.CreateInstance(Type_ResponseBodyFeature, _responseFeature.Body);
            }
        }



        #region SerslotResponseFeature
        class SerslotResponseFeature : IHttpResponseFeature
        {
            public SerslotResponseFeature()
            {
                StatusCode = 200;
                Headers = new HeaderDictionary();
                Body = Stream.Null;
            }


            public int StatusCode
            {
                get;
                set;
            }

            public string ReasonPhrase
            {
                get;
                set;
            }

            public IHeaderDictionary Headers
            {
                get;
                set;
            }

            public Stream Body
            {
                get;
                set;
            }

            public virtual bool HasStarted { get; set; } = false;




            private Stack<KeyValuePair<Func<object, Task>, object>> _onStarting;
            private Stack<KeyValuePair<Func<object, Task>, object>> _onCompleted;


            #region OnStarting
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public virtual void OnStarting(Func<object, Task> callback, object state)
            {
                lock (this)
                {
                    if (HasStarted)
                    {
                        throw new InvalidOperationException(nameof(OnStarting));
                    }

                    if (_onStarting == null)
                    {
                        _onStarting = new Stack<KeyValuePair<Func<object, Task>, object>>();
                    }
                    _onStarting.Push(new KeyValuePair<Func<object, Task>, object>(callback, state));
                }
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public Task FireOnStarting()
            {
                Stack<KeyValuePair<Func<object, Task>, object>> onStarting;
                lock (this)
                {
                    onStarting = _onStarting;
                    _onStarting = null;
                }

                if (onStarting == null)
                {
                    return Task.CompletedTask;
                }
                else
                {
                    return FireOnStartingMayAwait(onStarting);
                }

            }


            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private static Task FireOnStartingMayAwait(Stack<KeyValuePair<Func<object, Task>, object>> onStarting)
            {
                try
                {
                    var count = onStarting.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var entry = onStarting.Pop();
                        var task = entry.Key.Invoke(entry.Value);
                        if (!ReferenceEquals(task, Task.CompletedTask))
                        {
                            return FireOnStartingAwaited(task, onStarting);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                return Task.CompletedTask;
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private static async Task FireOnStartingAwaited(Task currentTask, Stack<KeyValuePair<Func<object, Task>, object>> onStarting)
            {
                try
                {
                    await currentTask;

                    var count = onStarting.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var entry = onStarting.Pop();
                        await entry.Key.Invoke(entry.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            #endregion


            #region OnCompleted           
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public virtual void OnCompleted(Func<object, Task> callback, object state)
            {
                lock (this)
                {
                    if (onCompleted == null)
                    {
                        onCompleted = new Stack<KeyValuePair<Func<object, Task>, object>>();
                    }
                    onCompleted.Push(new KeyValuePair<Func<object, Task>, object>(callback, state));
                }
            }
            Stack<KeyValuePair<Func<object, Task>, object>> onCompleted = null;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public Task FireOnCompleted()
            {
                Stack<KeyValuePair<Func<object, Task>, object>> onCompleted;
                lock (this)
                {
                    onCompleted = _onCompleted;
                    _onCompleted = null;
                }

                if (onCompleted == null)
                {
                    return Task.CompletedTask;
                }

                return FireOnCompletedAwaited(onCompleted);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private static async Task FireOnCompletedAwaited(Stack<KeyValuePair<Func<object, Task>, object>> onCompleted)
            {
                foreach (var entry in onCompleted)
                {
                    try
                    {
                        await entry.Key.Invoke(entry.Value);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            #endregion
        }

        #endregion

    }
}
