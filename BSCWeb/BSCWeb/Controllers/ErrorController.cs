using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BSC.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// {0}中是错误码
        /// </summary>
        /// <returns></returns>
        [Route("/error/{0}")]
        public IActionResult Page()
        {
            //跳转到404错误页
            if (Response.StatusCode == 404)
            {
                return View("/Views/Shared/_Notfound.cshtml");
            }
            return View();
        }
    }
}