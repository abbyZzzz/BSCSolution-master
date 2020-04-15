using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    [SugarTable("program_region")]
    public class ProgramRegion
    {
        /// <summary>
        /// 媒体编号 自增长
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        /// <summary>
        /// 节目id序号
        /// </summary>
        public int program_id { get; set; }

        /// <summary>
        /// 区块编号
        /// </summary>
        public int region_no { get; set; }
        /// <summary>
        /// x坐标
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// y坐标
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// 尺寸宽
        /// </summary>
        public int w { get; set; }
        /// <summary>
        /// 尺寸高
        /// </summary>
        public int h { get; set; }

        /// <summary>
        /// 档案列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<ProgramRegionMedia> MediaList { set; get; } = new List<ProgramRegionMedia>();
    }
}

