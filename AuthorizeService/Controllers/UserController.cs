using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using Advantech.AuthenticationService.Service;
using Advantech.Service;
using Microsoft.AspNetCore.Authorization;

namespace Advantech.AuthenticationService.Controllers
{
    [Route("Authentication/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
            this._userService.InitTables();
        }
        [HttpGet]
        [Authorize]
        public string Get(int id)
        {
            var user= _userService.QueryableToEntity(x => x.id == id);
            return JsonConvert.SerializeObject(user); 
        }
        
    }
}
