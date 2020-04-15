using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.UtilsStandardLib.Mqtt
{
    /// <summary>
    /// MQTT配置信息
    /// </summary>
    public class MQTTConfig
    {
        /// <summary>
        /// 虚拟主机，rabbitmq使用
        /// </summary>
        public string VirtualHost { set; get; }
        /// <summary>
        /// 主机信息
        /// </summary>
        public string HostName { set; get; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { set; get; } = 5672;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        /// 默认交换机名称
        /// </summary>
        public string DefaultExchangeName { set; get; }
    }
}
