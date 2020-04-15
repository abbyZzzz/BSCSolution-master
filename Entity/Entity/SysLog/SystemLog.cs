using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Advantech.Entity.SysLog
{
    /// <summary>
    /// 系统运行日志
    /// </summary>
    [SugarTable("system_log")]
    public class SystemLog
    {
        /// <summary>
        /// 序列号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 源
        /// </summary>
        [SugarColumn(IsNullable =true)]
        public string source { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string subject { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}
