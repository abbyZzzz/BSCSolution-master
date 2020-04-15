using Advantech.AppCommon;
using Advantech.BSCPlayer;
using Advantech.BSCPlayer.Schedule;
using Advantech.Entity;
using Advantech.Service;
using Advantech.UtilsStandardLib;
using CommonServiceLocator;
using RabbitMQ.Client;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Advantech.BSCPlayer.View
{
    /// <summary>
    /// Window_layout.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private readonly ProgramScheduleManager _programScheduleManager;

        public MainView(ProgramScheduleManager programScheduleManager)
        {
            InitializeComponent();
            _programScheduleManager = programScheduleManager;
            _programScheduleManager.mainWindow = this;
            _programScheduleManager.PlayStart();
            //ClientInfoManager.SendClientOnlineStatusMsg();
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MsgDialogView msgDialogView = new MsgDialogView();
            msgDialogView.ShowDialog();
        }

        //private void button_Click(object sender, RoutedEventArgs e)
        //{
        //   var test = IoC.Get<MediaFileTask>();
        //    test.GetMediaFileInfo("150");
        //}
    }
}
