using Advantech.Entity.Schedule;
using Advantech.Repository;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ProgramScheduleTimeService : ProgramScheduleTimeRepository, IProgramScheduleTimeService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProgramScheduleTimeService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }

        public string GetAll()
        {
            var query = _sqlSugarClient.Queryable<ProgramScheduleTime,ScheduleTimeDate>
                        ((m,n)=>new object[] { 
                        JoinType.Left,m.id==n.schedule_time_id
                        })
                        .Select((m,n) => new {
                            m.id,
                            m.schedule_id,
                            m.schedule_day_id,
                            n.schedule_date,
                            m.schedule_week,
                            m.primary_bit
                        }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetAllDetails()
        {
            var query = _sqlSugarClient.Queryable<ProgramScheduleTime, ProgramSchedule, ScheduleDay,ScheduleTimeDate>
                    ((m, n, p,q) => new object[] {
                    JoinType.Left,m.schedule_id == n.id,
                    JoinType.Left,m.schedule_day_id == p.id,
                    JoinType.Left,m.id==q.schedule_time_id
                    })
                    .Select((m, n, p,q) => new {
                        m.id,
                        m.schedule_id,
                        program_schedule_name = n.schedule_name,
                        m.schedule_day_id,
                        schedule_day_name = p.schedule_name,
                        q.schedule_date,
                        m.schedule_week,
                        m.primary_bit
                    }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetById(int id)
        {
            var query = _sqlSugarClient.Queryable<ProgramScheduleTime,ScheduleTimeDate>
                        ((m,n)=> new object[] {
                            JoinType.Left,m.id==n.schedule_time_id
                        })
                        .Where(m => m.id == id)
                        .Select((m,n) => new {
                            m.id,
                            m.schedule_id,
                            m.schedule_day_id,
                            n.schedule_date,
                            m.schedule_week,
                            m.primary_bit
                        }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetDetailsById(int id)
        {
            var query = _sqlSugarClient.Queryable<ProgramScheduleTime, ProgramSchedule, ScheduleDay, ScheduleTimeDate>
                    ((m, n, p, q) => new object[] {
                    JoinType.Left,m.schedule_id == n.id,
                    JoinType.Left,m.schedule_day_id == p.id,
                    JoinType.Left,m.id==q.schedule_time_id
                    })
                    .Where(m => m.id == id)
                    .Select((m, n, p, q) => new {
                        m.id,
                        m.schedule_id,
                        program_schedule_name = n.schedule_name,
                        m.schedule_day_id,
                        schedule_day_name = p.schedule_name,
                        q.schedule_date,
                        m.schedule_week,
                        m.primary_bit
                    }).ToList();
            return JsonConvert.SerializeObject(query);
        }
    }
}
