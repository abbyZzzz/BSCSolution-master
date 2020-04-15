using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.UserAndGroup
{
    [SugarTable("user_security")]
    public class UserSecurity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 权限id
        /// </summary>
        public int security_id { get; set; }
    }
}
