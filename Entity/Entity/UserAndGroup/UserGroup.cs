using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity.UserAndGroup
{
    [SugarTable("user_group")]
    public class UserGroup
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 群组名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string group_name { get; set; }
        /// <summary>
        /// 群组描述
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string group_desc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
