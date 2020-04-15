using Advantech.PlayerComponent;
using Advantech.Entity;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using System.Windows;

namespace Advantech.BSCPlayer
{
    public static class LayoutExtention
    {
        /// <summary>
        /// 增加图片组件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static UIElement AddImageComponent(this Canvas layout, string SourcePath, ProgramRegion Region, bool Visible = true)
        {
            Image imageCtrl = new Image();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(SourcePath);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(imageCtrl, image);
            Canvas.SetLeft(imageCtrl, Region.x);
            Canvas.SetTop(imageCtrl, Region.y);
            imageCtrl.Width = Region.w;
            imageCtrl.Height = Region.h;
            imageCtrl.Stretch = Stretch.Fill;
            imageCtrl.StretchDirection = StretchDirection.Both;
            imageCtrl.Visibility = Visible == true ? Visibility.Visible : Visibility.Hidden;
            layout.Children.Add(imageCtrl);
            return imageCtrl;
        }
        /// <summary>
        /// 增加pdf组件
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement AddPdfComponent(this Canvas layout, string SourcePath, ProgramRegion Region, bool Visible = true)
        {
            ChromiumWebBrowser chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Address = SourcePath;
            Canvas.SetLeft(chromiumWebBrowser, Region.x);
            Canvas.SetTop(chromiumWebBrowser, Region.y);
            chromiumWebBrowser.Width = Region.w;
            chromiumWebBrowser.Height = Region.h;
            chromiumWebBrowser.Visibility = Visible == true ? Visibility.Visible : Visibility.Hidden;
            layout.Children.Add(chromiumWebBrowser);
            return chromiumWebBrowser;
        }
        /// <summary>
        /// 增加web组件
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement AddWebComponent(this Canvas layout, string SourcePath, ProgramRegion Region, bool Visible = true)
        {
            ChromiumWebBrowser chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Address = SourcePath;
            Canvas.SetLeft(chromiumWebBrowser, Region.x);
            Canvas.SetTop(chromiumWebBrowser, Region.y);
            chromiumWebBrowser.Width = Region.w;
            chromiumWebBrowser.Height = Region.h;
            chromiumWebBrowser.Visibility = Visible == true ? Visibility.Visible : Visibility.Hidden;
            layout.Children.Add(chromiumWebBrowser);
            return chromiumWebBrowser;
        }
        /// <summary>
        /// 增加media组件
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement AddMediaComponent(this Canvas layout, string SourcePath, ProgramRegion Region, bool Visible = true)
        {
            MediaControl mediaControl = new MediaControl();
            mediaControl.mediaUri = new Uri(SourcePath);
            Canvas.SetLeft(mediaControl, Region.x);
            Canvas.SetTop(mediaControl, Region.y);
            mediaControl.Width = Region.w;
            mediaControl.Height = Region.h;
            mediaControl.Visibility = Visible == true ? Visibility.Visible : Visibility.Hidden;
            layout.Children.Add(mediaControl);
            mediaControl.LoadMedia();
            return mediaControl;
        }
        /// <summary>
        /// 增加PPT组件
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement AddPPTComponent(this Canvas layout, string SourcePath, ProgramRegion Region, bool Visible = true)
        {
            AppControl appControl = new AppControl();
            appControl.ExeName = @"";
            appControl.CtrlLocation = new System.Windows.Point() { X = Region.x, Y = Region.y };
            appControl.Width = Region.w;
            appControl.Height = Region.h;
            appControl.Visibility = Visible == true ? Visibility.Visible : Visibility.Hidden;
            layout.Children.Add(appControl);
            appControl.LoadExe(SourcePath);
            return appControl;
        }

        /// <summary>
        /// 增加外部exe组件
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement AddExeComponent(this Canvas layout, string SourcePath, string Args, ProgramRegion Region)
        {
            AppControl appControl = new AppControl();
            appControl.ExeName = SourcePath;
            appControl.CtrlLocation = new System.Windows.Point() { X = Region.x, Y = Region.y };
            appControl.Width = Region.w;
            appControl.Height = Region.h;
            layout.Children.Add(appControl);//先放入窗体内部
            appControl.LoadExe(Args);      //再执行加载exe
            return appControl;
        }
        /// <summary>
        /// 设置组件的隐藏
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement SetVisible(this UIElement uIElement, bool Visible = true)
        {
            uIElement.Dispatcher.Invoke(new Action(() =>
            {
                uIElement.Visibility = Visible == true ? Visibility.Visible : Visibility.Hidden;
            }));
            return uIElement;
        }
        /// <summary>
        /// 清除组件内部资源
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="SourcePath"></param>
        /// <param name="Region"></param>
        public static UIElement DisposeResource(this UIElement uIElement)
        {
            uIElement.Dispatcher.Invoke(new Action(() =>
            {
                if (uIElement is MediaControl)
                {
                    MediaControl mediaControl = uIElement as MediaControl;
                    if (mediaControl != null)
                    {
                        mediaControl.Dispose();
                        mediaControl = null;
                    }
                }
                else if (uIElement is Image)
                {
                    Image image = uIElement as Image;
                    if (image != null)
                    {
                        image = null;
                    }
                }
                else if (uIElement is ChromiumWebBrowser)
                {
                    ChromiumWebBrowser chromiumWebBrowser = uIElement as ChromiumWebBrowser;
                    if (chromiumWebBrowser != null)
                    {
                        chromiumWebBrowser.Dispose();
                    }
                    chromiumWebBrowser = null;
                }

            }));
            return uIElement;
        }
    }
}
