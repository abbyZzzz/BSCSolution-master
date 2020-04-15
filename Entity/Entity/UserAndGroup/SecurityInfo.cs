using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.UserAndGroup
{
    [SugarTable("security_info")]
    public class SecurityInfo
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string security_name { get; set; }
        /// <summary>
        /// 权限描述信息
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string security_desc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
