using Advantech.AppCommon;
using Advantech.BSCPlayer.Common;
using Advantech.BSCPlayer.Schedule;
using Advantech.BSCPlayer.View;
using Advantech.BSCPlayer.ViewModel;
using Advantech.Entity;
using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using log4net;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Advantech.BSCPlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IoC.SetupIoC();//IoC容器启用
            ServerCfg.LoadServerSettings();
            var clientInfoManager = IoC.Get<ClientInfoViewModel>();
            clientInfoManager.GetPlayerInfo();                            //获取本地的硬件信息
            
            var log= IoC.Get<ILogWrite>();
            log.WriteLog("程序启动");

            var scheduleManager = IoC.Get<ProgramScheduleManager>();
            MainView win = new MainView(scheduleManager);
            win.Show();
            Application.Current.Exit += Current_Exit;
        }
        /// <summary>
        /// 程序退出清理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_Exit(object sender, ExitEventArgs e)
        {
            //var scheduleManager = IoC.Get<ProgramScheduleManager>();//排程任务退出
            //scheduleManager.Dispose();
            //var rabbitMQManager = IoC.Get<RabbitMQClientManager>();
            //rabbitMQManager.rabbitMQClientHandler.Cleanup();//mqtt客户端断开连接
            //var mediaFileTask = IoC.Get<MediaFileTask>();//文件下载任务退出
            //mediaFileTask.Dispose();
            IoC.DisPose();
            Environment.Exit(0);
            //ClientInfoManager.SendClientOfflineStatusMsg();//发送离线信息
            //RabbitMQClientManager.rabbitMQClientHandler.Cleanup();//mqtt客户端断开连接
            //mediaFileTask.Dispose();//
        }
    }
}
