using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    /// <summary>
    /// 档案信息
    /// </summary>
    [SugarTable("media_info")]
    public class MediaInfo
    {
        /// <summary>
        /// 媒体编号 自增长
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

        public int id { get; set; }

        /// <summary>
        /// 重新分配的id序号名称
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string serial_number { get; set; }

        /// <summary>
        /// 媒体名称(原始名称)
        /// </summary>
        [SugarColumn(Length = 200)]
        public string media_name { get; set; }

        /// <summary>
        /// 媒体档案格式(pdf,ppt,video,image,html,audio)
        /// </summary>
        [SugarColumn(Length = 10)]
        public string media_type { get; set; }

        /// <summary>
        /// 档案大小
        /// </summary>
        public int media_size { get; set; }

        /// <summary>
        /// 影片播放长度，或者页码
        /// </summary>
        public int media_len { get; set; }

        /// <summary>
        /// 媒体宽
        /// </summary>
        public int media_width { get; set; }

        /// <summary>
        /// 媒体高
        /// </summary>
        public int media_height { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string media_argv { get; set; }

        /// <summary>
        /// 群组ID
        /// </summary>
        public int media_group_id { get; set; }

        /// <summary>
        /// 建立时间
        /// </summary>
        public DateTime create_time { get; set; } = DateTime.Now;

        /// <summary>
        /// 预留属性，雅马哈传输文件id
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string reserved_property1 { get; set; }

        /// <summary>
        /// 预留属性2
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string reserved_property2 { get; set; }

        /// <summary>
        /// 文件地址(非表格字段)
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string media_address { get; set; }

        /// <summary>
        /// 缩略图地址(非表格字段)
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string preview_address { get; set; }

    }
}

