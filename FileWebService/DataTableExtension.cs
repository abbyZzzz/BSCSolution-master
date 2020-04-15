using Advantech.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.FileWebService
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
            var MediaInfoRepository = serviceProvider.GetRequiredService<IMediaInfoRepository>();//媒体信息表
            MediaInfoRepository.InitTables();
            var MediaGroupRepository = serviceProvider.GetRequiredService<IMediaGroupRepository>();//媒体群组表
            MediaGroupRepository.InitTables();
            return services;
        }
    }
}
