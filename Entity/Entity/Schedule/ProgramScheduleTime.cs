using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.Schedule
{
    [SugarTable("program_schedule_time ")]
    public class ProgramScheduleTime
    {
        /// <summary>
        /// 群组id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 排程id
        /// </summary>
        public int schedule_id { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string program_schedule_name { get; set; }
        /// <summary>
        /// 单日时刻表id
        /// </summary>
        public int schedule_day_id { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string schedule_day_name { get; set; }
        /// <summary>
        /// 播放的日期
        /// </summary>
        [SugarColumn(IsNullable = true,Length =10)]
        public string schedule_date { get; set; }
        /// <summary>
        /// 周几播放，里面数字按照分号分隔
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 20)]
        public string schedule_week { get; set; }
        /// <summary>
        /// 默认值为0；=1则表示为默认的单日时刻表。每个排程默认的单日时刻表有且只能有一个
        /// </summary>
        public int primary_bit { get; set; } = 0;
        /// <summary>
        /// 单日时刻表对象
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public ScheduleDay scheduleDay { set; get; }
    }
}
