using Advantech.SecurityService.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thrift;
using static Advantech.SecurityService.Rpc.ThriftRpcTokenService;

namespace Advantech.SecurityService.Rpc
{
    public static class ThriftRpcServiceExtention
    {
        public static IServiceCollection AddThriftRpc(this IServiceCollection services)
        {
            services.AddScoped<IAsync, RpcTokenService>();
            var asyncService = services.BuildServiceProvider().GetRequiredService<IAsync>();
            //注入rpc服务实现实例
            services.AddSingleton<ITAsyncProcessor, AsyncProcessor>(provider =>
            {
                //var asyncService = provider.GetService<IAsync>();//不能在此获取，因为跨scope了
                return new AsyncProcessor(asyncService);
            });
            //监听rpc端口
            services.AddHostedService<RpcServiceHost>();
            return services;
        }
    }
}
