using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantech.BSCPlayer.Common
{
    public class ServerCfg
    {
        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public static string FileServerAddr;
        /// <summary>
        /// 数据服务地址
        /// </summary>
        public static string DataServerAddr;

        public static void LoadServerSettings()
        {
            FileServerAddr = ConfigurationManager.AppSettings["FileService_Address"];
            DataServerAddr = ConfigurationManager.AppSettings["DataService_Address"];
        }


    }
}
