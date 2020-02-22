using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vit.Ioc
{
    public static class IocHelp
    {         
        public static readonly Ioc Instance = new Ioc();


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
        public static void AddTransient(Type serviceType, Type implementationType)
        {
            Instance.AddTransient(serviceType, implementationType);
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
        public static void AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            Instance.AddTransient(serviceType, implementationFactory);
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
        public static void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Instance.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public static void AddTransient(Type serviceType)
        {
            Instance.AddTransient(serviceType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public static void AddTransient<TService>() where TService : class
        {
            Instance.AddTransient<TService>();
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
        public static void AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            Instance.AddTransient<TService>(implementationFactory);
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
        public static void AddTransient<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        {
            Instance.AddTransient<TService, TImplementation>(implementationFactory);
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
        public static void AddScoped(Type serviceType, Type implementationType)
        {
            Instance.AddScoped(serviceType, implementationType);
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
        public static void AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            Instance.AddScoped(serviceType, implementationFactory);
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
        public static void AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Instance.AddScoped<TService, TImplementation>();
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public static void AddScoped(Type serviceType)
        {
            Instance.AddScoped(serviceType);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        public static void AddScoped<TService>() where TService : class
        {
            Instance.AddScoped<TService>();
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
        public static void AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            Instance.AddScoped<TService>(implementationFactory);
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
        public static void AddScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        {
            Instance.AddScoped<TService, TImplementation>(implementationFactory);
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
        public static void AddSingleton(Type serviceType, Type implementationType)
        {
            Instance.AddSingleton(serviceType, implementationType);
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
        public static void AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            Instance.AddSingleton(serviceType, implementationFactory);
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
        public static void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Instance.AddSingleton<TService, TImplementation>();
        }


        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public static void AddSingleton(Type serviceType)
        {
            Instance.AddSingleton(serviceType);
        }


        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public static void AddSingleton<TService>()
            where TService : class
        {
            Instance.AddSingleton<TService>();
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
        public static void AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            Instance.AddSingleton<TService>(implementationFactory);
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
        public static void AddSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        {
            Instance.AddSingleton<TService, TImplementation>(implementationFactory);
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
        public static void AddSingleton(Type serviceType, object implementationInstance)
        {
            Instance.AddSingleton(serviceType, implementationInstance);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> with an
        /// instance specified in <paramref name="implementationInstance" /> to the
        /// specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="implementationInstance">The instance of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />
        public static void AddSingleton<TService>(TService implementationInstance) where TService : class
        {
            Instance.AddSingleton<TService>(implementationInstance);
        }
        #endregion


        #region Create

        /// <summary>
        /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T" /> or null if there is no such service.</returns>
        public static T Create<T>()
        {
            return Instance.Create<T>();
        }


        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType">serviceType</paramref>.  
        ///  -or-  
        ///  null if there is no service object of type <paramref name="serviceType">serviceType</paramref>.</returns>
        public static object Create(Type serviceType)
        {
            return Instance.Create(serviceType);
        }
        #endregion

        #region AutoCreate

        /// <summary>
        /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// if have not yet registered Type T in Ioc,then return new T();
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T" /> or null if there is no such service.</returns>
        public static T AutoCreate<T>() where T : class, new()
        {
            return Instance.AutoCreate<T>();
        }




        /// <summary>
        /// Get service of type <typeparamref name="TService" /> from the <see cref="TService:System.IServiceProvider" />.
        /// if have not yet registered Type TService in Ioc,then return new TImplementation();
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>      
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        public static TService AutoCreate<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService, new()
        {
            return Instance.AutoCreate<TService, TImplementation>();
        }
        #endregion


        #region SetServiceCollection
        public static void SetServiceCollection(IServiceCollection serviceCollection=null)
        {
            Instance.SetServiceCollection(serviceCollection);
        }
        #endregion


        #region CreateScope
        public static IServiceScope CreateScope()
        {
            return Instance.CreateScope();
        }
        #endregion

        #region Update       
        /// <summary>
        ///  请在注入后手动调用，否则所做的注入可能无效
        /// </summary>
        /// <returns></returns>
        public static void Update()
        {
            Instance.Update();
        }
        #endregion



        #region Contains
        /// <summary>
        /// 检测是否有过注册
        /// </summary>
        /// <param name="serviceType">The <see cref="T:System.Type" /> of the service.</param>
        /// <param name="lifetime">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceLifetime" /> of the service.</param>
        public static bool Contains(Type serviceType, ServiceLifetime lifetime)
        {
            return Instance.Contains(serviceType, lifetime);
        }

        /// <summary>
        /// 检测是否有过注册
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="lifetime">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceLifetime" /> of the service.</param>
        public static bool Contains<TService>(ServiceLifetime lifetime)
        {
            return Instance.Contains<TService>(lifetime);
        }
        #endregion

    }
}
