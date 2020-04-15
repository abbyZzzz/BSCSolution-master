using Advantech.CoreExtention;
using Advantech.UtilsStandardLib.System;
using Advantech.Entity;
using Advantech.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using Advantech.AuthenticationService.Service;
using Advantech.Entity.Token;
using Microsoft.AspNetCore.Http;
using Advantech.Entity.UserAndGroup;

namespace Advantech.AuthenticationService.Controllers
{
    [Route("Authentication/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public TokenController(IUserService userService,
                               ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("GetToken")]
        public IActionResult GetToken(string name, string password)
        {
            UserInfo user;
            TokenAuthorizeInfo tokenAuthorizeInfo = new TokenAuthorizeInfo();
            if (name!="admin" && password!="admin")//此代码临时处理，因为用户权限功能暂时无
            {
                user = _userService.QueryableToEntity(x => x.user_name.Equals(name));
                if (user == null)
                {
                    tokenAuthorizeInfo.ErrorMessage = "未查找到用户";
                }
                else if (!user.user_pwd.DecodeBase64().Equals(password))
                {
                    tokenAuthorizeInfo.ErrorMessage = "用户密码错误";
                }
            }
            else
            {
                user = new UserInfo() { id = 1, user_name = "admin" };
            }

            if(user !=null && user.id>0)
            {
                tokenAuthorizeInfo.AuthorizeType = "Bearer";
                tokenAuthorizeInfo.UserId = user.id;
                tokenAuthorizeInfo.UserName = user.user_name;
                tokenAuthorizeInfo.Token = _tokenService.GetToken(user);
                tokenAuthorizeInfo.ValidTime = DateTime.Now.AddMinutes(_tokenService.TokenSetting.ExpireMins);
            }
            return new JsonResult(tokenAuthorizeInfo);
           // return JsonConvert.SerializeObject(tokenAuthorizeInfo);
        }

        [HttpGet("ValidParToken")]
        public IActionResult ValidateToken(string AuthToken)//验证token结果
        {
            TokenAuthorizeInfo tokenParseResult = _tokenService.ValidateToken(AuthToken);
            return new JsonResult(tokenParseResult);
        }

        [HttpGet("ValidhHeaderToken")]
        public IActionResult ValidateToken()//验证token结果
        {
            TokenAuthorizeInfo tokenParseResult = new TokenAuthorizeInfo();
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];
                tokenParseResult = _tokenService.ValidateToken(token);
            }
            
            return new JsonResult(tokenParseResult);
        }
    }
    
}
