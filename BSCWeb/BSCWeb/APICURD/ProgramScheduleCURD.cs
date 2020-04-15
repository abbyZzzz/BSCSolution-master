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
    public class ProgramScheduleCURD
    {
        private static string url_program_schedule = GlobalParameter._AGENTURL + "/program/ProgramSchedule";
        //获取ProgramSchedule数据
        public static List<ProgramSchedule> Get(int id = 0)
        {
            string urls = "";
            if (id == 0)
            {
                urls = url_program_schedule + "/get/all/details";
            }
            else
            {
                urls = url_program_schedule + "/get/details/" + id;
            }

            string str = HttpHelper.HttpGet(urls);
            List<ProgramSchedule> result = JsonConvert.DeserializeObject<List<ProgramSchedule>>(str);
            return result;
        }
        //新增
        public static int Insert(ProgramSchedule obj)
        {
            string urls = url_program_schedule + "/insert";

            var objJson = JsonConvert.SerializeObject(obj);
            int result = Convert.ToInt32(HttpHelper.HttpPost(urls, objJson));
            return result;
        }
        //更新
        public static bool Update(ProgramSchedule obj)
        {
            string urls = url_program_schedule + "/update";

            var objJson = JsonConvert.SerializeObject(obj);
            bool result = Convert.ToBoolean(HttpHelper.HttpPut(urls, objJson));
            return result;
        }
        //删除
        public static bool Delete(int id)
        {
            string urls = url_program_schedule + "/delete?id=" + id;

            bool result = Convert.ToBoolean(HttpHelper.HttpDel(urls));
            return result;
        }
    }
}
