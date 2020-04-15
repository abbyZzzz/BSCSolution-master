using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.UtilsStandardLib.Mqtt
{
    /// <summary>
    /// mqtt消息
    /// </summary>
    public class MqttMsgArgs : EventArgs
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 消息标签，用于确认
        /// </summary>
        public ulong DeliveryTag { set; get; }

        public MqttMsgArgs(string content)
        {
            this.Content = content;
        }
        public MqttMsgArgs(ulong tag, string content)
        {
            this.DeliveryTag = tag;
            this.Content = content;
        }
    }
}
