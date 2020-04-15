using System;
using System.Collections.Generic;
using System.Linq;
using Advantech.BSCWeb.APICRUD;
using Advantech.BSCWeb.APIData;
using Advantech.BSCWeb.Models;
using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Advantech.BSCWeb.Controllers
{
    public class ScheduleController : Controller
    {    
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetProgramSchedule()
        {
            List<ProgramSchedule> scheduleDays = ProgramScheduleCURD.Get();

            return Json(scheduleDays.OrderBy(s => s.schedule_name));
        }

        public IActionResult Edit(int schedule_id,string schedule_name) {

            List<ScheduleDay> scheduleDays = ScheduleDayCURD.Get();
            ViewBag.scheduleDays = scheduleDays;
            ViewBag.scheduleId = schedule_id;
            ViewBag.scheduleName = schedule_name;

            List<ProgramScheduleTime> res = ProgramScheduleTimeCURD.Get()
                .Where(s => s.schedule_id == schedule_id && s.primary_bit==1).ToList();
            if (res.Count > 0)
            {
                ViewBag.defaultProgram = res[0].schedule_day_id;
            }
            else {
                ViewBag.defaultProgram = "";
            }
            

            return View();
        }

        public IActionResult EditHandle(string schedule_name,int schedule_id,int default_program_id) {
            string events = HttpContext.Request.Form["myevents"].ToString();
            List<ProgramEvent> programEvents = JsonConvert.DeserializeObject<List<ProgramEvent>>(events);
            //保证对于一个排程存在ProgramScheduleTime中的schedule_day_id唯一
            List<string> eventTitle = programEvents.GroupBy(s => s.title).Select(s => s.Key).ToList();

            List<ProgramSchedule> programSchedules = ProgramScheduleCURD.Get();//判断标题名称是否重复
            List<ScheduleDay> scheduleDays = ScheduleDayCURD.Get();
            
            //新增
            if (schedule_id == 0)
            {
                int count = programSchedules.Where(s => s.schedule_name == schedule_name).ToList().Count;
                if (count > 0)
                {
                    return Json("Repeat");
                }
                else {
                    //ProgramSchedule新增
                    ProgramSchedule programSchedule = new ProgramSchedule();
                    programSchedule.schedule_name = schedule_name;
                    programSchedule.group_id = 1;
                    programSchedule.user_id = 1;
                    programSchedule.create_time = DateTime.Now.ToLocalTime();
                    int program_schedule_id = ProgramScheduleCURD.Insert(programSchedule);

                    if (program_schedule_id > 0)
                    {
                        ////ProgramScheduleTime新增

                        //获取events title中对应的id
                        
                        var scheduleDays1 = (from p in eventTitle
                                             join q in scheduleDays
                                on p equals q.schedule_name
                                select new {
                                    q.id,
                                    q.schedule_name
                                }).ToList();
                        bool flag = true;//判断默认排程是否在日历中，以避免programscheduletime中存在重复scheduleday
                        for (int i=0;i< scheduleDays1.Count;i++) {
                            ProgramScheduleTime programScheduleTime = new ProgramScheduleTime();
                            programScheduleTime.schedule_id = program_schedule_id;
                            programScheduleTime.schedule_day_id = scheduleDays1[i].id;
                            //若默认排程在日历中则primary_bit=1
                            if (scheduleDays1[i].id== default_program_id) {
                                programScheduleTime.primary_bit = 1;
                                flag = false;
                            }
                            int program_schedule_time_id = ProgramScheduleTimeCURD.Insert(programScheduleTime);
                            if (program_schedule_time_id > 0)
                            {
                                //ScheduleTimeDate新增
                                for (int j = 0; j < programEvents.Count; j++)
                                {
                                    
                                    if (programEvents[j].title == scheduleDays1[i].schedule_name)
                                    {
                                        //保证一天一笔数据
                                        if (programEvents[j].start == programEvents[j].end)
                                        {
                                            ScheduleTimeDate scheduleTimeDate = new ScheduleTimeDate();
                                            scheduleTimeDate.schedule_time_id = program_schedule_time_id;
                                            scheduleTimeDate.schedule_date = programEvents[j].start;
                                            int schedule_time_date_id = ScheduleTimeDateCURD.Insert(scheduleTimeDate);
                                            if (schedule_time_date_id <= 0)
                                            {
                                                return Json("Fail");
                                            }
                                        }
                                        else {
                                            TimeSpan span = programEvents[j].end.Subtract(programEvents[j].start);
                                            int dayDiff = span.Days + 1;
                                            for (int day=0;day<dayDiff;day++) {
                                                ScheduleTimeDate scheduleTimeDate1 = new ScheduleTimeDate();
                                                scheduleTimeDate1.schedule_time_id = program_schedule_time_id;
                                                scheduleTimeDate1.schedule_date = programEvents[j].start.AddDays(day);
                                                int schedule_time_date_id = ScheduleTimeDateCURD.Insert(scheduleTimeDate1);
                                                if (schedule_time_date_id <= 0)
                                                {
                                                    return Json("Fail");
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            else {
                                return Json("Fail");
                            }
                            
                        }
                        ////若默认排程不在日历中则额外添加
                        if (flag && default_program_id>0) {
                            ProgramScheduleTime programScheduleTime = new ProgramScheduleTime();
                            programScheduleTime.schedule_id = program_schedule_id;
                            programScheduleTime.schedule_day_id = default_program_id;
                            programScheduleTime.primary_bit = 1;
                            int id = ProgramScheduleTimeCURD.Insert(programScheduleTime);
                            if (id <= 0)
                            {
                                return Json("Fail");
                            }
                        }  

                    }
                    else {
                        return Json("Fail");
                    }
                }
            }
            //修改
            else {
                int count = programSchedules.Where(s => s.schedule_name == schedule_name && s.id!= schedule_id).ToList().Count;
                if (count > 0)
                {
                    return Json("Repeat");
                }
                else
                {
                    List<ProgramSchedule> programs=programSchedules.Where(s => s.id == schedule_id).ToList();
                    //ProgramSchedule更新
                    ProgramSchedule programSchedule = new ProgramSchedule();
                    programSchedule.schedule_name = schedule_name;
                    programSchedule.group_id = programs[0].group_id;
                    programSchedule.user_id = programs[0].user_id;
                    programSchedule.create_time = programs[0].create_time;
                    bool res = ProgramScheduleCURD.Update(programSchedule);

                    if (res)
                    {
                        //ProgramScheduleTime更新
                        List<ProgramScheduleTime> programScheduleTimes = ProgramScheduleTimeCURD.Get().Where(s=>s.schedule_id==schedule_id).ToList();
                        List<ProgramScheduleTime> deleteList = programScheduleTimes;
                        List<string> InsertList = eventTitle;
                        bool flag = true;
                        for (int i=0;i< programScheduleTimes.Count;i++) {
                            if (eventTitle.Contains(programScheduleTimes[i].schedule_day_name))
                            {
                                deleteList.Remove(programScheduleTimes[i]);
                                InsertList.Remove(programScheduleTimes[i].schedule_day_name);

                                //判断预设排程是否在里面,若在则更新
                                if (default_program_id == programScheduleTimes[i].schedule_day_id) {
                                    flag = false;
                                    ProgramScheduleTime programScheduleTime = new ProgramScheduleTime();
                                    programScheduleTime = programScheduleTimes[i];
                                    programScheduleTime.primary_bit = 1;
                                    bool defaultUpdate=ProgramScheduleTimeCURD.Update(programScheduleTime);
                                    if (!defaultUpdate) {
                                        return Json("Fail");
                                    }
                                }


                                //操作schedule_time_date(先删除再新增)
                                List<ScheduleTimeDate> scheduleTimeDates = ScheduleTimeDateCURD.Get().Where(s => s.schedule_time_id == programScheduleTimes[i].id).ToList();
                                for (int delete=0;delete< scheduleTimeDates.Count;delete++) {
                                    bool deleteRes = ScheduleTimeDateCURD.Delete(scheduleTimeDates[delete].id);
                                    if (!deleteRes) {
                                        return Json("Fail");
                                    }
                                }

                                //ScheduleTimeDate新增
                                for (int j = 0; j < programEvents.Count; j++)
                                {

                                    if (programEvents[j].title == programScheduleTimes[i].schedule_day_name)
                                    {
                                        //保证一天一笔数据
                                        if (programEvents[j].start == programEvents[j].end)
                                        {
                                            ScheduleTimeDate scheduleTimeDate = new ScheduleTimeDate();
                                            scheduleTimeDate.schedule_time_id = programScheduleTimes[i].id;
                                            scheduleTimeDate.schedule_date = programEvents[j].start;
                                            int schedule_time_date_id = ScheduleTimeDateCURD.Insert(scheduleTimeDate);
                                            if (schedule_time_date_id <= 0)
                                            {
                                                return Json("Fail");
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan span = programEvents[j].end.Subtract(programEvents[j].start);
                                            int dayDiff = span.Days + 1;
                                            for (int day = 0; day < dayDiff; day++)
                                            {
                                                ScheduleTimeDate scheduleTimeDate1 = new ScheduleTimeDate();
                                                scheduleTimeDate1.schedule_time_id = programScheduleTimes[i].id;
                                                scheduleTimeDate1.schedule_date = programEvents[j].start.AddDays(day);
                                                int schedule_time_date_id = ScheduleTimeDateCURD.Insert(scheduleTimeDate1);
                                                if (schedule_time_date_id <= 0)
                                                {
                                                    return Json("Fail");
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                        ////若默认排程不在日历中则额外添加
                        if (flag && default_program_id > 0)
                        {
                            ProgramScheduleTime programScheduleTime = new ProgramScheduleTime();
                            programScheduleTime.schedule_id = schedule_id;
                            programScheduleTime.schedule_day_id = default_program_id;
                            programScheduleTime.primary_bit = 1;
                            int id= ProgramScheduleTimeCURD.Insert(programScheduleTime);
                            if (id<=0) {
                                return Json("Fail");
                            }
                        }

                        //删除数据库中不在提交的数据中的数据
                        for (int i=0;i< deleteList.Count;i++) {
                            bool deleteRes = ScheduleTimeDateCURD.Delete(deleteList[i].id);
                            if (!deleteRes)
                            {
                                return Json("Fail");
                            }
                        }


                        for (int j = 0; j < programEvents.Count; j++) {
                            if (InsertList.Contains(programEvents[j].title))
                            {
                                List<ScheduleDay> scheduleDays1= scheduleDays.Where(s => s.schedule_name == programEvents[j].title).ToList();
                                ProgramScheduleTime programScheduleTime = new ProgramScheduleTime();
                                programScheduleTime.schedule_day_id = scheduleDays1[0].id;
                                int program_schedule_time_id = ProgramScheduleTimeCURD.Insert(programScheduleTime);
                                //保证一天一笔数据
                                if (programEvents[j].start == programEvents[j].end)
                                {
                                    ScheduleTimeDate scheduleTimeDate = new ScheduleTimeDate();
                                    scheduleTimeDate.schedule_time_id = program_schedule_time_id;
                                    scheduleTimeDate.schedule_date = programEvents[j].start;
                                    int schedule_time_date_id = ScheduleTimeDateCURD.Insert(scheduleTimeDate);
                                    if (schedule_time_date_id <= 0)
                                    {
                                        return Json("Fail");
                                    }
                                }
                                else
                                {
                                    TimeSpan span = programEvents[j].end.Subtract(programEvents[j].start);
                                    int dayDiff = span.Days + 1;
                                    for (int day = 0; day < dayDiff; day++)
                                    {
                                        ScheduleTimeDate scheduleTimeDate1 = new ScheduleTimeDate();
                                        scheduleTimeDate1.schedule_time_id = program_schedule_time_id;
                                        scheduleTimeDate1.schedule_date = programEvents[j].start.AddDays(day);
                                        int schedule_time_date_id = ScheduleTimeDateCURD.Insert(scheduleTimeDate1);
                                        if (schedule_time_date_id <= 0)
                                        {
                                            return Json("Fail");
                                        }
                                    }
                                }

                            }
                        }



                    }
                    else {
                        return Json("Fail");
                    }

                }
            }
            


            return Json("Success");
        }

        public IActionResult GetProgramScheduleTime(int schedule_id) {
            List<ProgramScheduleTime> res = ProgramScheduleTimeCURD.Get()
                .Where(s=>s.schedule_id== schedule_id).ToList();
            return Json(res);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelProgramSchedule(int id)
        {
            if (id == 0)
            {
                return Json("Fail");
            }
            else
            {
                List<ProgramSchedule> programSchedules = ProgramScheduleCURD.Get(id);
                if (programSchedules.Count > 0)
                {
                    bool result = ProgramScheduleCURD.Delete(id);
                    if (result)
                    {
                        List<ProgramScheduleTime> scheduleResult = ProgramScheduleTimeCURD.Get().Where(s => s.schedule_id == id).ToList();
                        if (scheduleResult.Count > 0)
                        {
                            bool flag = true;
                            for (int i = 0; i < scheduleResult.Count; i++)
                            {
                                
                                bool res = ProgramScheduleTimeCURD.Delete(scheduleResult[i].id);
                                if (!res)
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                return Json("Success");
                            }
                            else
                            {
                                return Json("Fail");
                            }
                        }
                        return Json("Success");
                    }
                    else
                    {
                        return Json("Fail");
                    }
                }
                else
                {
                    return Json("Fail");
                }
            }

        }

    }
}