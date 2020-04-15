using Advantech.UtilsStandard.Interface;
using Advantech.UtilsStandardLib;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Advantech.UtilsStandardLib.Http
{
    /// <summary>
    /// 文件上传下载扩展
    /// </summary>
    public class HttpFileHelper: IHttpFile
    {
        private readonly ILogWrite _log;

        public HttpFileHelper(ILogWrite logWrite)
        {
            this._log = logWrite;
        }
        ///  <summary>
        /// 上传文件
        ///  </summary>
        ///  <param name="url">api地址</param>
        ///  <param name="filePath">上传文件路径</param>
        ///  <param name="postData">其他api要求数据(例如文件格式.jpg.rar.exe....)</param>
        ///  <returns></returns>
        public string HttpUploadFileAsync(string url, string filePath, string postData)
        {
            string result = string.Empty;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (var multipartFormDataContent = new MultipartFormDataContent())
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                        fileContent.Headers.Add("Content-Type", "multipart/form-data");
                        multipartFormDataContent.Add(fileContent, "file", fileName);
                        var body = new StringContent(postData, Encoding.UTF8, "application/json");
                        multipartFormDataContent.Add(body, "fileType");
                        var response = httpClient.PostAsync(url, multipartFormDataContent).Result;
                        //确保Http响应成功
                        if (response.IsSuccessStatusCode)
                        {
                            //异步读取json
                            result = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
               // Logger.Instance.Log($"http post error, url={url},exception:{e}", Category.Error);
            }

            return result;
        }
        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">文件地址</param>
        /// <param name="path">下载后的本地目录</param>
        /// <returns></returns>
        public bool HttpDownloadFileAsync(string url, string path)
        {
            bool res = true;
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();

                FileInfo fileInfo = new FileInfo(path);
                if(fileInfo.Exists)//文件存在则删除
                {
                    fileInfo.Delete();
                }
                if(!fileInfo.Directory.Exists)//文件目录不存在创建
                {
                    fileInfo.Directory.Create();
                }
                //创建本地文件写入流
                Stream stream = new FileStream(path, FileMode.Create);

                byte[] byteArr = new byte[1024];
                int size = responseStream.Read(byteArr, 0, (int)byteArr.Length);
                while (size > 0)
                {
                    stream.Write(byteArr, 0, size);
                    size = responseStream.Read(byteArr, 0, (int)byteArr.Length);
                }
                stream.Close();
                responseStream.Close();
                _log.WriteLog($"文件{path}下载成功");
            }
            catch(Exception ex)
            {
                _log.WriteLog("文件下载失败",ex);
                res = false;
            }
            return res;
        }
    }
    
}
