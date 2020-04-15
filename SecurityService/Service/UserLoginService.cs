using Advantech.Entity.Token;
using Advantech.Entity.UserAndGroup;
using Advantech.Service;
using System;
using Advantech.UtilsStandardLib.System;

namespace Advantech.SecurityService.Service
{
    public interface IUserLoginService
    {
        TokenAuthorizeInfo LoginAndGetToken(LoginUserMode userModel);
    }
    public class UserLoginService : IUserLoginService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserLoginService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 登录并获取token
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public TokenAuthorizeInfo LoginAndGetToken(LoginUserMode userModel)
        {
            UserInfo userInfo;
            TokenAuthorizeInfo tokenAuthorizeInfo = new TokenAuthorizeInfo();

            userInfo = _userService.QueryableToEntity(x => x.user_name.Equals(userModel.user_name));
            if (userInfo == null)
            {
                tokenAuthorizeInfo.ErrorMessage = "未查找到用户";
            }
            else if (!userInfo.user_pwd.DecodeBase64().Equals(userModel.user_pwd))
            {
                tokenAuthorizeInfo.ErrorMessage = "用户密码错误";
            }
            else
            {
                tokenAuthorizeInfo.Status = true;
                tokenAuthorizeInfo.AuthorizeType = "Bearer";
                tokenAuthorizeInfo.UserId = userInfo.id;
                tokenAuthorizeInfo.UserName = userInfo.user_name;
                tokenAuthorizeInfo.Token = _tokenService.GetToken(userInfo);
                tokenAuthorizeInfo.ValidTime = DateTime.Now.AddMinutes(_tokenService.TokenSetting.ExpireMins);

                userInfo.last_login = DateTime.Now;
                _userService.Update(userInfo);//更新最后一次登录时间
            }

            return tokenAuthorizeInfo;
        }
    }
}
