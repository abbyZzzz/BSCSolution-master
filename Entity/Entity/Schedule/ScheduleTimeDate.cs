using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.Schedule
{
    [SugarTable("schedule_time_date")]
    public class ScheduleTimeDate
    {
        /// <summary>
        /// 群组id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// program_schedule_time（播放排程时刻表）
        /// </summary>
        public int schedule_time_id { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string schedule_day_name { get; set; }
        /// <summary>
        /// 播放的日期
        /// </summary>
        [SugarColumn(IsNullable = true,Length =10)]
        public DateTime schedule_date { get; set; }
    }
}
