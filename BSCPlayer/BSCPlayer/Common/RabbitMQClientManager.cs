using Advantech.BSCPlayer.Common;
using Advantech.BSCPlayer.ViewModel;
using Advantech.Entity;
using Advantech.Entity.Mqtt;
using Advantech.Entity.Schedule;
using Advantech.Service;
using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib;
using Advantech.UtilsStandardLib.Mqtt;
using CommonServiceLocator;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Advantech.AppCommon
{
    /// <summary>
    /// rabbitmq
    /// </summary>
    public class RabbitMQClientManager
    {
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName;
        /// <summary>
        /// 派发的队列
        /// </summary>
        public string DispatchQueue;
        /// <summary>
        /// 派发的routekey
        /// </summary>
        public string DispatchRouteKey;
        /// <summary>
        /// 状态队列
        /// </summary>
        public string StatusQueue;
        /// <summary>
        /// 状态的routekey
        /// </summary>
        public string StatusRouteKey;
        public RabbitMQClientHandler rabbitMQClientHandler;
        private readonly MediaFileTask _mediaFileTask;
        private readonly IHttpRequest _httpRequest;
        private readonly IProgramInfoService _programService;
        private readonly IProgramRegionMediaService _regionMediaService;
        private readonly IProgramRegionService _programRegionService;
        private readonly IProgramScheduleService _programScheduleService;
        private readonly IProgramScheduleTimeService _programScheduleTimeService;
        private readonly IScheduleDayService _scheduleDayService;
        private readonly IScheduleDayProgramService _scheduleDayProgramService;
        private readonly ILogWrite _log;

        public RabbitMQClientManager(MediaFileTask mediaFileTask, IHttpRequest httpRequest, ILogWrite log,
                                     IProgramInfoService programService,
                                     IProgramRegionMediaService regionMediaService,
                                     IProgramRegionService programRegionService,
                                     IProgramScheduleService programScheduleService,
                                     IProgramScheduleTimeService programScheduleTimeService,
                                     IScheduleDayService scheduleDayService,
                                     IScheduleDayProgramService scheduleDayProgramService
                                     )
        {
            _mediaFileTask = mediaFileTask;
            _httpRequest = httpRequest;
            _log = log;
            _programService = programService;
            _regionMediaService = regionMediaService;
            _programRegionService = programRegionService;
            _programScheduleService = programScheduleService;
            _programScheduleTimeService = programScheduleTimeService;
            _scheduleDayService = scheduleDayService;
            _scheduleDayProgramService = scheduleDayProgramService;
            LoadSettings();
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadSettings()
        {
            MQTTConfig config = new MQTTConfig();
            config.HostName = ConfigurationManager.AppSettings["Mqtt_Server"];
            config.Port =int.Parse(ConfigurationManager.AppSettings["Mqtt_Port"]);
            config.UserName = ConfigurationManager.AppSettings["Mqtt_User"];
            config.Password = ConfigurationManager.AppSettings["Mqtt_Password"];
            config.DefaultExchangeName= ConfigurationManager.AppSettings["Mqtt_Exchange"];
            ILogWrite _log = ServiceLocator.Current.GetInstance<ILogWrite>();
            rabbitMQClientHandler = new RabbitMQClientHandler(config, _log);
            rabbitMQClientHandler.Connect();//连接

            ExchangeName = config.DefaultExchangeName;
            DispatchQueue = ConfigurationManager.AppSettings["Mqtt_DispatchQueue"];
            DispatchRouteKey = ConfigurationManager.AppSettings["Mqtt_DispatchRouteKey"];
            List<RabbitQueueBind> list = new List<RabbitQueueBind>();
            list.Add(new RabbitQueueBind(){ 
                   ExchangeName= ExchangeName,
                   QueueName= DispatchQueue,
                   RouteKey= DispatchRouteKey
            });

            StatusQueue = ConfigurationManager.AppSettings["Mqtt_StatusQueue"];
            StatusRouteKey = ConfigurationManager.AppSettings["Mqtt_StatusRouteKey"];
            list.Add(new RabbitQueueBind()
            {
                ExchangeName = ExchangeName,
                QueueName = StatusQueue,
                RouteKey = StatusRouteKey
            });

            rabbitMQClientHandler.ExchangeAndQueueInitial(ExchangeName, ExchangeType.Topic, list);//初始化交换机和队列的绑定关系

            rabbitMQClientHandler.RabbitmqMessageConsume(DispatchQueue);//订阅队列：派送队列
            rabbitMQClientHandler.MessageReceivedRaised += RabbitMQClientHandler_MessageReceivedRaised;
        }

        /// <summary>
        /// mqtt消息接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RabbitMQClientHandler_MessageReceivedRaised(object sender, EventArgs e)
        {
            MqttMsgArgs mqttMsgArgs = e as MqttMsgArgs;
            try
            {
                MqttDispatchInfo dispatchTaskMqtt = JsonConvert.DeserializeObject<MqttDispatchInfo>(mqttMsgArgs.Content);
                if (dispatchTaskMqtt != null)
                {
                    if (dispatchTaskMqtt.client_id.Contains(ClientInfoViewModel.client_id))//派送到本地客户端
                    {
                        _log.WriteLog($"接收到mqtt派送信息{dispatchTaskMqtt.content_id}");
                        if (dispatchTaskMqtt.type == 0)//排程信息
                        {
                            LoadSchedule(dispatchTaskMqtt.content_id);
                        }
                        else//插播试播的节目信息
                        {
                            LoadProgrammeInfo(dispatchTaskMqtt.content_id);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _log.WriteLog($"mqtt派送信息处理出错",ex);
            }
            rabbitMQClientHandler.RejectQueueMsg(mqttMsgArgs.DeliveryTag, false);//确认消息，否则下次不会再次发送消息过来
        }
        /// <summary>
        /// 加载排程信息到本地
        /// </summary>
        /// <param name="schedule_id"></param>
        /// <returns></returns>
        private bool LoadSchedule(int schedule_id)
        {
            string response = _httpRequest.HttpGet(ServerCfg.DataServerAddr + $"/program/ProgramSchedule/get/{schedule_id}");
            ProgramSchedule programSchedule = JsonConvert.DeserializeObject<ProgramSchedule>(response);
            if (programSchedule != null)
            {
                _programScheduleService.ClearAll();//清除本地所有信息

                if (_programScheduleService.Insert(programSchedule)==false)
                {
                    _log.WriteLog($"排程{programSchedule.id}存储失败");
                    return false;
                }
                foreach (var schduleTime in programSchedule.TimeList)
                {
                    if(_programScheduleTimeService.Insert(schduleTime)==false)
                    {
                        _log.WriteLog($"排程{programSchedule.id}播放时刻{schduleTime.id}存储失败");
                        return false;
                    }
                    if(_scheduleDayService.Insert(schduleTime.scheduleDay)==false)
                    {
                        _log.WriteLog($"排程{programSchedule.id}播放时刻{schduleTime.id}单日时刻{schduleTime.scheduleDay.id}存储失败");
                        return false;
                    }
                    foreach (ScheduleDayProgram dayProg in schduleTime.scheduleDay.ProgramList)
                    {
                        if (_scheduleDayProgramService.Insert(dayProg) == false)
                        {
                            _log.WriteLog($"排程{programSchedule.id}播放时刻{schduleTime.id}单日时刻{schduleTime.scheduleDay.id}节目{dayProg.id}存储失败");
                            return false;
                        }

                        if (LoadProgrammeInfo(dayProg.program_id)==false)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 加载节目信息到本地
        /// </summary>
        /// <param name="program_id"></param>
        /// <returns></returns>
        private bool LoadProgrammeInfo(int program_id)
        {
            string response = _httpRequest.HttpGet(ServerCfg.DataServerAddr+ $"program/ProgramInfo/{program_id}");
            ProgramInfo program = JsonConvert.DeserializeObject<ProgramInfo>(response);
            if (program != null)
            {
                if(_programService.Insert(program))
                {
                    foreach (var region in program.RegionList)
                    {
                        if(_programRegionService.Insert(region))
                        {
                            foreach (var media in region.MediaList)
                            {
                                if(_regionMediaService.Insert(media))
                                {
                                    _mediaFileTask.GetMediaFileInfo(media.media_id);//加入到下载队列
                                }
                                else
                                {
                                    _log.WriteLog($"节目{program.id}区块{region.id}媒体{media.id}存储失败");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            _log.WriteLog($"节目{program.id}区块{region.id}存储失败");
                            return false;
                        }
                    }
                }
                else
                {
                    _log.WriteLog($"节目{program.id}存储失败");
                    return false;
                }
            }
            return true;
        }
    }
}
