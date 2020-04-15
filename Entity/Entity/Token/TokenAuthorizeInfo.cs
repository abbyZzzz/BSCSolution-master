using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity.Token
{
    /// <summary>
    /// token解析结果
    /// </summary>
    public class TokenAuthorizeInfo
    {
        /// <summary>
        /// 状态。=true为有效
        /// </summary>
        public bool Status { set; get; } = false;
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { set; get; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public DateTime ValidTime { set; get; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 授权类型
        /// </summary>
        public string AuthorizeType { set; get; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { set; get; }
    }
}
