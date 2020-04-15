using Advantech.BSCPlayer.ViewModel;
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

namespace Advantech.BSCPlayer.View
{
    /// <summary>
    /// Window_Msg.xaml 的交互逻辑
    /// </summary>
    public partial class MsgDialogView : Window
    {
        public MsgDialogView()
        {
            InitializeComponent();
            MsgDialogViewModel msgDialogViewModel= IoC.GetViewModel<MsgDialogViewModel>(this);
            this.DataContext = msgDialogViewModel;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
