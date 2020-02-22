using Sers.ApiLoader.Sers;
using Sers.ApiLoader.Sers.Attribute;
using Vit.Core.Util.Common;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Ioc;

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
