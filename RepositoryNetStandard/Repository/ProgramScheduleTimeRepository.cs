using Advantech.Entity;
using Advantech.Entity.Schedule;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ProgramScheduleTimeRepository : BaseRepository<ProgramScheduleTime>, IProgramScheduleTimeRepository
    {
        public ProgramScheduleTimeRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
