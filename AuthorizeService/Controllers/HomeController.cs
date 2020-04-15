using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using Advantech.AuthenticationService.Service;

namespace Advantech.AuthenticationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        public HomeController()
        {
           
        }

        public IActionResult Get()
        {
            return Content("OK");
        }
    }
}
