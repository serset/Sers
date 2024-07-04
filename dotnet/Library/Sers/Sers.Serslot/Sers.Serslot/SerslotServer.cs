using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;

using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.Serslot
{
    public partial class SerslotServer : IServer
    {

        public IServiceProvider serviceProvider { get; set; }



        string pairingToken;
        public void InitPairingToken(IWebHostBuilder hostBuilder)
        {
            //search "MS-ASPNETCORE-TOKEN" to know why
            string PairingToken = "TOKEN";
            pairingToken = hostBuilder.GetSetting(PairingToken) ?? Environment.GetEnvironmentVariable($"ASPNETCORE_{PairingToken}");
        }




        #region ProcessRequest

        Action<FeatureCollection> OnProcessRequest;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IFeatureCollection ProcessRequest(HttpRequestFeature requestFeature)
        {
            requestFeature.InitForSerslot(pairingToken, out var features);

            OnProcessRequest(features);

            return features;
        }

        #endregion



        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            try
            {
                #region #1 build OnProcessRequest
                void ProcessRequest(IFeatureCollection features)
                {

                    Exception _applicationException = null;

                    var httpContext = application.CreateContext(features);
                    try
                    {
                        // Run the application code for this request
                        // application.ProcessRequestAsync(httpContext).GetAwaiter().GetResult();
                        application.ProcessRequestAsync(httpContext).Wait();
                    }
                    catch (Exception ex)
                    {
                        _applicationException = ex;
                        Logger.Error(ex);
                    }
                    application.DisposeContext(httpContext, _applicationException);
                };
                OnProcessRequest = ProcessRequest;
                #endregion


                #region #2 start ServiceStation

                #region ##1 Init
                ServiceStation.ServiceStation.Init();
                Sers.Core.Module.App.SersApplication.onStop += () =>
                {
                    if (serviceProvider.GetService(typeof(IHostApplicationLifetime)) is IHostApplicationLifetime lifetime)
                    {
                        lifetime.StopApplication();
                    }
                };
                #endregion

                Logger.Info("[Serslot] Mode: BackgroundTask");

                #region ##2 load apis

                ServiceStation.ServiceStation.Instance.LoadApi();

                LoadSerslotApi(ServiceStation.ServiceStation.Instance.localApiService, Assembly.GetEntryAssembly());

                #endregion

                // ##3 Start ServiceStation
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
            }, cancellationToken);
        }

        // Ungraceful shutdown
        public void Dispose()
        {
            var cancelledTokenSource = new CancellationTokenSource();
            cancelledTokenSource.Cancel();
            StopAsync(cancelledTokenSource.Token).GetAwaiter().GetResult();
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();
    }
}
