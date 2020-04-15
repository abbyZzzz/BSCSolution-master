using Advantech.UtilsStandard.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Advantech.CoreExtention.Http
{
    public static class HttpClientFactoryExtension
    {
        /// <summary>
        /// 注入注入HttpClientFactoryHelper
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientFactoryHelper(this IServiceCollection services)
        {
            services.AddHttpClient();         //http请求客户端
            //var httpFactory = services.BuildServiceProvider().GetService<IHttpClientFactory>();
            //var logWrite = services.BuildServiceProvider().GetService<ILogWrite>();

            //services.AddTransient<HttpClientFactoryHelper>(provider =>
            //{
            //    return new HttpClientFactoryHelper(httpFactory, logWrite);
            //});

            services.AddScoped<IHttpClientFactoryHelper,HttpClientFactoryHelper>();
            return services;
        }
    }
}
