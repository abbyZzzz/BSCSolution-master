using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Service;
using Advantech.UtilsStandard.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Advantech.BSCPlayer.Schedule
{
    /// <summary>
    /// 排程任务
    /// </summary>
    public class ProgramScheduleManager : ProgramPlayProvider
    {
        private ProgramSchedule schedule;
        private readonly IProgramScheduleService _programScheduleService;
        private readonly IProgramInfoService _programInfoService;
        private readonly IMediaInfoService _mediaInfoService;
        private readonly ILogWrite _logWrite;
        /// <summary>
        /// 主窗体
        /// </summary>
        public Window mainWindow { set; get; }

        private Dictionary<int, ProgramRegionPlayManager> RegionManagerDic = new Dictionary<int, ProgramRegionPlayManager>();

        public ProgramScheduleManager(IProgramScheduleService programScheduleService,
                                        IProgramInfoService programInfoService,
                                        IMediaInfoService mediaInfoService,
                                        ILogWrite logWrite)
        {
            _programScheduleService = programScheduleService;
            _mediaInfoService = mediaInfoService;
            _programInfoService = programInfoService;
            _logWrite = logWrite;
        }

        //加载一个排程
        public void LoadSchedule()
        {
            var item = _programScheduleService.QueryableToEntity(x => x.id > 0);
            if (item != null)
            {
                schedule = _programScheduleService.GetCompositeById(item.id);//加载完整信息
            }
        }
        /// <summary>
        /// 开始播放任务
        /// </summary>
        public override void ProgramPlay()
        {
            DateTime dtNow;
            ProgramScheduleTime scheduleTime = null;

            while (!StopBit)
            {
                if (schedule == null)//未有排程，持续查询
                {
                    LoadSchedule();
                    if (schedule == null)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }
                }
                else if (PauseBit)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                dtNow = DateTime.Now;
                foreach (var item in schedule.TimeList)
                {
                    if (!string.IsNullOrEmpty(item.schedule_date))//播放的日期设定不为空，则为优先
                    {
                        if (dtNow.ToString("yyyy-MM-dd") == item.schedule_date)
                        {
                            scheduleTime = item;
                            break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.schedule_week))//星期设定不为空，则为优先
                    {
                        if (item.schedule_week.Contains(dtNow.DayOfWeek.ToString()))
                        {
                            scheduleTime = item;
                            break;
                        }
                    }
                }

                if (scheduleTime == null)//未有复合条件的，加载默认时刻
                {
                    scheduleTime = schedule.TimeList.FirstOrDefault(x => x.primary_bit == 1);//默认的排程
                }
                //时刻表未加载，或者时刻表变化，则加载
                if (scheduleTime != null)
                {
                    if(PresentPlayObject == null || 
                        (PresentPlayObject as ProgramScheduleTime) != scheduleTime)
                    {
                        RegionManagerDic = new Dictionary<int, ProgramRegionPlayManager>();
                        try
                        {
                            LoadScheduleDay(scheduleTime);
                            this.PresentPlayObject = scheduleTime;
                        }
                        catch (Exception ex)
                        {
                            _logWrite.WriteLog($"LoadScheduleDay出错", ex);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 加载单日时刻表
        /// </summary>
        /// <param name="programScheduleTime"></param>
        private void LoadScheduleDay(ProgramScheduleTime programScheduleTime)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtStart, dtEnd;
            ScheduleDayProgram dayProgram = null;
            Canvas canvasLayOut = null;
            ScheduleDay scheduleDay = programScheduleTime.scheduleDay;
            foreach (var dayProgramItem in scheduleDay.ProgramList)//查找具体时刻表里面符合时间范围的节目
            {
                string tmp = dtNow.ToShortDateString() + " " + dayProgramItem.start_time;//开始时间
                DateTime.TryParse(tmp, out dtStart);
                tmp = dtNow.ToShortDateString() + " " + dayProgramItem.end_time;//结束时间
                DateTime.TryParse(tmp, out dtEnd);
                if (dtNow >= dtStart && dtNow <= dtEnd)
                {
                    dayProgram = dayProgramItem;
                    _logWrite.WriteLog($"查找到ScheduleDayProgram对象id={dayProgram.id}");
                    break;
                }
            }
            if (dayProgram != null)//查找到正确的时刻
            {
                ProgramInfo programInfo = _programInfoService.GetCompositeById(dayProgram.program_id);
                if (programInfo != null)
                {
                    _logWrite.WriteLog($"开始加载节目信息={programInfo.id} {programInfo.program_name}");

                    mainWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        canvasLayOut = new Canvas();
                        mainWindow.Width = programInfo.w;   //窗体尺寸-宽
                        mainWindow.Height = programInfo.h;  //窗体尺寸-高                       
                        canvasLayOut.Width = programInfo.w;
                        canvasLayOut.Height = programInfo.h;
                    }));
                    this.PresentUIElement = canvasLayOut;

                    foreach (var region in programInfo.RegionList)//区块列表
                    {
                        if (!RegionManagerDic.ContainsKey(region.id))
                        {
                            ProgramRegionPlayManager regioManager = new ProgramRegionPlayManager(canvasLayOut, region, _mediaInfoService, _logWrite);
                            regioManager.PlayStart();
                            RegionManagerDic.Add(region.id, regioManager);
                        }
                    }

                    mainWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        mainWindow.Content = canvasLayOut;//画布赋值给窗体
                    }));
                }
                else
                {
                    _logWrite.WriteLog($"未查找到ProgramInfo对象,对象id={dayProgram.program_id}");
                }
            }
            else
            {
                _logWrite.WriteLog("未查找到ScheduleDayProgram对象");
            }
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public override void PlayPause()
        {
            foreach (var item in RegionManagerDic.Values)
            {
                item.PlayPause();
            }
            base.PlayPause();
        }
        /// <summary>
        /// 结束任务释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in RegionManagerDic.Values)
            {
                item.Dispose();
            }
            
        }
    }
}
