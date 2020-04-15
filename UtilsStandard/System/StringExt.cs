using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.UtilsStandardLib.System
{
    public static class StringExt
    {
        /// <summary>
        /// 按照utf8从Base64解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String DecodeBase64(this string content)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(content);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                
            }
            return decode;
        }
        /// <summary>
        /// 按照utf8进行编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string EncodeBase64(this string content)
        {
            string encode = "";
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                
            }
            return encode;
        }
    }
}
