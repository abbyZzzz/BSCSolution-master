using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.Schedule
{
    [SugarTable("program_schedule ")]
    public class ProgramSchedule
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
        /// user_group中的id
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
        /// 排程具体的列表信息
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public List<ProgramScheduleTime> TimeList { set; get; }
    }
}
