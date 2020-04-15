using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.CommonDefine
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileExtension : int
    {
        //图片
        JPG = 255216,
        GIF = 7173,
        BMP = 6677,
        PNG = 13780,
        //视频
        VIDEO = 0,//(MP4,MOV,AVI,FLV)
        //AVI = 0,
        //MOV = 0,
        WMV = 4838,
        //FLV = 0,

        //PPT
        PPT = 208207,
        PPTX = 8075,
        //PDF
        PDF = 3780,
        //音频
        MP3 = 7368,
        //网页
        HTML = 239187//(HTML,HTM)
    }
}
