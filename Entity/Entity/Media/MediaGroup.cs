using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    /// <summary>
    /// 档案目录结构
    /// </summary>
    [SugarTable("media_group")]
    public class MediaGroup
    {
        /// <summary>
        /// 序列号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 父群组id
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 群组名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string mgroup_name { get; set; }
        /// <summary>
        /// 区域id
        /// </summary>
        public int group_id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
