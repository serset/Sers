using System;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiServiceFactory
    {
        public static Func<ILocalApiService> CreateLocalApiService = () => new LocalApiService();
    }
}
