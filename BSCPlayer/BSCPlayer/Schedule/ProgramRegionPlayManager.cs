using Advantech.CommonDefine;
using Advantech.Entity;
using Advantech.Service;
using Advantech.UtilsStandard.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Advantech.BSCPlayer.Schedule
{
    /// <summary>
    /// 区域节目播放
    /// </summary>
    public class ProgramRegionPlayManager : ProgramPlayProvider
    {
        private string LocalPath = ConfigurationManager.AppSettings["Media_Path"];//本地配置路径
        private Canvas canvasLayout;
        private readonly ProgramRegion programRegion;
        private List<ProgramRegionMedia> playList;
        private IMediaInfoService mediaInfoService;
        private readonly ILogWrite _logWrite;
        private UIElement LastUIElement = null;//上一次的播放组件

        public ProgramRegionPlayManager(Canvas canvas, ProgramRegion programregion, IMediaInfoService mediaService, ILogWrite logWrite)
        {
            canvasLayout = canvas;
            programRegion = programregion;
            mediaInfoService = mediaService;
            playList = programRegion.MediaList.OrderBy(x => x.play_order).ToList();//排序
            _logWrite = logWrite;
        }
        /// <summary>
        /// 节目播放
        /// </summary>
        public override void ProgramPlay()
        {
            int disDelay = 0;//播放延时
            while (!StopBit)
            {
                if (PauseBit)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (programRegion.MediaList.Count > 0)
                {
                    ProgramRegionMedia media = playList[this.PlayIndex];
                    if (PresentUIElement == null)//未加载组件
                    {
                        MediaInfo mediaInfo = mediaInfoService.QueryableToEntity(x => x.id == media.media_id);
                        if (mediaInfo != null)
                        {
                            string mediaPath = LocalPath + mediaInfo.serial_number;
                            FileType mediaType = (FileType)Enum.Parse(typeof(FileType), mediaInfo.media_type, true);

                            _logWrite.WriteLog($"加入组件");
                            switch (mediaType)
                            {
                                case FileType.image:
                                    canvasLayout.Dispatcher.Invoke(new Action(() => {
                                        PresentUIElement = canvasLayout.AddImageComponent(mediaPath, programRegion, false);
                                    }));
                                    break;
                                case FileType.pdf:
                                    canvasLayout.Dispatcher.Invoke(new Action(() =>
                                    {
                                        PresentUIElement = canvasLayout.AddPdfComponent(mediaPath, programRegion, false);
                                    }));
                                    break;
                                case FileType.video:
                                    canvasLayout.Dispatcher.Invoke(new Action(() =>
                                    {
                                        PresentUIElement = canvasLayout.AddMediaComponent(mediaPath, programRegion, false);
                                    }));
                                    break;
                                case FileType.html:
                                    canvasLayout.Dispatcher.Invoke(new Action(() =>
                                    {
                                        PresentUIElement = canvasLayout.AddWebComponent(mediaPath, programRegion, false);
                                    }));
                                    break;
                                case FileType.ppt:
                                    canvasLayout.Dispatcher.Invoke(new Action(() =>
                                    {
                                        PresentUIElement = canvasLayout.AddPPTComponent(mediaPath, programRegion, false);
                                    }));
                                    break;
                                default:
                                    break;
                            }
                            disDelay = 0;
                            this.PresentPlayObject = mediaInfo;//当前播放的对象
                        }
                        else
                        {
                            _logWrite.WriteLog($"查找媒体资源失败，media.media_id={media.media_id}");
                        }
                    }
                    else//播放组件已加载，开始计时播放
                    {
                        ++media.play_second_count;//计时累计
                        if (media.play_second > 0 && media.play_second_count >= media.play_second)//时间到达
                        {
                            media.play_second_count = 0;
                            PlayIndex += 1;
                            LastUIElement = PresentUIElement;//上一个播放组件记忆

                            this.PresentUIElement.DisposeResource();
                            this.PresentUIElement = null;//当前播放的组件清除
                            this.PresentPlayObject = null;//当前播放的对象清除
                        }

                        if (PlayIndex >= programRegion.MediaList.Count)
                        {
                            PlayIndex = 0;//序号回滚
                        }
                    }

                    if (PresentUIElement != null && PresentUIElement.Visibility == Visibility.Hidden)
                    {
                        ++disDelay;
                        canvasLayout.Dispatcher.Invoke(new Action(() =>
                        {
                            if (disDelay >= 3 || LastUIElement == null)
                            {
                                if (LastUIElement != null && canvasLayout.Children.Contains(LastUIElement))
                                {
                                    LastUIElement.DisposeResource();//资源释放
                                    canvasLayout.Children.Remove(LastUIElement);//移除上一次加入组件
                                    _logWrite.WriteLog($"播放组件移除");
                                    LastUIElement = null;
                                }
                                PresentUIElement.SetVisible(true);
                            }
                        }));
                    }
                }

                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 移除组件播放停止
        /// </summary>
        public override void PlayStop()
        {
            base.PlayStop();

            if (canvasLayout.Children.Contains(PresentUIElement))
            {
                canvasLayout.Dispatcher.Invoke(new Action(() =>
                {
                    canvasLayout.Children.Remove(PresentUIElement);//移除上一次加入组件
                }));
                _logWrite.WriteLog($"节目播放停止，播放组件移除");
            }
        }
    }
}
