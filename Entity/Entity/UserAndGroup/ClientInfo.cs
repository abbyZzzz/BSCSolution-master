using System;
using SqlSugar;
using System.Text;

namespace Advantech.Entity.UserAndGroup
{
    [SugarTable("bsc_user_groups")]
    public class ClientInfo
    {
        /// <summary>
        /// 群组id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 客户端id，一般为客户端的cpu id信息
        /// </summary>
        [SugarColumn(Length = 20)]
        public string client_id { get; set; }
        /// <summary>
        /// 客户端所在群组
        /// </summary>
        public int cgroup_id { get; set; }
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string client_name { get; set; }
        /// <summary>
        /// 当前播放的排程id
        /// </summary>
        public int schedule_id { get; set; }
        /// <summary>
        /// 软件版本
        /// </summary>
        public string software_version { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string ip_address { get; set; }
        /// <summary>
        /// 屏幕数量
        /// </summary>
        public int screen_count { get; set; }
        /// <summary>
        /// 电脑名称
        /// </summary>
        public string computer_name { get; set; }
        /// <summary>
        /// 操作系统信息
        /// </summary>
        public string system_info { get; set; }
        /// <summary>
        /// 主板信息
        /// </summary>
        public string brand_info { get; set; }
        /// <summary>
        /// bios信息
        /// </summary>
        public string bios_info { get; set; }
        /// <summary>
        /// cpu信息
        /// </summary>
        public string cpu_info { get; set; }
        /// <summary>
        /// 存储容量
        /// </summary>
        public string memory_size { get; set; }
        /// <summary>
        /// 自动开机时间
        /// </summary>
        public string wakeup_time { get; set; }
        /// <summary>
        /// 是否启用自动开机
        /// </summary>
        public bool auto_wakeup { get; set; }
        /// <summary>
        /// 自动关机时间
        /// </summary>
        public string shutdown_time { get; set; }
        /// <summary>
        /// 是否启用自动关机
        /// </summary>
        public bool auto_shutdown { get; set; }
        /// <summary>
        /// 连线时间
        /// </summary>
        public DateTime online_time { get; set; }
        /// <summary>
        /// 断线时间
        /// </summary>
        public DateTime offline_time { get; set; }
        /// <summary>
        /// 派发时间
        /// </summary>
        public DateTime dispatch_time { get; set; }
        /// <summary>
        /// 在线状态
        /// </summary>
        public int online_status { get; set; }
    }
}
