/*
 * Licensed to the OpenSkywalking under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using Microsoft.Extensions.DependencyInjection;
using SkyWalking.Config;
using SkyWalking.Utilities.Configuration;
using SkyWalking.Agent.AspNetCore;

namespace Sers.Apm.SkyWalking.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSersSkyWalking(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSkyWalkingCore();

            services.AddSingleton<IConfigAccessor, Sers.Apm.SkyWalking.Core.Base.ConfigAccessor>();


            //services.AddSingleton<IContextCarrierFactory, ContextCarrierFactory>();
            //services.AddSingleton<ITraceDispatcher, AsyncQueueTraceDispatcher>();
            //services.AddSingleton<IExecutionService, TraceSegmentTransportService>();
            //services.AddSingleton<IExecutionService, ServiceDiscoveryService>();
            //services.AddSingleton<IExecutionService, SamplingRefreshService>();
            //services.AddSingleton<ISkyWalkingAgentStartup, SkyWalkingAgentStartup>();
            //services.AddSingleton<ISampler>(DefaultSampler.Instance);
            //services.AddSingleton(RuntimeEnvironment.Instance);
            //services.AddSingleton<TracingDiagnosticProcessorObserver>();
            //services.AddSingleton<IConfigAccessor, ConfigAccessor>();
            //services.AddSingleton<IHostedService, InstrumentationHostedService>();
            //services.AddSingleton<IEnvironmentProvider, HostingEnvironmentProvider>();
            //services.AddGrpcTransport().AddLogging();
            //services.AddSkyWalkingExtensions().AddAspNetCoreHosting().AddHttpClient().AddSqlClient().AddEntityFrameworkCore(c => c.AddPomeloMysql().AddNpgsql().AddSqlite());
            return services;
        }

        //private static IServiceCollection AddGrpcTransport(this IServiceCollection services)
        //{
        //    services.AddSingleton<ISkyWalkingClient, GrpcClient>();
        //    services.AddSingleton<ConnectionManager>();
        //    services.AddSingleton<IExecutionService, GrpcStateCheckService>();
        //    return services;
        //}
        
        //private static IServiceCollection AddLogging(this IServiceCollection services)
        //{
        //    services.AddSingleton<ILoggerFactory, DefaultLoggerFactory>();
        //    return services;
        //}
    }
}