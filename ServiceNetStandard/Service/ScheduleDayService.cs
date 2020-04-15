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
    public class ScheduleDayService : ScheduleDayRepository, IScheduleDayService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ScheduleDayService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
        /// <summary>
        /// 通过id获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScheduleDay GetScheduleDayById(int id)
        {
            ScheduleDay scheduleDay = this.QueryableToEntity(x => x.id == id);
            if(scheduleDay !=null)
            {
                scheduleDay.ProgramList = _sqlSugarClient.Queryable<ScheduleDayProgram>().Where(x => x.schedule_id == id).ToList();
            }
            return scheduleDay;

            //var query = _sqlSugarClient.Queryable<ScheduleDay, ScheduleDayProgram>
            //    ((s, p) => new object[] {
            //       JoinType.Left,s.id==p.schedule_id
            //     })
            //     .Select((s, p) => new
            //     {
            //         id = s.id,
            //         s.schedule_name,
            //         s.group_id,
            //         s.user_id,
            //         s.create_time,
            //         pid=p.id,
            //         CName = p.program_id,
            //         p.start_time,
            //         UName = p.end_time
            //     }).MergeTable().Where((s) => s.id == id);
        }


        /// <summary>
        /// 获取所有的ScheduleDay(数据库中原始数据)
        /// </summary>
        /// <returns></returns>
        public string GetAll() {
            var query = _sqlSugarClient.Queryable<ScheduleDay>()
                            .Select(s => new {
                                s.id,
                                s.schedule_name,
                                s.group_id,
                                s.user_id,
                                s.create_time
                            }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public string GetById(int id)
        {
            var query = _sqlSugarClient.Queryable<ScheduleDay>()
                        .Where(s => s.id == id)
                        .Select(s => new {
                            s.id,
                            s.schedule_name,
                            s.group_id,
                            s.user_id,
                            s.create_time
                        })
                        .ToList();
            return JsonConvert.SerializeObject(query);
        }

        /// <summary>
        /// 获取所有ScheduleDay(涉及连表的id均返回对应的name)
        /// </summary>
        /// <returns></returns>
        public string GetAllDetails() {
            var query = _sqlSugarClient.Queryable<ScheduleDay, UserInfo, UserGroup>
                        ((m, n, p) => new object[] {
                    JoinType.Left,m.user_id == n.id,
                    JoinType.Left,m.group_id == p.id,
                    JoinType.Left,n.group_id==p.id
                        })
                        .Select((m, n, p) => new {
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
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDetailsById(int id)
        {
            var query = _sqlSugarClient.Queryable<ScheduleDay, UserInfo, UserGroup>
                    ((m, n, p) => new object[] {
                    JoinType.Left,m.user_id == n.id,
                    JoinType.Left,m.group_id == p.id,
                    JoinType.Left,n.group_id==p.id
                    })
                    .Where(m => m.id == id)
                    .Select((m, n, p) => new {
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
    }
}
