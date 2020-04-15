using Advantech.Entity.Schedule;
using Advantech.Entity.UserAndGroup;
using Advantech.Repository;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ProgramScheduleService : ProgramScheduleRepository, IProgramScheduleService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProgramScheduleService(ISqlSugarClient sqlSugarClient) :base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
        /// <summary>
        /// 清除所有的排程信息，player播放端使用
        /// </summary>
        /// <returns></returns>
        public bool ClearAll()
        {
            _sqlSugarClient.Deleteable<ProgramScheduleTime>(x=>x.id>0);
            _sqlSugarClient.Deleteable<ScheduleDay>(x => x.id > 0);
            _sqlSugarClient.Deleteable<ScheduleDayProgram>(x => x.id > 0);
            return this.Delete(x => x.id > 0);
        }

        public string GetAll()
        {
            var query = _sqlSugarClient.Queryable<ProgramSchedule>()
                        .Select(s => new
                        {
                            s.id,
                            s.schedule_name,
                            s.group_id,
                            s.user_id,
                            s.create_time
                        }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetAllDetails()
        {
            var query = _sqlSugarClient.Queryable<ProgramSchedule, UserInfo, UserGroup>
                    ((m, n, p) => new object[] {
                    JoinType.Left,m.user_id == n.id,
                    JoinType.Left,m.group_id == p.id,
                    JoinType.Left,n.group_id==p.id
                    })
                    .Select((m, n, p) => new
                    {
                        m.id,
                        m.schedule_name,
                        m.group_id,
                        p.group_name,
                        m.user_id,
                        n.user_name,
                        m.create_time
                    }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetById(int id)
        {
            var query = _sqlSugarClient.Queryable<ProgramSchedule>()
                        .Where(s => s.id == id)
                        .Select(s => new
                        {
                            s.id,
                            s.schedule_name,
                            s.group_id,
                            s.user_id,
                            s.create_time
                        }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetDetailsById(int id)
        {
            var query = _sqlSugarClient.Queryable<ProgramSchedule, UserInfo, UserGroup>
                        ((m, n, p) => new object[] {
                            JoinType.Left,m.user_id == n.id,
                            JoinType.Left,m.group_id == p.id,
                            JoinType.Left,n.group_id==p.id
                        })
                        .Where(m => m.id == id)
                        .Select((m, n, p) => new
                        {
                            m.id,
                            m.schedule_name,
                            m.group_id,
                            p.group_name,
                            m.user_id,
                            n.user_name,
                            m.create_time
                        }).ToList();
            return JsonConvert.SerializeObject(query);
        }
        /// <summary>
        /// 联合查询，得出复合对象信息
        /// </summary>
        /// <returns></returns>
        public string GetAllUnionQuery() {
            List<ProgramSchedule> programSchedules = _sqlSugarClient.Queryable<ProgramSchedule>().Select(s=>new ProgramSchedule()).ToList();
            List<ProgramScheduleTime> programScheduleTimes = _sqlSugarClient.Queryable<ProgramScheduleTime>().Select(s => new ProgramScheduleTime()).ToList();
            List<ScheduleDay> scheduleDays = _sqlSugarClient.Queryable<ScheduleDay>().Select(s => new ScheduleDay()).ToList();
            List<ScheduleDayProgram> scheduleDayPrograms = _sqlSugarClient.Queryable<ScheduleDayProgram>().Select(s => new ScheduleDayProgram()).ToList();

            for (int i = 0; i < programSchedules.Count; i++)
            {
                List<ProgramScheduleTime> programScheduleTimesRes = new List<ProgramScheduleTime>();
                for (int j = 0; j < programScheduleTimes.Count; j++)
                {
                    for (int m = 0; m < scheduleDays.Count; m++)
                    {
                        List<ScheduleDayProgram> scheduleDayProgramsRes = new List<ScheduleDayProgram>();
                        for (int n = 0; n < scheduleDayPrograms.Count; n++)
                        {
                            if (scheduleDays[m].id == scheduleDayPrograms[n].schedule_id)
                            {
                                scheduleDayProgramsRes.Add(scheduleDayPrograms[n]);
                            }
                        }
                        scheduleDays[m].ProgramList = scheduleDayProgramsRes;

                        if (programScheduleTimes[j].schedule_day_id == scheduleDays[m].id)
                        {
                            programScheduleTimes[j].scheduleDay = scheduleDays[m];
                        }
                    }

                    if (programSchedules[i].id == programScheduleTimes[j].schedule_id)
                    {
                        programScheduleTimesRes.Add(programScheduleTimes[j]);
                    }
                }

                programSchedules[i].TimeList = programScheduleTimesRes;
            }

            return JsonConvert.SerializeObject(programSchedules);
        }
        /// <summary>
        /// 联合查询，得出复合对象信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProgramSchedule GetCompositeById(int id)
        {
            ProgramSchedule schedule = this.QueryableToEntity(x => x.id == id);
            if (schedule != null)
            {
                List<ProgramScheduleTime> list = _sqlSugarClient.Queryable<ProgramScheduleTime>()
                                                                .Where(x => x.schedule_id == schedule.id).ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var scheduleTimeItem in list)
                    {
                        scheduleTimeItem.scheduleDay = _sqlSugarClient.Queryable<ScheduleDay>()
                                                                      .First(x => x.id == scheduleTimeItem.schedule_day_id);
                        if (scheduleTimeItem.scheduleDay != null)
                        {
                            var dayList = _sqlSugarClient.Queryable<ScheduleDayProgram>()
                                                         .Where(x => x.schedule_id == scheduleTimeItem.scheduleDay.id).ToList();
                            scheduleTimeItem.scheduleDay.ProgramList = dayList;
                        }
                    }
                }
                schedule.TimeList = list;
            }
            return schedule;
        }
    }
}
