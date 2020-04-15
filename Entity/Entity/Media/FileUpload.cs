using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    public class FileUpload
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string file_name { get; set; }
        /// <summary>
        /// 文件夹id
        /// </summary>
        public int media_group_id { get; set; }
        /// <summary>
        /// 原文件名称(Update用到)
        /// </summary>
        public string before_name { get; set; }
        /// <summary>
        /// 操作类型(add,update,delete)
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 文件所在路径
        /// </summary>
        public string file_path { get; set; }
    }
}
