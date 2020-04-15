using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Advantech.CoreExtention
{
    public class Shared
    {
        /// <summary>
        /// 截取字符中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ExtractNum(string str)
        {
            string result=  System.Text.RegularExpressions.Regex.Replace(str, @"[^0-9]+", "");
            if (!string.IsNullOrEmpty(result))
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNum(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        /// <summary>
        /// 只显示10个字符串，其余用..代替
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string SubStringName(string str,int maxLength = 10)
        {
            str = str.Length > maxLength ? str.Substring(0, maxLength) + "..." : str;
            return str;
        }
    }
}
