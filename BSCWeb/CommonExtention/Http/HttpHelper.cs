using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Advantech.CoreExtention.Http
{
    public class HttpHelper
    {
        /// <summary>
        ///  http Post
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postData">抛送参数</param>
        /// <param name="contentType">默认json</param>
        /// <param name="authorization">token</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData, string contentType= "application/json;charset=UTF-8", string authorization=null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "POST";
                _reques.Timeout = 5000;
                //判断是否需要认证
                if(!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                //设定ContentType
                _reques.ContentType = contentType;
                byte[] data = Encoding.UTF8.GetBytes(postData);
                _reques.ContentLength = data.Length;
                using (Stream reqStream = _reques.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);

                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }

        public static string HttpPut(string url, string postData, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "PUT";
                _reques.Timeout = 5000;
                //判断是否需要认证
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                //设定ContentType
                _reques.ContentType = contentType;
                byte[] data = Encoding.UTF8.GetBytes(postData);
                _reques.ContentLength = data.Length;
                using (Stream reqStream = _reques.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);

                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }
        /// <summary>
        /// Http Get
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="contentType">contentType</param>
        /// <param name="authorization">token</param>
        /// <returns></returns>
        public static string HttpGet(string url, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "GET";
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                _reques.ContentType = contentType;
                _reques.Timeout = 5000;
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream myResponseStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = reader.ReadToEnd();
                reader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string HttpDel(string url, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "DELETE";
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                _reques.ContentType = contentType;
                _reques.Timeout = 5000;
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream myResponseStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = reader.ReadToEnd();
                reader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 验证远程文件是否存在
        /// </summary>
        /// <param name="fileUrl">文件地址</param>
        /// <returns></returns>
        public static bool RemoteFileExists(string fileUrl)
        {
            bool result = false;//下载结果
            WebResponse response = null;
            try
            {
                WebRequest req = WebRequest.Create(fileUrl);
                req.Timeout = 1000;
                response = req.GetResponse();
                result = response == null ? false : true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }
    }
}
