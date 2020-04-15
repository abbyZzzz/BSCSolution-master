using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    public class ZTree
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 父节点id
        /// </summary>
        public string pId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否处于开启状态
        /// </summary>
        public bool open { get; set; } = false;
    }
}
