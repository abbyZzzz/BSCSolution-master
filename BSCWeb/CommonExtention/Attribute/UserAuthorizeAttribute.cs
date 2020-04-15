using Advantech.CoreExtention.Http;
using Advantech.Entity.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Advantech.CoreExtention.Attribute
{
    /// <summary>
    /// 用户权限过滤器
    /// </summary>
    public class UserAuthorize : TypeFilterAttribute
    {
        /// <summary>
        /// 是否跳转至登录页面
        /// </summary>
        /// <param name="IsRedirectToLogin"></param>
        public UserAuthorize(bool IsRedirectToLogin=false) : base(typeof(UserAuthorizeFilter))
        {
            this.Arguments = new object[] { IsRedirectToLogin };
        }

        private class UserAuthorizeFilter : ActionFilterAttribute
        {
            private readonly IHttpClientFactoryHelper _httpClient; //http请求
            private readonly AuthServerConfig _authServerConfig;//配置
            /// <summary>
            /// 是否跳转到登录地址
            /// </summary>
            private readonly bool _IsRedirectToLogin;

            public UserAuthorizeFilter(bool IsRedirectToLogin, AuthServerConfig filterConfig, IHttpClientFactoryHelper httpRequest)
            {
                _IsRedirectToLogin = IsRedirectToLogin;
                _httpClient = httpRequest;
                _authServerConfig = filterConfig;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                string token = string.Empty;
                token = context.HttpContext.Request.Headers["Authorization"];//先从请求头获取token
                if (string.IsNullOrEmpty(token))
                {
                    token = context.HttpContext.Session.GetString("token");//再从session中获取token
                }
                
                var objectResult = context.Result as ObjectResult;
                string url = $"{_authServerConfig.TokenServerUrl}/security/Token/ValidateToken";
                string posData = JsonConvert.SerializeObject(token);
                TokenAuthorizeInfo tokenAuthorizeInfo = await _httpClient.GetJsonResult<TokenAuthorizeInfo>(url, posData, HttpMethod.Get);

                if (tokenAuthorizeInfo != null)
                {
                    if (tokenAuthorizeInfo.Status)
                    {
                        //continue;
                        //context.Result = objectResult;
                        await next.Invoke();
                    }
                    else
                    {
                        if (_IsRedirectToLogin)
                        {
                            context.HttpContext.Response.StatusCode = 401;
                            context.Result = new RedirectResult(_authServerConfig.LoginUrl);//跳转到登录页面
                        }
                        else
                        {
                            objectResult = new ObjectResult(new { code = 401, msg = "用户未有授权" });
                            context.Result = objectResult;
                            context.HttpContext.Response.StatusCode = 401;
                        }
                    }
                }
                else
                {
                    if (_IsRedirectToLogin)
                    {
                        context.HttpContext.Response.StatusCode = 401;
                        context.Result = new RedirectResult(_authServerConfig.LoginUrl);//跳转到登录页面
                    }
                    else
                    {
                        objectResult = new ObjectResult(new { code = 401, msg = "用户未有授权" });
                        context.Result = objectResult;
                        context.HttpContext.Response.StatusCode = 401;
                    }
                }
            }
        }
    }


}
