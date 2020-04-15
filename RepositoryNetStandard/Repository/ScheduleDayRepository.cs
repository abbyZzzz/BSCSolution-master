using Advantech.Entity;
using Advantech.Entity.Schedule;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ScheduleDayRepository : BaseRepository<ScheduleDay>, IScheduleDayRepository
    {
        public ScheduleDayRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
