using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.Schedule
{
    [SugarTable("schedule_day_program")]
    public class ScheduleDayProgram
    {
        /// <summary>
        /// 群组id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 播放排程id
        /// </summary>
        public int schedule_id { get; set; }

        [SugarColumn(IsIgnore =true)]
        public string schedule_name { get; set; }
        /// <summary>
        /// 节目编号
        /// </summary>
        public int program_id { get; set; }


        [SugarColumn(IsIgnore = true)]
        public string program_name { get; set; }
        /// <summary>
        /// 开始时间.格式必须统一为18:00:00
        /// </summary>
        public string start_time { get; set; }
        /// <summary>
        /// 结束时间.格式必须统一为18:00:00
        /// </summary>
        public string end_time { get; set; }
    }
}
