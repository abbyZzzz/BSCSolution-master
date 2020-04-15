using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity.Token
{
    /// <summary>
    /// 用户权限服务配置
    /// </summary>
    public class AuthServerConfig
    {
        /// <summary>
        /// 登录地址连接
        /// </summary>
        public string LoginUrl { set; get; }
        /// <summary>
        /// 授权服务地址
        /// </summary>
        public string TokenServerUrl { set; get; }
    }
}
