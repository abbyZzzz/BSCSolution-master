using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.UtilsStandardLib.Mqtt
{
    /// <summary>
    /// 队列绑定关系
    /// </summary>
    public class RabbitQueueBind
    {
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { set; get; }
        /// <summary>
        /// queue名称
        /// </summary>
        public string QueueName { set; get; }
        /// <summary>
        /// routekey
        /// </summary>
        public string RouteKey { set; get; }
    }
}
