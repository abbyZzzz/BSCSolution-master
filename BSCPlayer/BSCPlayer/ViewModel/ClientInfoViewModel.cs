using Advantech.AppCommon;
using Advantech.CommonDefineStandardLib;
using Advantech.Entity;
using Advantech.Entity.UserAndGroup;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantech.BSCPlayer.ViewModel
{
    public class ClientInfoViewModel : ViewModelBase
    {
        public ClientInfo _PlayerInfo;
        public ClientInfo PlayerInfo
        {
            get { return _PlayerInfo; }
            set { _PlayerInfo = value; RaisePropertyChanged(() => PlayerInfo); }
        }
        /// <summary>
        /// 本地客户端id
        /// </summary>
        public static string client_id;

        private readonly RabbitMQClientManager _rabbitMQClientManager;

        public ClientInfoViewModel(RabbitMQClientManager rabbitMQClientManager)
        {
            this._rabbitMQClientManager = rabbitMQClientManager;
        }
        /// <summary>
        /// 获取本地硬件信息
        /// </summary>
        public void GetPlayerInfo()
        {
            _PlayerInfo = new ClientInfo();
            _PlayerInfo.client_id = HardwareInfoHelper.GetCPUSN();
            client_id = _PlayerInfo.client_id;
            _PlayerInfo.computer_name = Environment.MachineName;
            _PlayerInfo.system_info = HardwareInfoHelper.GetOS_Version();
            _PlayerInfo.ip_address = HardwareInfoHelper.GetMachineIp();
            _PlayerInfo.bios_info = HardwareInfoHelper.GetBiosInfo();
            _PlayerInfo.brand_info = HardwareInfoHelper.GetBoardInfo();
            _PlayerInfo.cpu_info = HardwareInfoHelper.GetCPUInfo();
            _PlayerInfo.screen_count = HardwareInfoHelper.GetScreenCount();
            _PlayerInfo.memory_size = HardwareInfoHelper.GetPhisicalMemory();
            _PlayerInfo.software_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _PlayerInfo.online_status = (int)ClientStatusEnum.OnLine;
        }
        /// <summary>
        /// 发送客户端上线信息
        /// </summary>
        public void SendClientOnlineStatusMsg()
        {
            string clientJson = JsonConvert.SerializeObject(PlayerInfo);
            _rabbitMQClientManager.rabbitMQClientHandler.PublishMessageAndWaitConfirm("player.status", clientJson);
        }
        /// <summary>
        /// 发送客户端上线信息
        /// </summary>
        public void SendClientOfflineStatusMsg()
        {
            PlayerInfo.online_status = (int)ClientStatusEnum.OffLine;
            string clientJson = JsonConvert.SerializeObject(PlayerInfo);
            _rabbitMQClientManager.rabbitMQClientHandler.PublishMessageAndWaitConfirm("player.status", clientJson);
        }
    }
}
