using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advantech.AuthenticationService.Service
{
    public interface IIdentityService
    {
        int GetUserId();

        string GetUserName();
    }

    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context;
        }
        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <returns></returns>
        public int GetUserId()
        {
            var nameId = _context.HttpContext.User.FindFirst("id");

            return nameId != null ? Convert.ToInt32(nameId.Value) : 0;
        }
        /// <summary>
        /// 获取用户名称
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            return _context.HttpContext.User.FindFirst("name")?.Value;
        }
    }
}
