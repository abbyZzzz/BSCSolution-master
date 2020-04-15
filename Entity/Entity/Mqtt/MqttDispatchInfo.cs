using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity.Mqtt
{
    /// <summary>
    /// 派送的MQTT消息
    /// </summary>
    public class MqttDispatchInfo
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        public string client_id { set; get; } 
        /// <summary>
        /// 派送类型。=0为排程，1为试播或者插播的节目
        /// </summary>
        public int type { set; get; }
        /// <summary>
        /// 派送的内容的id，对应排程id或者节目id
        /// </summary>
        public int content_id { set; get; }
        /// <summary>
        /// 播放开始时间
        /// </summary>
        public DateTime start_time { set; get; }
        /// <summary>
        /// 播放结束时间
        /// </summary>
        public DateTime end_time { set; get; }
    }
}
