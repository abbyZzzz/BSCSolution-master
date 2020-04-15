using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.BSCWeb.APICRUD
{
    public class ProgramScheduleTimeCURD
    {
        private static string url_program_schedule_time = GlobalParameter._AGENTURL + "/program/ProgramScheduleTime";
        //获取ProgramSchedule数据
        public static List<ProgramScheduleTime> Get(int id = 0)
        {
            string urls = "";
            if (id == 0)
            {
                urls = url_program_schedule_time + "/get/all/details";
            }
            else
            {
                urls = url_program_schedule_time + "/get/details/" + id;
            }

            string str = HttpHelper.HttpGet(urls);
            List<ProgramScheduleTime> result = JsonConvert.DeserializeObject<List<ProgramScheduleTime>>(str);
            return result;
        }
        //新增
        public static int Insert(ProgramScheduleTime obj)
        {
            string urls = url_program_schedule_time + "/insert";

            var objJson = JsonConvert.SerializeObject(obj);
            int result = Convert.ToInt32(HttpHelper.HttpPost(urls, objJson));
            return result;
        }
        //更新
        public static bool Update(ProgramScheduleTime obj)
        {
            string urls = url_program_schedule_time + "/update";

            var objJson = JsonConvert.SerializeObject(obj);
            bool result = Convert.ToBoolean(HttpHelper.HttpPut(urls, objJson));
            return result;
        }
        //删除
        public static bool Delete(int id)
        {
            string urls = url_program_schedule_time + "/delete?id=" + id;

            bool result = Convert.ToBoolean(HttpHelper.HttpDel(urls));
            return result;
        }
    }
}
