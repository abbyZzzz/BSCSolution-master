using Advantech.BSCPlayer.Common;
using Advantech.Entity;
using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Advantech.AppCommon
{
    /// <summary>
    /// 媒体文件任务，主要负责媒体文件的下载
    /// </summary>
    public class MediaFileTask:IDisposable
    {
        /// <summary>
        /// 需要下载的队列
        /// </summary>
        public Queue<string> FilesQueue = new Queue<string>();
        private string LocalPath = ConfigurationManager.AppSettings["Media_Path"];//本地配置路径
        private Thread downLoadThread;

        private readonly ILogWrite _log;
        private readonly IHttpFile _httpFile;
        private readonly IHttpRequest _httpRequest;
        
        public MediaFileTask(IHttpRequest httpHelper, IHttpFile httpFile,ILogWrite logWrite)
        {
            this._httpRequest = httpHelper;
            this._log = logWrite;
            this._httpFile = httpFile;

            FileInfo file = new FileInfo(LocalPath);
            if(!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            downLoadThread = new Thread(DownLoadFileAsync);
            downLoadThread.Start();
        }
        /// <summary>
        /// 查询服务器配置，获取节目列表信息
        /// </summary>
        public void GetMediaFileInfo(int media_id)
        {
            string body = string.Empty;
            string url = ServerCfg.FileServerAddr + $"/file/MediaInfo?id={media_id}";
            string res= _httpRequest.HttpGet(url);
            List<MediaInfo> _mediaInfo = JsonConvert.DeserializeObject<List<MediaInfo>>(res);
            if (_mediaInfo != null)
            {
                if(FileCheck(_mediaInfo[0]))
                {
                    FilesQueue.Enqueue(_mediaInfo[0].media_address);//将信息中的地址加入下载队列
                }
                else
                {
                    _log.WriteLog($"文件{_mediaInfo[0].media_address}大小无变化，跳过下载");
                }
            }
            else
            {
                _log.WriteLog($"请求节目信息为空{media_id}");
            }
        }
        /// <summary>
        /// 文件有变化返回true
        /// </summary>
        /// <param name="mediaInfo"></param>
        /// <returns></returns>
        private bool FileCheck(MediaInfo mediaInfo)
        {
            string fileLocalPath = LocalPath + Path.GetFileName(mediaInfo.media_address);
            FileInfo fileInfo = new FileInfo(fileLocalPath);
            if(fileInfo.Exists)
            {
                int kbSize = (int)fileInfo.Length / 1024;
                if (mediaInfo.media_size != kbSize)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 文件下载周期任务
        /// </summary>
        /// <returns></returns>
        public void DownLoadFileAsync()
        {
            while (true)
            {
                if (FilesQueue != null && FilesQueue.Count > 0)
                {
                    string fileUrl = FilesQueue.Dequeue();
                    string fileLocalPath = LocalPath + Path.GetFileName(fileUrl);
                    bool ret = _httpFile.HttpDownloadFileAsync(fileUrl, fileLocalPath);
                    if (!ret)//失败
                    {
                        FilesQueue.Enqueue(fileUrl);//重新加入下载任务
                    }
                    else
                    {
                        _log.WriteLog("已成功下载文件" + fileUrl);
                    }
                }
                Console.WriteLine(this.GetHashCode());
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 结束任务释放资源
        /// </summary>
        public void Dispose()
        {
            if(downLoadThread!=null)
            {
                try
                {
                    downLoadThread.Join();
                    downLoadThread = null;
                }
                catch(Exception ex){}
            }
        }
    }
}
