using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Advantech.BSCWeb.APICRUD
{
    public class ScheduleTimeDateCURD
    {
        private static string url_schedule_time_date = GlobalParameter._AGENTURL + "/program/ScheduleTimeDate";
        //获取ProgramSchedule数据
        public static List<ScheduleTimeDate> Get(int id = 0)
        {
            string urls = "";
            if (id == 0)
            {
                urls = url_schedule_time_date + "/get/all/details";
            }
            else
            {
                urls = url_schedule_time_date + "/get/details/" + id;
            }

            string str = HttpHelper.HttpGet(urls);
            List<ScheduleTimeDate> result = JsonConvert.DeserializeObject<List<ScheduleTimeDate>>(str);
            return result;
        }
        //新增
        public static int Insert(ScheduleTimeDate obj)
        {
            string urls = url_schedule_time_date + "/insert";

            var objJson = JsonConvert.SerializeObject(obj);
            int result = Convert.ToInt32(HttpHelper.HttpPost(urls, objJson));
            return result;
        }
        //更新
        public static bool Update(ScheduleTimeDate obj)
        {
            string urls = url_schedule_time_date + "/update";

            var objJson = JsonConvert.SerializeObject(obj);
            bool result = Convert.ToBoolean(HttpHelper.HttpPut(urls, objJson));
            return result;
        }
        //删除
        public static bool Delete(int id)
        {
            string urls = url_schedule_time_date + "/delete?id=" + id;

            bool result = Convert.ToBoolean(HttpHelper.HttpDel(urls));
            return result;
        }
    }
}
