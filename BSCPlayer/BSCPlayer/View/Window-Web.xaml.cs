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
    /// Window_Web.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Web : Window
    {
        public Window_Web()
        {
            InitializeComponent();

            //browser.Load("https://www.baidu.com/");
            browser.Address = "https://www.baidu.com/";
        }
    }
}
