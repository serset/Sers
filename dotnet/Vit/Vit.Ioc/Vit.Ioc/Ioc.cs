using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vit.Ioc
{
    public class Ioc
    {

        #region AddTransient
        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType" /> with an
        /// implementation of the type specified in <paramref name="implementationType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient(Type serviceType, Type implementationType)
        {
            rootServiceCollection.AddTransient(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType" /> with a
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            rootServiceCollection.AddTransient(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService" /> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>      
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            rootServiceCollection.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient(Type serviceType)
        {
            rootServiceCollection.AddTransient(serviceType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient<TService>() where TService : class
        {
            rootServiceCollection.AddTransient<TService>();
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService" /> with a
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            rootServiceCollection.AddTransient<TService>(implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService" /> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> using the
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public void AddTransient<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        {
            rootServiceCollection.AddTransient<TService, TImplementation>(implementationFactory);
        }


        #endregion

        #region AddScoped

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType" /> with an
        /// implementation of the type specified in <paramref name="implementationType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped(Type serviceType, Type implementationType)
        {          
            rootServiceCollection.AddScoped(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType" /> with a
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            rootServiceCollection.AddScoped(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService" /> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            rootServiceCollection.AddScoped<TService, TImplementation>();
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped(Type serviceType)
        {
            rootServiceCollection.AddScoped(serviceType);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped<TService>() where TService : class
        {
            rootServiceCollection.AddScoped<TService>();
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService" /> with a
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            rootServiceCollection.AddScoped<TService>(implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService" /> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> using the
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public void AddScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        {
            rootServiceCollection.AddScoped<TService, TImplementation>(implementationFactory);
        }




        #endregion

        #region AddSingleton

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType" /> with an
        /// implementation of the type specified in <paramref name="implementationType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton(Type serviceType, Type implementationType)
        {
            rootServiceCollection.AddSingleton(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType" /> with a
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            rootServiceCollection.AddSingleton(serviceType, implementationFactory);
        }


        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            rootServiceCollection.AddSingleton<TService, TImplementation>();
        }


        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton(Type serviceType)
        {
            rootServiceCollection.AddSingleton(serviceType);
        }


        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton<TService>()
            where TService : class
        {
            rootServiceCollection.AddSingleton<TService>();
        }


        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> with a
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            rootServiceCollection.AddSingleton<TService>(implementationFactory);
        }




        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> using the
        /// factory specified in <paramref name="implementationFactory" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        {
            rootServiceCollection.AddSingleton<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType" /> with an
        /// instance specified in <paramref name="implementationInstance" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationInstance">The instance of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton(Type serviceType, object implementationInstance)
        {
            rootServiceCollection.AddSingleton(serviceType, implementationInstance);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> with an
        /// instance specified in <paramref name="implementationInstance" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="implementationInstance">The instance of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public void AddSingleton<TService>(TService implementationInstance) where TService : class
        {
            rootServiceCollection.AddSingleton<TService>(implementationInstance);
        }
        #endregion


        #region Create


        /// <summary>
        /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T" /> or null if there is no such service.</returns>
        public T Create<T>()
        {
            //lock (this)
            //{                 
                return CurServiceProvider.GetService<T>();
            //}
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType">serviceType</paramref>.  
        ///  -or-  
        ///  null if there is no service object of type <paramref name="serviceType">serviceType</paramref>.</returns>
        public object Create (Type serviceType) 
        {
            //lock (this)
            //{
                return CurServiceProvider.GetService(serviceType);
            //}
        }
        #endregion


        #region AutoCreate

        /// <summary>
        /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// if have not yet registered Type T in Ioc,then return new T();
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T" /> or null if there is no such service.</returns>
        public T AutoCreate<T>()where T: class,new()
        {
            //lock (this)
            //{
                return Create<T>() ?? new T();
            //}
        }


        /// <summary>
        /// Get service of type <typeparamref name="TService" /> from the <see cref="TService:System.IServiceProvider" />.
        /// if have not yet registered Type TService in Ioc,then return new TImplementation();
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>      
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public TService AutoCreate<TService, TImplementation>() 
            where TService : class 
            where TImplementation : class, TService, new()        
        {
            //lock (this)
            //{
                return Create<TService>() ?? new TImplementation();
            //}
        }
        #endregion




        #region 逻辑        


        #region 成员变量

        #region AsyncCache

        #region class AsyncCache<T>      
        /// <summary>
        /// 多包裹一层的原因是 子异步任务结束时会还原子异步任务对AsyncLocal做的更改(即子异步任务对AsyncLocal做的更改不会保留到子异步任务结束后的父异步任务中)
        /// 参见https://blog.csdn.net/kkfd1002/article/details/80102244
        /// </summary>
        class AsyncCache<T>
        {

            readonly System.Threading.AsyncLocal<CachedData> _AsyncLocal = new System.Threading.AsyncLocal<CachedData>();

            class CachedData
            {
                public T Cache;
            }
            public T Value
            {
                get
                {
                    if (null == _AsyncLocal.Value)
                        return default(T);
                    return _AsyncLocal.Value.Cache;
                }
                set
                {
                    var asyncLocal = _AsyncLocal.Value;
                    if (null == asyncLocal) asyncLocal = _AsyncLocal.Value = new CachedData();
                    asyncLocal.Cache = value;
                }
            }
        }
        #endregion




        readonly AsyncCache<ScopeCache> _AsyncCache = new AsyncCache<ScopeCache>();   

        ScopeCache AsyncScopeCache
        {
            get
            {
                return _AsyncCache.Value;
            }
            set
            {
                _AsyncCache.Value = value;
            }
        }

        #endregion


        public IServiceCollection rootServiceCollection { get; private set; }
  
        private IServiceProvider _rootServiceProvider;

        /// <summary>
        /// 请勿轻易设置，当serviceProvider为空时会自动创建
        /// </summary>
        public IServiceProvider rootServiceProvider
        {
            get => (_rootServiceProvider ?? (_rootServiceProvider = rootServiceCollection.BuildServiceProvider()));
            set => _rootServiceProvider = value;
        }



        IServiceScope CurScope => (AsyncScopeCache?.Scope);
        IServiceProvider CurServiceProvider => (AsyncScopeCache?.ServiceProvider) ?? (rootServiceProvider);
        #endregion


        #region class ScopeCache


        class ScopeCache : IDisposable
        {
            public IServiceProvider ServiceProvider = null;

            public IServiceScope Scope = null;

            Ioc ioc = null;
            ScopeCache parent = null;

            public void SetToLocal(Ioc ioc, IServiceScope Scope, IServiceProvider ServiceProvider)
            {
                this.ioc = ioc;
                this.Scope = Scope;
                this.ServiceProvider = ServiceProvider;
                lock (ioc)
                {
                    parent = ioc.AsyncScopeCache;
                    ioc.AsyncScopeCache = this;
                }
            }

            public void Dispose()
            {
                lock (ioc)
                {
                    //need judje?
                    if (ioc.AsyncScopeCache == this)
                    {
                        ioc.AsyncScopeCache = parent;
                    }
                    else
                    {                        
                        throw new Exception($"did not call IServiceScope.Dispose in order[Vit.Core.Util.Ioc,{ nameof(Ioc) }.cs,Line {GetFileLineNumber()}]");

                        #region GetFileLineNumber
                        int GetFileLineNumber()
                        {
                            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
                            return st.GetFrame(0).GetFileLineNumber();
                        }                       
                        #endregion

                    }
                }
            }
        }
        #endregion


        public Ioc(IServiceCollection serviceCollection = null)
        {
            SetServiceCollection(serviceCollection);
            AddScoped<ScopeCache>();
        }

        public void SetServiceCollection(IServiceCollection serviceCollection = null)
        {
            rootServiceCollection = serviceCollection ?? new ServiceCollection();
            _rootServiceProvider = null;
        }
 

        public IServiceScope CreateScope()
        {
            var scope = CurServiceProvider.CreateScope();
            try
            {
                var provider = scope.ServiceProvider;
                var scopeData = provider.GetService<ScopeCache>();
                scopeData.SetToLocal(this, scope, provider);
                return scope;
            }
            catch (Exception)
            {
                scope.Dispose();
                throw;
            }
        }



        /// <summary>
		///  请在注入后手动调用，否则所做的注入可能无效
		/// </summary>
		/// <returns></returns>
		public void Update()
        {
            _rootServiceProvider = null;
        }
        #endregion



        #region GetServiceDescriptor(s)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public List<ServiceDescriptor> GetServiceDescriptors(Type serviceType, ServiceLifetime lifetime)
        {
            var list = from serviceDescriptor in rootServiceCollection
                       where serviceDescriptor.Lifetime == lifetime && serviceDescriptor.ServiceType == serviceType
                       select serviceDescriptor;
            return list.ToList();
        }


        public ServiceDescriptor GetServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            var list = GetServiceDescriptors(serviceType, lifetime);

            return list.LastOrDefault();
            //var lasts = list.TakeLast(1).ToList();
            //if (1 == lasts.Count)
            //{
            //    return lasts[0];
            //}
            //return null;
        }
        #endregion

        #region Contains
        /// <summary>
        /// 检测是否有过注册
        /// </summary>
        /// <param name="serviceType">The <see cref="T:System.Type" /> of the service.</param>
        /// <param name="lifetime">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceLifetime" /> of the service.</param>
        public bool Contains(Type serviceType, ServiceLifetime lifetime)
        {
            return 0 < GetServiceDescriptors(serviceType, lifetime).Count;
        }

        /// <summary>
        /// 检测是否有过注册
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="lifetime">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceLifetime" /> of the service.</param>
        public bool Contains<TService>(ServiceLifetime lifetime)
        {
            return Contains(typeof(TService), lifetime);
        }
        #endregion


    }
}
