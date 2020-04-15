using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    [SugarTable("program_region_media ")]
    public class ProgramRegionMedia
    {
        /// <summary>
        /// 媒体编号 自增长
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 区块id序号
        /// </summary>
        public int region_id { get; set; }
        /// <summary>
        /// 档案id信息
        /// </summary>
        public int media_id { get; set; }
        /// <summary>
        /// 播放顺序
        /// </summary>
        public int play_order { get; set; }
        /// <summary>
        /// 播放次数
        /// </summary>
        public int play_count { get; set; }
        /// <summary>
        /// 播放时长(秒)
        /// </summary>
        public int play_second { get; set; }
        /// <summary>
        /// 播放参数
        /// </summary>
        public string play_argv { get; set; }
        /// <summary>
        /// 单页停留时间
        /// </summary>
        public int play_page_changing_second { get; set; }
        ///// <summary>
        ///// 媒体的地址信息
        ///// </summary>
        //public string media_address { get; set; }
        /// <summary>
        /// 播放时长(秒)累计，用于线程内部缓存
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public int play_second_count { get; set; }
    }
}

