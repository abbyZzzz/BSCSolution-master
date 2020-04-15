using Advantech.Entity;
using Advantech.Entity.Schedule;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ProgramScheduleRepository : BaseRepository<ProgramSchedule>, IProgramScheduleRepository
    {
        public ProgramScheduleRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
