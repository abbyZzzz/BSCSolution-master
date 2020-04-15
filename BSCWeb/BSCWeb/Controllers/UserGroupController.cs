using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Advantech.Entity;
using Advantech.Service;
using Microsoft.AspNetCore.Mvc;

namespace BSCWeb.Controllers
{
    public class UserGroupController : Controller
    {
        private readonly IUserGroupService _mediaInfoService;
        private readonly IHttpClientFactory _clientFactory;
        public UserGroupController(IUserGroupService mediaInfoService)
        {
            _mediaInfoService = mediaInfoService;
        }
        public IActionResult Index()
        {
            return View();
        }


        

        
    }
}