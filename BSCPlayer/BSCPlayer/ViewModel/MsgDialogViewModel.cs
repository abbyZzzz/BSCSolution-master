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

        #region ȫ������
        private RelayCommand submitCancelCmd;
        /// <summary>
        /// ִ���ύ����ķ���
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
        /// ִ���ύ����ķ���
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

        #region ��������
        /// <summary>
        /// ִ���ύ����
        /// </summary>
        private void ExcuteOkCommand()
        {
            hostWindow.Close();
        }
        /// <summary>
        /// ִ���ύ����
        /// </summary>
        private void ExcuteCancelCommand()
        {
            hostWindow.Close();
        }
        /// <summary>
        /// �Ƿ��ִ�У�����ñ��Ƿ���֤ͨ�����ж������Ƿ�ִ�У�
        /// </summary>
        /// <returns></returns>
        private bool CanExcute()
        {
            return true;
        }
        #endregion
    }
}