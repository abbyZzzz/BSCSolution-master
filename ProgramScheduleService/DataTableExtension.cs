using Advantech.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.ProgramScheduleService
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
            var ProgramGroupRepository = serviceProvider.GetRequiredService<IProgramGroupRepository>();//节目群组表
            ProgramGroupRepository.InitTables();
            var ProgramInfoRepository = serviceProvider.GetRequiredService<IProgramInfoRepository>();//节目信息表
            ProgramInfoRepository.InitTables();
            var ProgramRegionRepository = serviceProvider.GetRequiredService<IProgramRegionRepository>();//区块信息表
            ProgramRegionRepository.InitTables();
            var ProgramRegionMediaRepository = serviceProvider.GetRequiredService<IProgramRegionMediaRepository>();//区块所属媒体表
            ProgramRegionMediaRepository.InitTables();
            var ScheduleDayRepository = serviceProvider.GetRequiredService<IScheduleDayRepository>();//单日时刻表列表
            ScheduleDayRepository.InitTables();
            var ScheduleDayProgramRepository = serviceProvider.GetRequiredService<IScheduleDayProgramRepository>();//单日时刻表管理
            ScheduleDayProgramRepository.InitTables();
            var ProgramScheduleRepository = serviceProvider.GetRequiredService<IProgramScheduleRepository>();//播放排程列表
            ProgramScheduleRepository.InitTables();
            var ProgramScheduleTimeRepository = serviceProvider.GetRequiredService<IProgramScheduleTimeRepository>();//播放排程时刻表
            ProgramScheduleTimeRepository.InitTables();
            return services;
        }
    }
}
