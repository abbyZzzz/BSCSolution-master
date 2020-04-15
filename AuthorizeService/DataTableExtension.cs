using Advantech.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.AuthenticationService
{
    public static class DataTableExtension
    {
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInitDataTable(this IServiceCollection services)
        {
            var UserRepository = services.BuildServiceProvider().GetRequiredService<IUserRepository>();
            UserRepository.InitTables();
            
            var UserGroupRepository = services.BuildServiceProvider().GetRequiredService<IUserGroupRepository>();
            UserGroupRepository.InitTables();

            var SecurityInfoRepository = services.BuildServiceProvider().GetRequiredService<ISecurityInfoRepository>();
            SecurityInfoRepository.InitTables();

            var UserSecurityRepository = services.BuildServiceProvider().GetRequiredService<IUserSecurityRepository>();
            UserSecurityRepository.InitTables();

            return services;
        }
    }
}
