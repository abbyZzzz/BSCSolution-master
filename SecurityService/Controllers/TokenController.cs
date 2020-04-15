using Advantech.Service;
using Microsoft.AspNetCore.Mvc;
using Advantech.SecurityService.Service;
using Advantech.Entity.Token;
using Microsoft.AspNetCore.Http;
using Advantech.UtilsStandard.Interface;

namespace Advantech.SecurityService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILogWrite _logWrite;
        private readonly ITokenService _tokenService;

        public TokenController(ILogWrite logWrite,
                               ITokenService tokenService)
        {
            _logWrite = logWrite;
            _tokenService = tokenService;
        }
        [HttpGet]
        public IActionResult ValidateToken([FromBody]string AuthToken)//验证token结果
        {
            string token = AuthToken;
            if (token.Contains("Bearer"))
            {
                token = token.Replace("Bearer", string.Empty);
                token = token.Trim();
            }
            _logWrite.WriteLog("Token","ValidateToken","请求验证token");
            TokenAuthorizeInfo tokenParseResult = _tokenService.ValidateToken(token);
            return new JsonResult(tokenParseResult);
        }

        [HttpGet]
        public IActionResult ValidhHeaderToken()//验证token结果
        {
            TokenAuthorizeInfo tokenParseResult = new TokenAuthorizeInfo();
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];
                if (token.Contains("Bearer"))
                {
                    token = token.Replace("Bearer", string.Empty);
                    token = token.Trim();
                }
                _logWrite.WriteLog("Token", "ValidhHeaderToken", "请求验证token");
                tokenParseResult = _tokenService.ValidateToken(token);
            }

            return new JsonResult(tokenParseResult);
        }
    }

}
