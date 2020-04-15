using Advantech.UtilsStandard.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Advantech.CoreExtention.Http
{
    public interface IHttpClientFactoryHelper
    {
        Task<T> GetJsonResult<T>(string url, string postData, HttpMethod method, string authorization = null);
        Task<string> GetStringResult(string url, string postData, HttpMethod method, string authorization = null);
    }
    /// <summary>
    /// 通过工厂模式去管理请求对象
    /// </summary>
    public class HttpClientFactoryHelper: IHttpClientFactoryHelper
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogWrite _log;//日志写入对象

        public HttpClientFactoryHelper(IHttpClientFactory httpClientFactory, ILogWrite logWrite)
        {
            _clientFactory = httpClientFactory;
            _log = logWrite;
        }
        //public HttpClientFactoryHelper(IHttpClientFactory httpClientFactory)
        //{
        //    _clientFactory = httpClientFactory;
        //}
        /// <summary>
        /// 获取对象数据。请注意，此方法请求，对方必须直接回json对象，否则会请求失败
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="url">请求的url</param>
        /// <param name="postData">消息内容</param>
        /// <param name="method">请求方法</param>
        /// <param name="authorization">授权</param>
        /// <returns>返回的对象</returns>
        public async Task<T> GetJsonResult<T>(string url, string postData, HttpMethod method, string authorization = null)
        {
            T resp= default(T);
            try
            {
                if(_log !=null)
                {
                    _log.WriteLog($"GetJsonResult请求，url={url}");
                }

                var request = new HttpRequestMessage(method, url);

                //判断是否需要认证
                if (!string.IsNullOrEmpty(authorization))
                {
                    request.Headers.Add("Authorization", authorization);
                }
               
                var client = _clientFactory.CreateClient();

                HttpResponseMessage response;
               
                if(!string.IsNullOrEmpty(postData))
                {
                    HttpContent httpContent = new StringContent(postData, Encoding.UTF8);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request.Content = httpContent;
                }
                response = await client.SendAsync(request);
                if (response != null && response.IsSuccessStatusCode)
                {
                    resp = await response.Content.ReadAsAsync<T>();
                    return resp;
                }
                else
                {
                    _log.WriteLog("HttpClientFactoryHelper", "GetJsonResult", $"GetJsonResult失败，url={url}，error={response.ReasonPhrase}");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.WriteLog($"GetJsonResult请求错误，url={url}", ex);
                }
            }
            finally
            {
                //LogHelper.Info($"MyHttpClientHelper结束，url={client.BaseAddress},action={Utils.GetEnumDescription(action)},postData={paramStr} ,jrclientguid={jrclientguid}---------");
            }
            return resp;
        }

        /// <summary>
        /// 获取文本数据。对方回复内容为text/plain形式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="method"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public async Task<string> GetStringResult(string url, string postData, HttpMethod method, string authorization = null)
        {
            try
            {
                if (_log != null)
                {
                    _log.WriteLog($"GetStringResult请求，url={url}");
                }
                var request = new HttpRequestMessage(method, url);

                //判断是否需要认证
                if (!string.IsNullOrEmpty(authorization))
                {
                    request.Headers.Add("Authorization", authorization);
                }

                var client = _clientFactory.CreateClient();

                HttpResponseMessage response;

                if (!string.IsNullOrEmpty(postData))
                {
                    HttpContent httpContent = new StringContent(postData, Encoding.UTF8);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request.Content = httpContent;
                }
                response = await client.SendAsync(request);
                if (response != null && response.IsSuccessStatusCode)
                {
                    string resp = await response.Content.ReadAsStringAsync();
                    return resp;
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.WriteLog($"GetStringResult请求错误，url={url}", ex);
                }
            }
            finally
            {
                //LogHelper.Info($"MyHttpClientHelper结束，url={client.BaseAddress},action={Utils.GetEnumDescription(action)},postData={paramStr} ,jrclientguid={jrclientguid}---------");
            }
            return string.Empty;
        }
    }
}
