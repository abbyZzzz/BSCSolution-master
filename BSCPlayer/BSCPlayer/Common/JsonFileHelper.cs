using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantech.AppCommon
{
    public class JsonFileHelper
    {
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static T LoadJsonFromFile<T>(string FilePath)
        {
            try
            {
                using (StreamReader r = new StreamReader(FilePath))
                {
                    var json = r.ReadToEnd();
                    var t = JsonConvert.DeserializeObject<T>(json);
                    //foreach (var item in items)
                    //{
                    //    // Console.WriteLine("{0} {1}", item.temp, item.vcc);
                    //}
                    return t;
                }
            }
            catch(Exception ex)
            { }
            return default(T);
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool WriteJsonToFile<T>(T t,string FilePath)
        {
            try
            {
                var s = JsonConvert.SerializeObject(t);
                File.WriteAllText(FilePath, s);
                return true;
            }
            catch(Exception ex)
            { }
            return false;
        }
    }
}
