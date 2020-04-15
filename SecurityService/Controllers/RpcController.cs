using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Advantech.SecurityService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class RpcController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RpcController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        /// <summary>
        /// 获取RPC接口名称
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetRpcPort()
        {
            string port = _configuration.GetValue<string>("RpcPort");
            return new JsonResult(port);
        }
    }
}
