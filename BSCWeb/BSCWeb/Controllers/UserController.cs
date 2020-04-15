using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BSC.Models;
using SqlSugar;
using Advantech.CoreExtention;
using Advantech.Service;
using Advantech.Entity;
using Advantech.BSCWeb;
using Advantech.CoreExtention.Attribute;
using Advantech.Entity.Token;
using Advantech.UtilsStandard.Interface;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Advantech.CoreExtention.Http;

namespace BSC.Controllers
{
    /// <summary>
    /// 用于权限测试使用的
    /// </summary>
    public class UserController : Controller
    {
        private readonly IUserGroupService _userGroupService;
        private readonly IHttpRequest _httpRequest;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClientFactoryHelper _httpClient;

        public UserController(IUserGroupService userGroupService,
                              IHttpRequest httpRequest,
                              IHttpContextAccessor httpContextAccessor,
                              HttpClientFactoryHelper httpClientFactory)
        {
            _userGroupService = userGroupService;
            _httpRequest = httpRequest;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory;
        }

        [UserAuthorize(true)]//权限检查,失败则跳转到登录
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ServerPerformance()
        {
            return View();
        }

        public async Task<IActionResult> LoginAsync(string name, string password)
        {
            string url = "http://172.21.168.5:8010/Authentication/Token/GetToken?name=admin&password=admin";
            TokenAuthorizeInfo tokenAuthorizeInfo = await _httpClient.GetJsonResult<TokenAuthorizeInfo>(url, null, HttpMethod.Get);
            if (tokenAuthorizeInfo != null)
            {
                var _session = _httpContextAccessor.HttpContext.Session;
                _session.SetString("user_name", tokenAuthorizeInfo.UserName);
                _session.SetString("token", tokenAuthorizeInfo.Token);
                return Content("OK");
            }

            return Content("Fail");
        }

    }
}
