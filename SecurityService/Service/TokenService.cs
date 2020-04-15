using Advantech.AuthenticationService.Model;
using Advantech.Entity.UserAndGroup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Advantech.Entity.Token;

namespace Advantech.SecurityService.Service
{
    public interface ITokenService
    {
        JwtSetting TokenSetting { get; }
        string GetToken(UserInfo user);
        TokenValidationParameters GetValidationParameters();
        TokenAuthorizeInfo ValidateToken(string AuthToken);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtSetting _jwtSetting;
        public TokenService(IOptions<JwtSetting> option)
        {
            _jwtSetting = option.Value;
        }
        public JwtSetting TokenSetting
        {
            get { return _jwtSetting; }
        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetToken(UserInfo user)
        {
            //创建用户身份标识，可按需要添加更多信息
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),     //
                new Claim("id", user.id.ToString(), ClaimValueTypes.Integer32),
                new Claim("name", user.user_name)
            };
            try
            {
                //创建令牌
                var token = new JwtSecurityToken(
                        issuer: _jwtSetting.Issuer,
                        audience: _jwtSetting.Audience,
                        signingCredentials: _jwtSetting.Credentials,
                        claims: claims,
                        notBefore: DateTime.Now,
                        expires: DateTime.Now.AddMinutes(_jwtSetting.ExpireMins)
                    );
                string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return jwtToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return string.Empty;
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
        /// <summary>
        /// 解析Token
        /// </summary>
        /// <param name="AuthToken"></param>
        /// <returns></returns>
        public TokenAuthorizeInfo ValidateToken(string AuthToken)
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
            return tokenInfo;
        }
    }


}
