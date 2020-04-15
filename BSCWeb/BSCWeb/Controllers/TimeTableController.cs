using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advantech.CoreExtention;
using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Advantech.BSCWeb.Controllers
{
    public class TimeTableController : Controller
    {
        private string url_schedule_day_program = GlobalParameter._AGENTURL + "/program/ScheduleDayProgram";
        private string url_schedule_day = GlobalParameter._AGENTURL + "/program/ScheduleDay";
        private string url_media = GlobalParameter._AGENTURL + "/file/MediaInfo";
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetScheduleDay(int id = 0,int type=0)
        {
            string urls = url_schedule_day + "?id=" + id + "&type="+type;
            string jsonStr = HttpHelper.HttpGet(urls);
            List<ScheduleDay> scheduleDays = JsonConvert.DeserializeObject<List<ScheduleDay>>(jsonStr);

            return Json(scheduleDays.OrderBy(s => s.schedule_name));
        }

        [HttpPost]
        public IActionResult GetAjaxData(int media_group_id)
        {
            string urls = url_media + "?media_group_id=" + media_group_id;
            string jsonStr = HttpHelper.HttpGet(urls);
            List<MediaInfo> mediaInfos = JsonConvert.DeserializeObject<List<MediaInfo>>(jsonStr);
            return View(mediaInfos);
        }

        public IActionResult Edit(int id=0)
        {
            string jsonStr = HttpHelper.HttpGet(url_media);
            List<MediaInfo> mediaInfos = JsonConvert.DeserializeObject<List<MediaInfo>>(jsonStr);
            List<ScheduleDayProgram> scheduleDayPrograms = new List<ScheduleDayProgram>();
            if (id == 0)
            {
                
                ViewBag.scheduleDayPrograms = scheduleDayPrograms;
                return View(mediaInfos);
            }
            else {
                string scheduleDayUrl = url_schedule_day + "?id="+id;
                var scheduleDayStr = HttpHelper.HttpGet(scheduleDayUrl);
                List<ScheduleDay> scheduleDay = JsonConvert.DeserializeObject<List<ScheduleDay>>(scheduleDayStr);

                string scheduleDayProgramUrl = url_schedule_day_program + "?type=1";
                var scheduleDayProgramStr=HttpHelper.HttpGet(scheduleDayProgramUrl);
                List<ScheduleDayProgram> scheduleDayPrograms1 = JsonConvert.DeserializeObject<List<ScheduleDayProgram>>(scheduleDayProgramStr);

                for (int i=0;i< scheduleDayPrograms1.Count;i++) {
                    if (scheduleDay[0].id== scheduleDayPrograms1[i].schedule_id) {
                        scheduleDayPrograms.Add(scheduleDayPrograms1[i]);
                    }
                }

                ViewBag.scheduleName = scheduleDay[0].schedule_name;
                ViewBag.scheduleId = scheduleDay[0].id;
                ViewBag.scheduleDayPrograms = scheduleDayPrograms.OrderBy(s=>s.start_time).ToList();
                return View(mediaInfos);
            }
            
            
        }

        public IActionResult EditHandle(ScheduleDay scheduleDay) {
            
            string [] idRes=Convert.ToString(HttpContext.Request.Form["programIdRes"]).Split(',');
            string [] nameIdRes = Convert.ToString(HttpContext.Request.Form["programNameIdRes"]).Split(',');//将program的id和name组合起来，此处为program_id
            string [] startRes = Convert.ToString(HttpContext.Request.Form["programStartRes"]).Split(',');
            string [] endRes = Convert.ToString(HttpContext.Request.Form["programEndRes"]).Split(',');
            scheduleDay.create_time = DateTime.Now.ToLocalTime();
            var scheduleDayJson = JsonConvert.SerializeObject(scheduleDay);
            //获取ScheduleDay，避免标题重复
            var scheduleDayStr = HttpHelper.HttpGet(url_schedule_day);
            List<ScheduleDay> scheduleDaySearch = JsonConvert.DeserializeObject<List<ScheduleDay>>(scheduleDayStr);
            List<ScheduleDay> res = scheduleDaySearch.Where(s => s.schedule_name == scheduleDay.schedule_name && s.id!=scheduleDay.id).ToList();        

            if (scheduleDay.id > 0)
            {
                if (res.Count > 0)
                {
                    return Json("Repeat");
                }
                else {
                    string scheduleDayUrl = url_schedule_day + "?id=" + scheduleDay.id;
                    string jsonStr = HttpHelper.HttpGet(scheduleDayUrl);
                    List<ScheduleDay> scheduleDays = JsonConvert.DeserializeObject<List<ScheduleDay>>(jsonStr);

                    //更新ScheduleDay的schedule_name，其他保持不变
                    ScheduleDay schedule = new ScheduleDay();
                    schedule.id = scheduleDays[0].id;//不变
                    schedule.schedule_name = scheduleDay.schedule_name;//修改
                    schedule.group_id = scheduleDays[0].group_id;//不变
                    schedule.user_id = scheduleDays[0].user_id;//不变
                    schedule.create_time = scheduleDays[0].create_time;//不变
                    var scheduleJson = JsonConvert.SerializeObject(schedule);
                    bool result = Convert.ToBoolean(HttpHelper.HttpPut(url_schedule_day, scheduleJson));

                    if (result)
                    {
                        //获取ScheduleDayProgram,做新增、更新或删除操作
                        var scheduleDayProgramStr = HttpHelper.HttpGet(url_schedule_day_program);
                        List<ScheduleDayProgram> scheduleDayProgram1 = JsonConvert.DeserializeObject<List<ScheduleDayProgram>>(scheduleDayProgramStr);
                        //获取schedule_day下所有的program
                        List<ScheduleDayProgram> scheduleDayProgramSearch = scheduleDayProgram1.Where(s=>s.schedule_id==scheduleDay.id).ToList();
                        
                        List<ScheduleDayProgram> programIndex = new List<ScheduleDayProgram>();
                        //用于删除
                        List<ScheduleDayProgram> programDelete = scheduleDayProgramSearch;

                        bool flag = true;
                        if (idRes.Length > 1)
                        {
                            
                            for (int i = 0; i < idRes.Length; i++)
                            {
                                if (idRes[i].Contains("program"))
                                {
                                    ScheduleDayProgram scheduleDayProgram = new ScheduleDayProgram();
                                    scheduleDayProgram.schedule_id = scheduleDay.id;
                                    scheduleDayProgram.program_id = Convert.ToInt32(nameIdRes[i]);
                                    scheduleDayProgram.start_time = startRes[i];
                                    scheduleDayProgram.end_time = endRes[i];

                                    var scheduleDayProgramJson = JsonConvert.SerializeObject(scheduleDayProgram);
                                    int schedule_day_program_id = Convert.ToInt32(HttpHelper.HttpPost(url_schedule_day_program, scheduleDayProgramJson));
                                    if (schedule_day_program_id <= 0)
                                    {
                                        flag = false;
                                    }
                                }
                                else {
                                    programIndex = scheduleDayProgramSearch.Where(s => s.id == Convert.ToInt32(idRes[i])).ToList();
                                    //修改
                                    if (programIndex.Count > 0)
                                    {
                                        //将修改后的元素从查询出来的scheduleDayProgram中删除
                                        programDelete.RemoveAll(s=>s.id==programIndex[0].id);

                                        ScheduleDayProgram scheduleDayProgram = new ScheduleDayProgram();
                                        scheduleDayProgram.id = programIndex[0].id;//保持不变
                                        scheduleDayProgram.schedule_id = programIndex[0].schedule_id;//保持不变
                                        scheduleDayProgram.program_id = programIndex[0].program_id;//保持不变
                                        scheduleDayProgram.start_time = startRes[i];
                                        scheduleDayProgram.end_time = endRes[i];

                                        var scheduleDayProgramJson = JsonConvert.SerializeObject(scheduleDayProgram);
                                        bool res1 = Convert.ToBoolean(HttpHelper.HttpPut(url_schedule_day_program, scheduleDayProgramJson));
                                        if (!res1)
                                        {
                                            flag = false;
                                        }
                                    }
                                }
                            }
                        }

                        //删除
                        if (programDelete.Count > 0)
                        {
                            for (int x = 0; x < programDelete.Count; x++)
                            {
                                string urls = url_schedule_day_program + "?id=" + programDelete[x].id;
                                bool resultDelete = Convert.ToBoolean(HttpHelper.HttpDel(urls));
                                if (!resultDelete)
                                {
                                    flag=false;
                                }
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
                    else
                    {
                        return Json("Fail");
                    }
                }
            }
            else {
                if (res.Count > 0)
                {
                    return Json("Repeat");
                }
                else {
                    
                    int schedule_id = Convert.ToInt32(HttpHelper.HttpPost(url_schedule_day, scheduleDayJson));
                    if (schedule_id>0) {
                        if (idRes.Length>1) {
                            bool flag = true;
                            for (int i = 0; i < idRes.Length; i++)
                            {
                                ScheduleDayProgram scheduleDayProgram = new ScheduleDayProgram();
                                scheduleDayProgram.schedule_id = schedule_id;
                                scheduleDayProgram.program_id = Convert.ToInt32(nameIdRes[i]);
                                scheduleDayProgram.start_time = startRes[i];
                                scheduleDayProgram.end_time = endRes[i];

                                var scheduleDayProgramJson = JsonConvert.SerializeObject(scheduleDayProgram);
                                int schedule_day_program_id = Convert.ToInt32(HttpHelper.HttpPost(url_schedule_day_program, scheduleDayProgramJson));
                                
                                
                                if (schedule_day_program_id <= 0)
                                {
                                    flag=false;
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
                        
                    }
                    else {
                        return Json("Fail");
                    }
                }
                return Json("Success");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelScheduleDay(int id)
        {
            if (id == 0)
            {
                return Json("Fail");
            }
            else {
                string urls = url_schedule_day + "?id=" + id;
                string jsonStr = HttpHelper.HttpGet(urls);
                List<ScheduleDay> scheduleDays = JsonConvert.DeserializeObject<List<ScheduleDay>>(jsonStr);
                if (scheduleDays.Count > 0)
                {
                    bool result = Convert.ToBoolean(HttpHelper.HttpDel(urls));
                    if (result)
                    {
                        string programStr = HttpHelper.HttpGet(url_schedule_day_program);
                        List <ScheduleDayProgram> scheduleDays1= JsonConvert.DeserializeObject<List<ScheduleDayProgram>>(programStr);
                        List<ScheduleDayProgram> scheduleResult = scheduleDays1.Where(s => s.schedule_id == id).ToList();
                        if (scheduleResult.Count>0) {
                            bool flag = true;
                            for (int i=0;i< scheduleResult.Count;i++) {
                                string myUrl = url_schedule_day_program + "?id=" + scheduleResult[i].id;
                                bool res = Convert.ToBoolean(HttpHelper.HttpDel(myUrl));
                                if (!res) {
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
                    else {
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