using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantech.PlayerComponent
{
    /// <summary>
    /// 错误消息
    /// </summary>
    public class ErrMsgArgs : EventArgs
    {
        public string Message { set; get; }
        public ErrMsgArgs(string content)
        {
            this.Message = content;
        }
    }
}
