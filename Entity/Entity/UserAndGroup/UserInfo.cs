using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity.UserAndGroup
{
    /// <summary>
    /// 用于登录的用户对象
    /// </summary>
    public class LoginUserMode
    {
        /// <summary>
        /// 账户
        /// </summary>
        [SugarColumn(Length = 20)]
        public string user_name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(Length = 20)]
        public string user_pwd { get; set; }
    }
    [SugarTable("user_info")]
    public class UserInfo : LoginUserMode
    {
        /// <summary>
        /// 用户id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 用户描述
        /// </summary>
        [SugarColumn(Length = 100)]
        public string user_desc { get; set; }
        /// <summary>
        /// 用户建立时间
        /// </summary>
        public DateTime creat_time { get; set; }
        /// <summary>
        /// 群组id
        /// </summary>
        public int group_id { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime last_login { get; set; }
    }
}
