using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.UtilsStandard.Interface
{
    /// <summary>
    /// http文件操作接口
    /// </summary>
    public interface IHttpFile
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        string HttpUploadFileAsync(string url, string filePath, string postData);
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool HttpDownloadFileAsync(string url, string path);
    }
}
