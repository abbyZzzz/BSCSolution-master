using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    /// <summary>
    /// 节目信息
    /// </summary>
    [SugarTable("program_info")]
    public class ProgramInfo
    {
        /// <summary>
        /// 媒体编号 自增长
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        /// <summary>
        /// 节目id序号
        /// </summary>
        public string program_name { get; set; }

        /// <summary>
        /// 群组id
        /// </summary>
        public int program_group_id { get; set; }
        ///// <summary>
        ///// x坐标
        ///// </summary>
        //public int x { get; set; }
        ///// <summary>
        ///// y坐标
        ///// </summary>
        //public int y { get; set; }
        /// <summary>
        /// 尺寸宽
        /// </summary>
        public int w { get; set; }
        /// <summary>
        /// 尺寸高
        /// </summary>
        public int h { get; set; }
        /// <summary>
        /// 背景图片id
        /// </summary>
        public int bg_image { get; set; }
        /// <summary>
        /// 背景音乐id
        /// </summary>
        public int bg_music { get; set; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string bg_color { get; set; }
        ///// <summary>
        ///// 包含的档案格式
        ///// </summary>
        //public string media_type { get; set; }
        
        /// <summary>
        /// 屏幕位置设定
        /// </summary>
        public string screen_area { get; set; }
        ///// <summary>
        ///// X轴座标偏移值
        ///// </summary>
        //public int revise_x { get; set; }
        ///// <summary>
        ///// Y轴座标偏移值
        ///// </summary>
        //public int revise_y { get; set; }
        /// <summary>
        /// 屏幕数量
        /// </summary>
        public int screen_num { get; set; }
        /// <summary>
        /// 屏幕位置设定
        /// </summary>
        public string layout { get; set; }
        /// <summary>
        /// 是否在编辑中
        /// </summary>
        public int is_editing { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 审核状态：0待提交，1审核中，2审核通过，3退回
        /// </summary>
        public int audit_status { get; set; }
        /// <summary>
        /// 播放总时长
        /// </summary>
        public int play_second { get; set; }
        /// <summary>
        /// 最后编辑时间
        /// </summary>
        public DateTime last_time { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 区块列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<ProgramRegion> RegionList { set; get; } = new List<ProgramRegion>();
        /// <summary>
        /// 节目图片地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string program_address { get; set; }
        /// <summary>
        /// 节目预览图地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string preview_address { get; set; }
    }
}

