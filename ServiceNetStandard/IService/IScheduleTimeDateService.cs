using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public interface IScheduleTimeDateService : IBaseRepository<ScheduleTimeDate>
    {
        string GetAll();
    }
}
