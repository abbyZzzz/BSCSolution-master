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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Advantech.PlayerComponent
{
    /// <summary>
    /// MediaControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaControl : UserControl, IDisposable
    {
        /// <summary>
        /// 播放资源链接
        /// </summary>
        public Uri mediaUri { set; get; }
        private bool palyBit = false;

        public MediaControl()
        {
            InitializeComponent();
            // 交互式控制
            mediaElement.LoadedBehavior = MediaState.Manual;
            // 添加元素加载完成事件 -- 自动开始播放
            mediaElement.Loaded += new RoutedEventHandler(media_Loaded);
            // 添加媒体播放结束事件 -- 重新播放
            mediaElement.MediaEnded += new RoutedEventHandler(media_MediaEnded);
            // 添加元素卸载完成事件 -- 停止播放
            mediaElement.Unloaded += new RoutedEventHandler(media_Unloaded);
            mediaElement.MouseDown += mediaElement_MouseDown;
            this.MouseDown += mediaElement_MouseDown;
            mediaElement.Margin = new Thickness(0);
            mediaElement.HorizontalAlignment = HorizontalAlignment.Stretch;
            mediaElement.VerticalAlignment = VerticalAlignment.Stretch;
            mediaElement.Stretch = Stretch.Fill;
            mediaElement.StretchDirection = StretchDirection.Both;
        }
        /// <summary>
        /// 加载视频文件进行播放
        /// </summary>
        public void LoadMedia()
        {
            // 绑定视频文件
            mediaElement.Source = mediaUri;
        }

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            mediaElement_MouseDown(mediaElement, null);
        }
        private void mediaElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (palyBit == true)
            {
                mediaElement.Pause();
                palyBit = false;
            }
            else
            {
                media_Loaded(sender, e);
            }
        }
        /*
            元素事件 
        */
        private void media_Loaded(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
            palyBit = true;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            // MediaElement需要先停止播放才能再开始播放，
            // 否则会停在最后一帧不动
            //(sender as MediaElement).Stop();
            media_Unloaded(sender, e);
            media_Loaded(sender, e);
        }

        private void media_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mediaElement != null)
                {
                    mediaElement.Stop();
                }
                palyBit = false;
            }
            catch (Exception ex)
            {

            }
        }

        public void Dispose()
        {
            try
            {
                media_Unloaded(null, null);
                mediaElement = null;
            }
            catch (Exception ex)
            {

            }

        }
    }
}
