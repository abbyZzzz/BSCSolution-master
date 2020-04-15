using Advantech.Entity;
using Advantech.Entity.SysLog;
using Advantech.Repository;
using Advantech.UtilsStandard.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public interface ISystemLogService : IBaseRepository<SystemLog>, ILogWrite
    {
    }
}
