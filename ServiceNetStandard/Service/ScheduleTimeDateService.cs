using Advantech.Entity.Schedule;
using Advantech.Repository;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ScheduleTimeDateService : ScheduleTimeDateRepository, IScheduleTimeDateService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ScheduleTimeDateService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }

        public string GetAll()
        {
            var query = _sqlSugarClient.Queryable<ScheduleTimeDate>()
                            .Select(s => new {
                                s.id,
                                s.schedule_time_id,
                                s.schedule_date,
                            }).ToList();
            return JsonConvert.SerializeObject(query);
        }
    }
}
