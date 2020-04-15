using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Advantech.PlayerComponent
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class AppControl : UserControl, IDisposable
    {
        #region 外部事件
        public event EventHandler ProcessErrorRaised;

        private void OnProcessErrorRaised(string message)
        {
            ErrMsgArgs eventArgs = new ErrMsgArgs(message);

            ProcessErrorRaised?.Invoke(this, eventArgs);
        }
        /// <summary>
        /// Process启动事件，请在LoadExe前进行绑定
        /// </summary>
        public event EventHandler HostProcessStarted;

        private void OnHostProcessStarted()
        {
            HostProcessStarted?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        /// <summary>
        /// 位置坐标
        /// </summary>
        public Point CtrlLocation { set; get; }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ExeName { get; set; }

        /// <summary>
        /// Track if the control is disposed
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// Handle to the application Window
        /// </summary>
        private IntPtr appProcessWinHandle;

        private Process appProcess;

        public AppControl()
        {
            InitializeComponent();
            this.Unloaded += AppControl_Unloaded;
            this.SizeChanged += AppControl_SizeChanged;
        }

        ~AppControl()
        {
            this.Dispose();
        }
        /// <summary>
        /// 加载外部程序
        /// </summary>
        /// <param name="Args">参数</param>
        public void LoadExe(string Args)
        {
            int count = 0;
            if (appProcess != null)
            {
                StopAppProcess();
            }

            if (string.IsNullOrEmpty(ExeName))
            {
                return;
            }

            Application.Current.Exit -= Current_Exit;
            Application.Current.Exit += Current_Exit;
            
            try
            {
                appProcessWinHandle = IntPtr.Zero;
                var procInfo = new ProcessStartInfo(this.ExeName);
                procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(this.ExeName);
                procInfo.WindowStyle = ProcessWindowStyle.Maximized;
                if(!string.IsNullOrEmpty(Args))
                {
                    procInfo.Arguments = Args;
                }
                appProcess = Process.Start(procInfo);

                bool res=appProcess.WaitForInputIdle(6*1000);//最大超时10s
                if (res)
                {
                    while (appProcess.MainWindowHandle.ToInt32() == 0)
                    {
                        Thread.Sleep(500);
                        ++count;
                        if(count>=10)
                        {
                            OnProcessErrorRaised("wait for appProcess.MainWindowHandle time out ");
                            break;
                        }
                        Console.WriteLine("-------------appProcess.MainWindowHandle.ToInt32() == 0");
                    }

                    this.appProcessWinHandle = appProcess.MainWindowHandle;
                    Window win = Window.GetWindow(this);
                    if (win != null)
                    {
                        var helper = new WindowInteropHelper(win);

                        Win32Api.SetParent(appProcessWinHandle, helper.Handle);
                        Win32Api.SetWindowLongA(appProcessWinHandle, Win32Api.GWL_STYLE, Win32Api.WS_VISIBLE);

                        if (appProcess != null && appProcess.HasExited == false)
                        {
                            OnHostProcessStarted();
                        }

                        UpdateSize();
                    }
                    else
                    {
                        OnProcessErrorRaised("窗体对象为空，请先放入窗体内部再执行加载exe");
                        Console.WriteLine("窗体对象为空，请先放入窗体内部再执行加载exe");
                    }
                }
                else
                {
                    OnProcessErrorRaised("未能等待到空闲状态");
                    Console.WriteLine("未能等待到空闲状态");
                }
            }
            catch (Exception ex)
            {
                OnProcessErrorRaised(ex.Message + "Error");
                Console.WriteLine(ex.Message + "Error");
                // 出錯了，把自己隱藏起來
                this.Visibility = Visibility.Collapsed;
            }
        }
        
        private void AppControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit -= Current_Exit;
            this.Dispose();
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            this.Dispose();
        }

        private void AppControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (this.appProcessWinHandle != IntPtr.Zero)
            {
                PresentationSource source = PresentationSource.FromVisual(this);

                var scaleX = 1D;
                var scaleY = 1D;
                if (source != null)
                {
                    scaleX = source.CompositionTarget.TransformToDevice.M11;
                    scaleY = source.CompositionTarget.TransformToDevice.M22;
                }
                
                var width = (int)(this.ActualWidth * scaleX);
                var height = (int)(this.ActualHeight * scaleY);

                //Win32Api.MoveWindow(appWinPtr, 0, 0, width, height, true);
                Win32Api.MoveWindow(appProcessWinHandle, (int)CtrlLocation.X, (int)CtrlLocation.Y, width, height, true);
            }
        }
        /// <summary>
        /// Close <code>AppFilename</code> 
        /// <para>将属性<code>AppFilename</code>指向的应用程序关闭</para>
        /// </summary>
        public void StopAppProcess()
        {
            if (appProcess != null)// && AppProcess.MainWindowHandle != IntPtr.Zero)
            {
                try
                {
                    if (!appProcess.HasExited)
                        appProcess.Kill();
                    if (appProcessWinHandle != IntPtr.Zero)
                    {
                        appProcessWinHandle = IntPtr.Zero;
                    }
                }
                catch (Exception)
                {
                }
                appProcess = null;
            }
        }
        protected void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    StopAppProcess();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);//上面已经释放，通知系统不要重复释放
        }
    }

    
}
