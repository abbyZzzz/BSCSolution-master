using Advantech.Entity;
using Advantech.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public interface IProgramInfoService : IBaseRepository<ProgramInfo>
    {
        /// <summary>
        /// 通过id获取一个节目完整的信息列表，包括下面的区块，区块里面的节目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProgramInfo GetCompositeById(int id);
    }
}
