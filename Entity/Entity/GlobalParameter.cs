using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    public class GlobalParameter
    {
        /// <summary>
        /// 媒体存放路径
        /// </summary>
        public static string _MEDIA{ get; set; }
        /// <summary>
        /// 预览图存放路径
        /// </summary>
        public static string _PREVIEW { get; set; }
        /// <summary>
        /// Icon存放路径
        /// </summary>
        public static string _ICON { get; set; }
        /// <summary>
        /// 临时目录路径
        /// </summary>
        public static string _TEMPROARY { get; set; }
        /// <summary>
        /// 节目图片目录
        /// </summary>
        public static string _PROGRAM { get; set; }
        /// <summary>
        /// 节目图片预览图
        /// </summary>
        public static string _PROGRAMPREVIEW { get; set; }
        /// <summary>
        /// 第三方程序_ffmpeg.exe地址(用来执行视频类文件的参数获取与图片生成)
        /// </summary>
        public static string _FFMPEG { get; set; }
        /// <summary>
        /// API的URL(例如https://127.0.0.1)
        /// </summary>
        public static string _APIURL { get; set; }
        /// <summary>
        /// 指定文件目录
        /// </summary>
        public static string _FOLDER { get; set; }
        /// <summary>
        /// 代理服务器地址
        /// </summary>
        public static string _AGENTURL { get; set; }
        /// <summary>
        /// 树状图用户群组ID的前缀（防止因为重复ID造成树状图出错）
        /// </summary>
        public static string _PREFIX { get; set; }
        /// <summary>
        /// 格式化数值
        /// </summary>
        public static string _FORMATVALUE { get; set; } = "0000000000";

    }
}
