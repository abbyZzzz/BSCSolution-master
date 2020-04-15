using Advantech.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.BSCWeb
{
    public static class DataTableExtension
    {
        /// <summary>
        /// 自动创建数据表
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInitDataTable(this IServiceCollection services)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var UserRepository = serviceProvider.GetRequiredService<ISystemLogRepository>();//系统运行日志
            UserRepository.InitTables();

            return services;
        }
    }
}
