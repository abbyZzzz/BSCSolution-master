using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Advantech.BSCPlayer.Schedule
{
    /// <summary>
    /// 播放接口
    /// </summary>
    public interface IProgramPlayProvider: IDisposable
    {
        /// <summary>
        /// 当前播放顺序号
        /// </summary>
        int PlayIndex { set; get; }
        /// <summary>
        /// 暂停标识
        /// </summary>
        bool PauseBit { set; get; }
        /// <summary>
        /// 停止任务
        /// </summary>
        bool StopBit { set; get; }
        /// <summary>
        /// 当前播放的对象，媒体/排程
        /// </summary>
        object PresentPlayObject { set; get; }

        void PlayStart();
        void PlayStop();
        void PlayPause();
        void ProgramPlay();
    }
    /// <summary>
    /// 播放抽象类
    /// </summary>
    public abstract class ProgramPlayProvider : IProgramPlayProvider
    {
        #region 属性
        private int playIndex = 0;//播放顺序
        public int PlayIndex
        {
            get => playIndex;
            set => playIndex = value;
        }
        private bool pauseBit = false;
        public bool PauseBit
        {
            get => pauseBit;
            set => pauseBit = value;
        }
        private bool stopBit = false;
        public bool StopBit
        {
            get => stopBit;
            set => stopBit = value;
        }
        private Thread playerTask;
        public Thread PlayerTask
        {
            get => playerTask;
            set => playerTask = value;
        }
        private object presentPlayObject;
        /// <summary>
        /// 当前播放的信息，媒体/节目
        /// </summary>
        public object PresentPlayObject 
        { 
            get => presentPlayObject;
            set => presentPlayObject=value;
        }
        private UIElement uiElement;
        /// <summary>
        /// 当前播放的元素，画布/组件
        /// </summary>
        public UIElement PresentUIElement
        {
            get => uiElement;
            set => uiElement = value;
        }
        #endregion

        #region 方法
        public virtual void PlayPause()
        {
            PauseBit = !PauseBit;
        }

        public virtual void PlayStart()
        {
            if (playerTask == null)
            {
                playerTask = new Thread(() => ProgramPlay());
            }
            playerTask.Start();
        }

        public virtual void PlayStop()
        {
            StopBit = true;
            if (playerTask != null)
            {
                try
                {
                    playerTask.Join();
                }
                catch(Exception ex) { }
            }
        }

        public virtual void ProgramPlay()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            PlayStop();
            playerTask = null;
        }
        #endregion
    }
}
