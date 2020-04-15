using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.UtilsStandard.Interface
{
    /// <summary>
    /// Http操作接口
    /// </summary>
    public interface IHttpRequest
    {
        string HttpPost(string url, string postData, string contentType = "application/json;charset=UTF-8", string authorization = null);
        string HttpPut(string url, string postData, string contentType = "application/json;charset=UTF-8", string authorization = null);
        string HttpGet(string url, string contentType = "application/json;charset=UTF-8", string authorization = null);
        string HttpDel(string url, string contentType = "application/json;charset=UTF-8", string authorization = null);
    }
}
