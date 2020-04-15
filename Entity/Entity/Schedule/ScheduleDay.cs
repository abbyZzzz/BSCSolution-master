using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.Schedule
{
    [SugarTable("schedule_day ")]
    public class ScheduleDay
    {
        /// <summary>
        /// 群组id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 播放排程名称
        /// </summary>
        [SugarColumn(Length = 100)]
        public string schedule_name { get; set; }
        /// <summary>
        /// 对应区域/部门的id
        /// </summary>
        public int group_id { get; set; }

        [SugarColumn(IsIgnore =true)]
        public string group_name { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int user_id { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string user_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 节目时刻表清单
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public List<ScheduleDayProgram> ProgramList { set; get; }
    }
}
