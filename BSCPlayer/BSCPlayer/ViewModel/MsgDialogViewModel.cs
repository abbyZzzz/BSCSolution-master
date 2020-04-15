using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;

namespace Advantech.BSCPlayer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MsgDialogViewModel : ViewModelBase
    {
        private Window hostWindow;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MsgDialogViewModel(Window window)
        {
            hostWindow = window;
        }
        //private Action closeAction;
        public Action CloseAction { set; get; }

        #region 全局命令
        private RelayCommand submitCancelCmd;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand SubmitCancelCmd
        {
            get
            {
                if (submitCancelCmd == null) return new RelayCommand(() => ExcuteCancelCommand(), CanExcute);
                return submitCancelCmd;
            }
            set { submitCancelCmd = value; }
        }
        private RelayCommand submitOkCmd;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand SubmitOkCmd
        {
            get
            {
                if (submitOkCmd == null) return new RelayCommand(() => ExcuteOkCommand(), CanExcute);
                return submitOkCmd;
            }
            set { submitOkCmd = value; }
        }
        #endregion

        #region 附属方法
        /// <summary>
        /// 执行提交方法
        /// </summary>
        private void ExcuteOkCommand()
        {
            hostWindow.Close();
        }
        /// <summary>
        /// 执行提交方法
        /// </summary>
        private void ExcuteCancelCommand()
        {
            hostWindow.Close();
        }
        /// <summary>
        /// 是否可执行（这边用表单是否验证通过来判断命令是否执行）
        /// </summary>
        /// <returns></returns>
        private bool CanExcute()
        {
            return true;
        }
        #endregion
    }
}