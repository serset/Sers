using Sers.Core.Module.Api.Data;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.Common;
using Sers.Core.Util.Ioc;

namespace App.Ioc.Station.Controllers.Demo
{


    public class DemoController : IApiController
    {


        /// <summary>
        /// Demo1-注释
        /// </summary>
        /// <returns>ArgModelDesc-returns</returns>
        [SsRoute("demo")] 
        public ApiReturn<object> Demo1()
        {

            return new {
                Singleton1 = IocHelp.Create<ISingleton>(),
                Singleton2 = IocHelp.Create<ISingleton>(),

                Scoped1 = IocHelp.Create<IScoped>(),
                Scoped2 = IocHelp.Create<IScoped>(),

                Transient1 = IocHelp.Create<ITransient>(),
                Transient2 = IocHelp.Create<ITransient>(),
            };
        }


        public interface ISingleton { }
        public interface IScoped { }
        public interface ITransient { }

        public class ArgModel: IScoped, ITransient, ISingleton
        {
            /// <summary>
            ///  
            /// </summary> 
            public string guid { get; set; } = CommonHelp.NewGuid();

        }
        
    }
}
