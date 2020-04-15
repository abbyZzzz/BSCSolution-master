using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Advantech.Entity.Token;
using System.Threading;
using Advantech.SecurityService.Service;
using Advantech.Entity.UserAndGroup;
using static Advantech.SecurityService.Rpc.ThriftRpcTokenService;

namespace Advantech.SecurityService.Rpc
{
    /// <summary>
    /// RPC访问方式
    /// </summary>
    public class RpcTokenService : IAsync
    {
        private readonly ITokenService _tokenService;
        private readonly IUserLoginService _userLoginService;

        public RpcTokenService(IUserLoginService userLoginService,
                               ITokenService tokenService)
        {
            _userLoginService = userLoginService;
            _tokenService = tokenService;
        }

        public async Task<string> GetTokenAsync(string UserJsonString, CancellationToken cancellationToken)
        {
            LoginUserMode user = JsonConvert.DeserializeObject<LoginUserMode>(UserJsonString);
            TokenAuthorizeInfo tokenAuthorizeInfo = _userLoginService.LoginAndGetToken(user);
            string ResponseJson = JsonConvert.SerializeObject(tokenAuthorizeInfo);
            return ResponseJson;
        }

        public async Task<string> ValidateTokenAsync(string AuthToken, CancellationToken cancellationToken)
        {
            TokenAuthorizeInfo tokenInfo = _tokenService.ValidateToken(AuthToken);
            return JsonConvert.SerializeObject(tokenInfo);
        }

    }
}
