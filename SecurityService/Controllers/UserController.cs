using Microsoft.AspNetCore.Mvc;
using System;
using Advantech.SecurityService.Service;
using Advantech.Service;
using Microsoft.AspNetCore.Authorization;
using Advantech.Entity.UserAndGroup;
using Advantech.Entity.Token;
using Advantech.UtilsStandardLib.System;

namespace Advantech.SecurityService.Controllers
{
    [Route("[Controller]/[Action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserLoginService _userLoginService;

        public UserController(IUserService userService, IUserLoginService userLoginService)
        {
            _userService = userService;
            _userLoginService = userLoginService;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            var user = _userService.QueryableToEntity(x => x.id == id);
            return new JsonResult(user);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Add([FromBody] UserInfo user)
        {
            var userInfo = _userService.QueryableToEntity(x => x.user_name == user.user_name);
            if (userInfo == null)
            {
                user.creat_time = DateTime.Now;
                user.user_pwd = user.user_pwd.EncodeBase64();//64位加密
                if (_userService.Insert(userInfo))
                {
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("fail");
                }
            }
            return new JsonResult("用户已经存在");
        }
        [HttpDelete]
        [Authorize]
        public IActionResult Delete([FromBody] int id)
        {
            var userInfo = _userService.QueryableToEntity(x => x.id == id);
            if (userInfo != null)
            {
                if (_userService.Delete(userInfo))
                {
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("fail");
                }
            }
            return new JsonResult("用户不存在");
        }
        [HttpPut]
        [Authorize]
        public IActionResult Update([FromBody] UserInfo user)
        {
            var userInfo = _userService.QueryableToEntity(x => x.id == user.id);
            if (userInfo != null)
            {
                if (_userService.Update(userInfo))
                {
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("fail");
                }
            }
            return new JsonResult("用户不存在");
        }
        /// <summary>
        /// 登录并获取token
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login([FromBody]LoginUserMode userModel)
        {
            TokenAuthorizeInfo tokenAuthorizeInfo = _userLoginService.LoginAndGetToken(userModel);
            return new JsonResult(tokenAuthorizeInfo);
        }
    }
}
