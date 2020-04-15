using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public interface IProgramScheduleService : IBaseRepository<ProgramSchedule>
    {
        /// <summary>
        /// 获取复合对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProgramSchedule GetCompositeById(int id);

        string GetAll();

        string GetById(int id);
        string GetAllDetails();
        string GetDetailsById(int id);
        string GetAllUnionQuery();
        /// <summary>
        /// 清除所有的排程信息，player播放端使用
        /// </summary>
        /// <returns></returns>
        bool ClearAll();


    }
}
