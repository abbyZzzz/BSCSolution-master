using Advantech.UtilsStandardLib.System;
using Advantech.Entity;
using Advantech.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advantech.Entity.Token;
using static Advantech.AuthenticationService.Rpc.ThriftRpcTokenService;
using System.Threading;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Advantech.AuthenticationService.Model;
using Microsoft.Extensions.Options;
using Advantech.AuthenticationService.Service;
using Advantech.Entity.UserAndGroup;

namespace Advantech.AuthenticationService.Rpc
{
    /// <summary>
    /// RPC访问方式，代码待进一步优化
    /// </summary>
    public class RpcTokenService: IAsync
    {
        private readonly JwtSetting _jwtSetting;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public RpcTokenService(IUserService userService,
                               ITokenService tokenService,
                               IOptions<JwtSetting> option)
        {
            _userService = userService;
            _tokenService = tokenService;
            _jwtSetting = option.Value;
        }

        public async Task<string> GetTokenAsync(string UserJsonString, CancellationToken cancellationToken)
        {
            TokenAuthorizeInfo tokenAuthorizeInfo = new TokenAuthorizeInfo();
            UserInfo user = JsonConvert.DeserializeObject<UserInfo>(UserJsonString);

            if (user.user_name != "admin" && user.user_pwd != "admin")//此代码临时处理，因为用户权限功能暂时无
            {
                user = _userService.QueryableToEntity(x => x.user_name.Equals(user.user_name));
                if (user == null)
                {
                    tokenAuthorizeInfo.ErrorMessage = "未查找到用户";
                }
                else if (!user.user_pwd.DecodeBase64().Equals(user.user_pwd))
                {
                    tokenAuthorizeInfo.ErrorMessage = "用户密码错误";
                }
            }
            else
            {
                user = new UserInfo() { id = 1, user_name = "admin" };
            }

            if (user != null && user.id > 0)
            {
                tokenAuthorizeInfo.AuthorizeType = "Bearer";
                tokenAuthorizeInfo.UserId = user.id;
                tokenAuthorizeInfo.UserName = user.user_name;
                tokenAuthorizeInfo.Token = _tokenService.GetToken(user);
                tokenAuthorizeInfo.ValidTime = DateTime.Now.AddMinutes(_tokenService.TokenSetting.ExpireMins);
            }

            string ResponseJson = JsonConvert.SerializeObject(tokenAuthorizeInfo);
            return ResponseJson;
        }

        public async Task<string> ValidateTokenAsync(string AuthToken, CancellationToken cancellationToken)
        {
            DateTime ValidTime;
            TokenAuthorizeInfo tokenInfo = new TokenAuthorizeInfo();

            if (string.IsNullOrEmpty(AuthToken) == false)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();
                SecurityToken validatedToken;
                try
                {
                    ClaimsPrincipal principal = tokenHandler.ValidateToken(AuthToken, validationParameters, out validatedToken);
                    JwtSecurityToken JwtVlidatedToken = validatedToken as JwtSecurityToken;
                    ValidTime = validatedToken.ValidTo.AddHours(8);//时区为零时区，需要加8小时

                    Claim claim = JwtVlidatedToken.Claims.First(x => x.Type.Equals("name"));
                    if (claim != null)
                    {
                        tokenInfo.UserName = claim.Value;
                        tokenInfo.ValidTime = ValidTime;
                        tokenInfo.Status = true;
                    }
                    else
                    {
                        tokenInfo.ErrorMessage = "错误，Token不含用户名称";
                    }
                }
                catch (Exception ex)//token过期会自动报错
                {
                    tokenInfo.ErrorMessage = "错误,Token解析失败" + ex.Message;
                }
            }
            return JsonConvert.SerializeObject(tokenInfo);
        }

        /// <summary>
        /// 获取JWT参数
        /// </summary>
        /// <returns></returns>
        public TokenValidationParameters GetValidationParameters()
        {
            Byte[] SecurityKeyByte = Encoding.UTF8.GetBytes(_jwtSetting.SecurityKey);
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(SecurityKeyByte);
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = _jwtSetting.Issuer,
                ValidAudience = _jwtSetting.Audience,
                IssuerSigningKey = symmetricSecurityKey
            };
            return tokenValidationParameters;
        }
    }
}
