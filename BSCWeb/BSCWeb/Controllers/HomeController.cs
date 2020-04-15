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

namespace BSC.Controllers
{
    public class HomeController : Controller
    {
        //[ServiceFilter(typeof(UserAuthorizeAttribute))]
        //[UserAuthorize(true)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ServerPerformance()
        {
            return View();
        }
        
    }
}
