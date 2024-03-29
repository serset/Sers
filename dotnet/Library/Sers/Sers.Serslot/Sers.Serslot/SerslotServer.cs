using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.Serslot
{
    public partial class SerslotServer : IServer
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider serviceProvider { get; set; }

        #region PairingToken       
        string pairingToken;
        public void InitPairingToken(IWebHostBuilder hostBuilder)
        {
            //search "MS-ASPNETCORE-TOKEN" to know why
            string PairingToken = "TOKEN";
            pairingToken = hostBuilder.GetSetting(PairingToken) ?? Environment.GetEnvironmentVariable($"ASPNETCORE_{PairingToken}");

        }
        #endregion



        #region ProcessRequest       

        Action<FeatureCollection> OnProcessRequest;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IHttpResponseFeature ProcessRequest(HttpRequestFeature requestFeature)
        {
            requestFeature.InitForSerslot(pairingToken, out var _responseFeature, out var features);

            OnProcessRequest(features);

            return _responseFeature;
        } 

        #endregion

 


        public IFeatureCollection Features { get; } = new FeatureCollection();


        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            try
            {
                #region (x.1) build OnProcessRequest               
                OnProcessRequest = (features) =>
                {

                    Exception _applicationException = null;

                    var httpContext = application.CreateContext(features);
                    try
                    {

                        //var httpContext_ = httpContext.GetProperty<object>("HttpContext");
                        //if (httpContext_ is Microsoft.AspNetCore.Http.HttpContext defaultHttpContext)
                        //{
                        //    //if (defaultHttpContext.Response.Body == null)                            
                        //    defaultHttpContext.Response.Body = features.Get<IHttpResponseFeature>().Body;                           
                        //}




                        // Run the application code for this request
                        // application.ProcessRequestAsync(httpContext).GetAwaiter().GetResult();
                        application.ProcessRequestAsync(httpContext).Wait();


                        //var _responseFeature = features.Get<IHttpResponseFeature>() as SerslotResponseFeature;
                        //if (_responseFeature != null)
                        //{                           
                        //    _responseFeature.FireOnStarting();
                        //    _responseFeature.FireOnCompleted();
                        //}
                    }
                    catch (Exception ex)
                    {
                        _applicationException = ex;
                        Logger.Error(ex);
                    }
                    application.DisposeContext(httpContext, _applicationException);
                };
                #endregion


                #region (x.2) start ServiceStation                

                #region (x.x.1) Init
                ServiceStation.ServiceStation.Init();
                Sers.Core.Module.App.SersApplication.onStop += () =>
                {
                    if (serviceProvider.GetService(typeof(IApplicationLifetime)) is IApplicationLifetime lifetime)
                    {
                        lifetime.StopApplication();
                    }
                };
                #endregion

                Logger.Info("[Serslot] Mode: BackgroundTask");

                #region (x.x.2)����api           

                ServiceStation.ServiceStation.Instance.LoadApi();

                LoadSerslotApi(ServiceStation.ServiceStation.Instance.localApiService, Assembly.GetEntryAssembly());

                #endregion

                //(x.x.3)Start ServiceStation
                if (!ServiceStation.ServiceStation.Start())
                {
                    Dispose();
                }
                #endregion

            }
            catch
            {
                Dispose();
                throw;
            }
        }

        // Graceful shutdown if possible
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                try
                {
                    ServiceStation.ServiceStation.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        // Ungraceful shutdown
        public void Dispose()
        {
            var cancelledTokenSource = new CancellationTokenSource();
            cancelledTokenSource.Cancel();
            StopAsync(cancelledTokenSource.Token).GetAwaiter().GetResult();
        }

    }
}
