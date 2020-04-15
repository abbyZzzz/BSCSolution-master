using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Advantech.Entity.Token;
using Microsoft.AspNetCore.Http;

namespace Advantech.CoreExtention.Middleware
{
    public class SecurityHeadersMiddleware
    {
        /// <summary>
        /// token当前暂存
        /// </summary>
        public static TokenAuthorizeInfo tokenAuthorizeInfo = new TokenAuthorizeInfo();

        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //TokenAuthorizeInfo tokenAuthorizeInfo = context.RequestServices.GetService(typeof(TokenAuthorizeInfo)) as TokenAuthorizeInfo;
            string token = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token=context.Request.Cookies["Authorization"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = token;//加入请求头
                }
            }
            await _next(context);
        }
    }
}
