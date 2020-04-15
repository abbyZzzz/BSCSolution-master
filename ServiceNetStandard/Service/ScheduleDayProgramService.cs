using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Repository;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ScheduleDayProgramService : ScheduleDayProgramRepository, IScheduleDayProgramService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ScheduleDayProgramService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }

        public string GetAll()
        {
            var query = _sqlSugarClient.Queryable<ScheduleDayProgram>().ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetAllDetails()
        {
            var query = _sqlSugarClient.Queryable<ScheduleDayProgram, ProgramInfo, ScheduleDay>
                    ((m, n, p) => new object[] {
                    JoinType.Left,m.program_id == n.id,
                    JoinType.Left,m.schedule_id == p.id

                    })
                    .Select((m, n, p) => new {
                        m.id,
                        m.schedule_id,
                        p.schedule_name,
                        m.program_id,
                        n.program_name,
                        m.start_time,
                        m.end_time
                    }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetById(int id)
        {
            var query = _sqlSugarClient.Queryable<ScheduleDayProgram>().Where(s => s.id == id).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetDetailsById(int id)
        {
            var query = _sqlSugarClient.Queryable<ScheduleDayProgram, ProgramInfo, ScheduleDay>
                    ((m, n, p) => new object[] {
                    JoinType.Left,m.program_id == n.id,
                    JoinType.Left,m.schedule_id == p.id
                    })
                    .Where(m => m.id == id)
                    .Select((m, n, p) => new {
                        m.id,
                        m.schedule_id,
                        p.schedule_name,
                        m.program_id,
                        n.program_name,
                        m.start_time,
                        m.end_time
                    }).ToList();
            return JsonConvert.SerializeObject(query);
        }
    }
}
