using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib;
using Advantech.UtilsStandardLib.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Advantech.CoreExtention
{
    /// <summary>
    /// net core mvc 容器DI
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 单个服务注册DI
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="injection"></param>
        public static void Register<TService, TImplementation>(IServiceCollection services, ServiceLifetime injection = ServiceLifetime.Scoped)
        {
            //ServiceCollectionDescriptorExtensions
            switch (injection)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(TService), typeof(TImplementation));
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(TService), typeof(TImplementation));
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(TService), typeof(TImplementation));
                    break;
            }
        }
        /// <summary>
        ///根据生成的dll注册服务
        /// </summary>
        /// <param name="service">IServiceCollection</param>
        /// <param name="assemblyName">程序集的名称</param>
        /// <param name="injection">生命周期</param>
        public static void RegisterAssembly(IServiceCollection services, string assemblyName, ServiceLifetime injection = ServiceLifetime.Scoped)
        {
            CheckNull.ArgumentIsNullException(services, nameof(services));
            CheckNull.ArgumentIsNullException(assemblyName, nameof(assemblyName));
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            if (assembly.IsNullT())
            {
                throw new DllNotFoundException($"\"{assemblyName}\".dll不存在");
            }
            var types = assembly.GetTypes().Where(x => (typeof(IDependency).IsAssignableFrom(x)) &&
                                                  !x.IsInterface).ToList();
            //&& !o.Name.Contains("Base")
            foreach (var type in types)
            {
                var faces = type.GetInterfaces().Where(o => o.Name != "IDependency" && !o.Name.Contains("Base")).ToArray();
                if (faces.Any())
                {
                    var interfaceType = faces.LastOrDefault();//.FirstOrDefault();
                    switch (injection)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(interfaceType, type);
                            if(faces.Count()>2)//存在多个注册
                            {
                                services.AddScoped(faces[faces.Count()-2], type);
                            }
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton(interfaceType, type);
                            if (faces.Count() > 2)//存在多个注册
                            {
                                services.AddScoped(faces[faces.Count() - 2], type);
                            }
                            break;

                        case ServiceLifetime.Transient:
                            services.AddTransient(interfaceType, type);
                            if (faces.Count() > 2)//存在多个注册
                            {
                                services.AddScoped(faces[faces.Count() - 2], type);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 根据生成的dll注册服务
        /// </summary>
        /// <param name="service">IServiceCollection</param>
        /// <param name="assemblyName">dll数组</param>
        /// <param name="injection">生命周期</param>
        public static void RegisterAssembly(IServiceCollection services, string[] assemblyName, ServiceLifetime injection = ServiceLifetime.Singleton)
        {
            CheckNull.ArgumentIsNullException(services, nameof(services));
            CheckNull.ArgumentIsNullException(assemblyName, nameof(assemblyName));
            foreach (var item in assemblyName)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(item));
                if (assembly.IsNullT())
                {
                    throw new DllNotFoundException($"\"{assemblyName}\".dll不存在");
                }
                var types = assembly.GetTypes().Where(o =>
                (typeof(IDependency).IsAssignableFrom(o)) && !o.IsInterface).ToList();
                foreach (var type in types)
                {
                    var faces = type.GetInterfaces().Where(o => o.Name != "IDependency" && !o.Name.Contains("Base")).ToArray();
                    if (faces.Any())
                    {
                        var interfaceType = faces.FirstOrDefault();
                        switch (injection)
                        {
                            case ServiceLifetime.Scoped:
                                services.AddScoped(interfaceType, type);
                                break;

                            case ServiceLifetime.Singleton:
                                services.AddSingleton(interfaceType, type);
                                break;

                            case ServiceLifetime.Transient:
                                services.AddTransient(interfaceType, type);
                                break;
                        }
                    }
                }
            }
        }
    }
}
