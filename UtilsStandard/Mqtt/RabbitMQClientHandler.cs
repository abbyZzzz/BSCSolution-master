using Advantech.UtilsStandard.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Advantech.UtilsStandardLib.Mqtt
{
    /// <summary>
    /// RabbitMQ客户端处理类
    /// </summary>
    public class RabbitMQClientHandler
    {
        public  bool ExternHanleConsumerMsg = true;//外部处理消息
        private readonly MQTTConfig mConfig;
        /// <summary>
        /// 日志功能，需要外部注入
        /// </summary>
        private readonly ILogWrite _log;//日志记录功能注入
        /*-------------------------------------------------------------------------------------*/
        //ConnectionFactory
        private static ConnectionFactory mc_ConnectionFactory = null;
        //Connection
        private IConnection Connection;

        //Send Channel
        private IModel SendChannel;
        //Listen Channel
        private IModel ListenChannel;
        /// <summary>
        /// 只注入配置
        /// </summary>
        /// <param name="Config"></param>
        public RabbitMQClientHandler(MQTTConfig Config)
        {
            this.mConfig = Config;
        }
        /// <summary>
        /// 注入配置和日志
        /// </summary>
        /// <param name="Config"></param>
        /// <param name="log"></param>
        public RabbitMQClientHandler(MQTTConfig Config, ILogWrite log)
        {
            this.mConfig = Config;
            this._log = log;
        }
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            try
            {
                _log.WriteLog("获取RabbitMQ服务器参数：" + mConfig.HostName + ":" + mConfig.Port + " (" + mConfig.UserName + "/" + mConfig.Password + ")");
                //连接工厂
                mc_ConnectionFactory = new ConnectionFactory();

                //连接工厂信息
                mc_ConnectionFactory.HostName = mConfig.HostName;// "localhost";
                mc_ConnectionFactory.Port = mConfig.Port;// "5672"

                mc_ConnectionFactory.UserName = mConfig.UserName;// "guest";
                mc_ConnectionFactory.Password = mConfig.Password;// "guest";
                //mc_ConnectionFactory.VirtualHost = defaultRabbitVirtualHost;// "/"

                mc_ConnectionFactory.RequestedHeartbeat = 30;//心跳包
                mc_ConnectionFactory.AutomaticRecoveryEnabled = true;//自动重连
                mc_ConnectionFactory.TopologyRecoveryEnabled = true;//拓扑重连
                mc_ConnectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

                //创建连接
                Connection = mc_ConnectionFactory.CreateConnection();

                //断开连接时，调用方法自动重连
                Connection.ConnectionShutdown += Connection_ConnectionShutdown;

                //创建发送频道
                SendChannel = Connection.CreateModel();
                //创建接收频道
                ListenChannel = Connection.CreateModel();

                //发送频道确认模式，发送了消息后，可以收到回应
                SendChannel.ConfirmSelect();

                _log.WriteLog("尝试连接至RabbitMQ服务器：" + mConfig.HostName);
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
            {
                _log.WriteLog("尝试连接至RabbitMQ服务器错误：",e);
            }
            catch (Exception ex)
            {
                _log.WriteLog("尝试连接至RabbitMQ服务器错误：", ex);
            }
        }
        /// <summary>
        /// 绑定关系建立
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        /// <param name="BindingList"></param>
        public void ExchangeAndQueueInitial(string exchangeName,string exchangeType, List<RabbitQueueBind> BindingList)
        {
            if (!ChannelStateCheck(SendChannel)) return;
            SendChannel.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: true, autoDelete: false);//申明交换机

            foreach(var item in BindingList)
            {
                SendChannel.QueueDeclare(item.QueueName, true, false, false, null);//申明队列
                                                                               //交换机与队列绑定
                SendChannel.QueueBind(queue: item.QueueName,
                                      exchange: item.ExchangeName,
                                      routingKey: item.RouteKey);
            }
        }
        /// <summary>
        /// 清除连接信息
        /// </summary>
        public void Cleanup()
        {
            try
            {
                if (SendChannel != null && SendChannel.IsOpen)
                {
                    try
                    {
                        SendChannel.Close();
                    }
                    catch (Exception ex)
                    {
                        _log.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的Channel[发送]，但遇到错误", ex);
                    }
                    SendChannel = null;
                }

                if (ListenChannel != null && ListenChannel.IsOpen)
                {
                    try
                    {
                        ListenChannel.Close();
                    }
                    catch (Exception ex)
                    {
                        _log.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的Channel[接收]，但遇到错误", ex);
                    }
                    ListenChannel = null;
                }

                if (Connection != null && Connection.IsOpen)
                {
                    try
                    {
                        Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        _log.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的连接，但遇到错误", ex);
                    }
                    Connection = null;
                }
            }
            catch (IOException ex)
            {
                _log.WriteLog("RabbitMQ重新连接错误", ex);
            }
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _log.WriteLog("RabbitMQ已经断开连接，正在尝试重新连接至RabbitMQ服务器");

            Reconnect();
        }

        private void Reconnect()
        {
            //清除连接及频道
            Cleanup();
            Connect();
        }
        /// <summary>
        /// 消息队列和交换机在初始化进行创建
        /// </summary>
        /// <param name="routingKey">RoutingKey</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public bool PublishMessageAndWaitConfirm(string routingKey,string message)
        {
            try
            {
                if (!ChannelStateCheck(SendChannel)) return false;
                //设置消息持久性
                IBasicProperties props = SendChannel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = Encoding.UTF8.GetBytes(message);
                SendChannel.BasicPublish(mConfig.DefaultExchangeName, routingKey, props, messageBody);

                //等待确认
                return SendChannel.WaitForConfirms();
            }
            catch (Exception ex)
            {
                _log.WriteLog("RabbitMQ出现通用问题" + ex.Message, ex);

                return false;
            }
        }
        /// <summary>
        /// 发送消息至服务端.采用默认交换机，不管routekey和交换机名称
        /// </summary>
        /// <param name="ExchangeName">交换机名称</param>
        /// <param name="RoutingKey">路由名称</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public bool PublishMessageAndWaitConfirm(string ExchangeName, string RoutingKey, string message)
        {
            try
            {
                if (!ChannelStateCheck(SendChannel)) return false;
                ////声明队列
                //SendChannel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //设置消息持久性
                IBasicProperties props = SendChannel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = Encoding.UTF8.GetBytes(message);
                SendChannel.BasicPublish(ExchangeName, RoutingKey, null, messageBody);

                //等待确认
                return SendChannel.WaitForConfirms();
            }
            catch (Exception ex)
            {
                _log.WriteLog("RabbitMQ出现通用问题" + ex.Message, ex);

                return false;
            }
        }
        /// <summary>
        /// 接收处理消息
        /// </summary>
        /// <param name="queueName"></param>
        public void RabbitmqMessageConsume(string queueName)
        {
            try
            {
                if (!ChannelStateCheck(ListenChannel)) return;
                bool queueDurable = true;
                string QueueName = queueName;

                //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                ListenChannel.QueueDeclare(queue: QueueName,durable: queueDurable,exclusive: false, autoDelete:false, null);
                //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                ListenChannel.BasicQos(0, 1, false);

                //创建基于该队列的消费者，绑定事件
                var consumer = new EventingBasicConsumer(ListenChannel);

                //回应消息监控
                consumer.Received += SyncData_Received;
              
                //绑定消费者
                ListenChannel.BasicConsume(QueueName, //队列名
                                           false,    //false：手动应答；true：自动应答
                                           consumer);

                _log.WriteLog("开始监控RabbitMQ服务器，队列" + QueueName);

            }
            catch (Exception ex)
            {
                _log.WriteLog("开始监控RabbitMQ服务器出错" ,ex);
            }
        }
        /// <summary>
        /// 消息接收与内容解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncData_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                //TOOD 验证程序退出后消费者是否退出去了
                var body = e.Body; //消息主体
                string message = Encoding.UTF8.GetString(body);

                _log.WriteLog("[x] 队列接收到消息：" + message.ToString());

                if(ExternHanleConsumerMsg)//外部处理消息
                {
                    MqttMsgArgs mqttMsgArgs = new MqttMsgArgs(e.DeliveryTag, message);
                    OnMessageReceived(mqttMsgArgs);
                }
                else
                {
                    //处理数据
                    bool processSuccessFlag = true;//处理消息逻辑编写
                    if (processSuccessFlag)
                    {
                        //回复确认
                        ListenChannel.BasicAck(e.DeliveryTag, false);
                    }
                    else
                    {
                        //未正常处理的消息，重新放回队列
                        ListenChannel.BasicReject(e.DeliveryTag, true);
                    }
                }
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex1)
            {
                Thread.Sleep(5000);
                ListenChannel.BasicNack(e.DeliveryTag, false, true);
                _log.WriteLog("开始监控RabbitMQ服务器出错", ex1);
            }
            catch (Exception ex)
            {
                Thread.Sleep(5000);
                ListenChannel.BasicNack(e.DeliveryTag, false, true);
                _log.WriteLog("开始监控RabbitMQ服务器出错", ex);
            }
        }
        /// <summary>
        /// 撤除消息
        /// </summary>
        /// <param name="DeliveryTag"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool RejectQueueMsg(ulong DeliveryTag,bool status)
        {
            if (Connection == null || !Connection.IsOpen)
            {
                _log.WriteLog("连接为空或连接已经关闭");
                return false;
            }
            if (ListenChannel == null || !ListenChannel.IsOpen) 
            {
                _log.WriteLog("通道为空或通道已经关闭");
                return false;
            }

            ListenChannel.BasicReject(DeliveryTag, status);
            return true;
        }
        /// <summary>
        /// 通道状态检查
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool ChannelStateCheck(IModel channel)
        {
            if (Connection == null || !Connection.IsOpen)
            {
                _log.WriteLog("连接为空或连接已经关闭");
                return false;
            }
            if (SendChannel == null || !SendChannel.IsOpen)
            {
                _log.WriteLog("通道为空或通道已经关闭");
                return false;
            }
            return true;

        }
        #region 消息接收后事件触发
        public event EventHandler MessageReceivedRaised;

        private void OnMessageReceived(MqttMsgArgs message)
        {
            MessageReceivedRaised?.Invoke(this, message);
        }
        #endregion
    }  
}