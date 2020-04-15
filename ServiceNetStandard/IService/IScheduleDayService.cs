using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public interface IScheduleDayService : IBaseRepository<ScheduleDay>
    {
        ScheduleDay GetScheduleDayById(int id);
        string GetAll();

        string GetById(int id);
        string GetAllDetails();
        string GetDetailsById(int id);
    }
}
