using System;
using System.Net.Http;
using System.Threading.Tasks;
using Advantech.CoreExtention.Http;
using Advantech.Entity.Token;
using Advantech.UtilsStandard.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Advantech.CoreExtention.Middleware
{
    /// <summary>
    /// 用户权限中间件
    /// </summary>
    public class UserAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthServerConfig _authServerConfig;//配置

        public UserAuthorizeMiddleware(RequestDelegate next, AuthServerConfig authServerConfig)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _authServerConfig = authServerConfig;
            //_httpClient = httpRequest;
        }
        //public UserAuthorizeMiddleware(RequestDelegate next, AuthServerConfig authServerConfig, IServiceProvider  serviceProvider)
        //{
        //    if (next == null)
        //    {
        //        throw new ArgumentNullException(nameof(next));
        //    }
        //    _next = next;
        //    _authServerConfig = authServerConfig;
        //    _serviceProvider = serviceProvider;
        //    var _emailRepository = serviceProvider.GetRequiredService<IHttpClientFactoryHelper>();
        //}
        /// <summary>
        /// Scoped的接口只能从Invoke进行方法注入，否则无法获取到对象
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IHttpClientFactoryHelper httpClient)
        {
            string token = string.Empty;
            token = context.Request.Headers["Authorization"];//先从请求头获取token
            if (string.IsNullOrEmpty(token))
            {
                token = context.Session.GetString("token");//再从session中获取token
            }

            string url = $"{_authServerConfig.TokenServerUrl}/Security/Token/ValidateToken";
            string posData = JsonConvert.SerializeObject(token);
            TokenAuthorizeInfo tokenAuthorizeInfo = await httpClient.GetJsonResult<TokenAuthorizeInfo>(url, posData, HttpMethod.Get);

            if (tokenAuthorizeInfo != null)
            {
                if (tokenAuthorizeInfo.Status)
                {
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                }
            }
            else
            {
                context.Response.StatusCode = 401;
            }

        }
    }
}
