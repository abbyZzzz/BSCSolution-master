using Advantech.Entity;
using Advantech.Entity.Schedule;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ScheduleTimeDateRepository : BaseRepository<ScheduleTimeDate>, IScheduleTimeDateRepository
    {
        public ScheduleTimeDateRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
