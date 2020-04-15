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
    public class ScheduleDayProgramCURD
    {
        private static string url_schedule_day_program = GlobalParameter._AGENTURL + "/program/ScheduleDayProgram";
        //获取ScheduleDayProgram数据
        public static List<ScheduleDayProgram> Get(int id = 0)
        {
            string urls = "";
            if (id == 0)
            {
                urls = url_schedule_day_program + "/get/all/details";
            }
            else
            {
                urls = url_schedule_day_program + "/get/details/" + id;
            }

            string str = HttpHelper.HttpGet(urls);
            List<ScheduleDayProgram> result = JsonConvert.DeserializeObject<List<ScheduleDayProgram>>(str);
            return result;
        }
        //新增
        public static int Insert(ScheduleDayProgram obj)
        {
            string urls = url_schedule_day_program + "/insert";

            var objJson = JsonConvert.SerializeObject(obj);
            int result = Convert.ToInt32(HttpHelper.HttpPost(urls, objJson));
            return result;
        }
        //更新
        public static bool Update(ScheduleDayProgram obj)
        {
            string urls = url_schedule_day_program + "/update";

            var objJson = JsonConvert.SerializeObject(obj);
            bool result = Convert.ToBoolean(HttpHelper.HttpPut(urls, objJson));
            return result;
        }
        //删除
        public static bool Delete(int id)
        {
            string urls = url_schedule_day_program + "/delete?id=" + id;

            bool result = Convert.ToBoolean(HttpHelper.HttpDel(urls));
            return result;
        }
    }
}
