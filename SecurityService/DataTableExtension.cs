using Advantech.Repository;
using Advantech.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.SecurityService
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

            var UserRepository = serviceProvider.GetRequiredService<IUserRepository>();
            UserRepository.InitTables();

            var UserGroupRepository = serviceProvider.GetRequiredService<IUserGroupRepository>();
            UserGroupRepository.InitTables();

            var SecurityInfoRepository = serviceProvider.GetRequiredService<ISecurityInfoRepository>();
            SecurityInfoRepository.InitTables();

            var UserSecurityRepository = serviceProvider.GetRequiredService<IUserSecurityRepository>();
            UserSecurityRepository.InitTables();

            return services;
        }
    }
}
