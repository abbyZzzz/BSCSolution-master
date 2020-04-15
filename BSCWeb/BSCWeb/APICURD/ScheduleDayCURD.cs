using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.BSCWeb.APIData
{
    public class ScheduleDayCURD
    {
        private static string url_schedule_day = GlobalParameter._AGENTURL + "/program/ScheduleDay";
        //获取ScheduleDay数据
        public static List<ScheduleDay> Get(int id = 0)
        {
            string urls = "";
            if (id == 0)
            {
                urls = url_schedule_day + "/get/all/details";
            }
            else
            {
                urls = url_schedule_day + "/get/details/" + id;
            }

            string str = HttpHelper.HttpGet(urls);
            List<ScheduleDay> result = JsonConvert.DeserializeObject<List<ScheduleDay>>(str);
            return result;
        }
        //新增
        public static int Insert(ScheduleDay obj)
        {
            string urls = url_schedule_day + "/insert";

            var objJson = JsonConvert.SerializeObject(obj);
            int result = Convert.ToInt32(HttpHelper.HttpPost(urls, objJson));
            return result;
        }
        //更新
        public static bool Update(ScheduleDay obj)
        {
            string urls = url_schedule_day + "/update";

            var objJson = JsonConvert.SerializeObject(obj);
            bool result = Convert.ToBoolean(HttpHelper.HttpPut(urls, objJson));
            return result;
        }
        //删除
        public static bool Delete(int id)
        {
            string urls = url_schedule_day + "/delete?id=" + id;

            bool result = Convert.ToBoolean(HttpHelper.HttpDel(urls));
            return result;
        }
    }
}
