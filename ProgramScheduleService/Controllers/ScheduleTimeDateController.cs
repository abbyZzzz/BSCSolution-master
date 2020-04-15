using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Entity.UserAndGroup;
using Advantech.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;

namespace Advantech.ProgramScheduleService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ScheduleTimeDateController : ControllerBase
    {
        private readonly IScheduleTimeDateService _scheduleTimeDateService;
        public ScheduleTimeDateController(IScheduleTimeDateService scheduleTimeDateService) {
            _scheduleTimeDateService = scheduleTimeDateService;
        }

        /// <summary>
        /// 获取原始数据库里数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public string Get()
        {
            string result = _scheduleTimeDateService.GetAll();
            return result;
        }

        /// <summary>
        /// 新增档案目录
        /// </summary>
        /// <param name="scheduleTimeDate">T</param>
        /// <returns></returns>
        [HttpPost]
        public int Insert(ScheduleTimeDate scheduleTimeDate)
        {
            int bigId = (int)_scheduleTimeDateService.InsertBigIdentity(scheduleTimeDate);
            return bigId;
        }

        /// <summary>
        /// 更新档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPut]
        public bool Update(ScheduleTimeDate obj)
        {
            if (_scheduleTimeDateService.UpdateEntity(obj))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除档案目录
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete]
        public bool Delete(int id)
        {
            Expression<Func<ScheduleTimeDate, bool>> _expression = _expression = f => f.id == id;
            List<ScheduleTimeDate> objList = _scheduleTimeDateService.QueryableToList(_expression);
            if (objList.Count > 0)
            {
                Expression<Func<ScheduleTimeDate, bool>> _expression_del = _expression_del = f => f.id == id;
                if (_scheduleTimeDateService.Delete(_expression_del))
                {
                    return true;
                }
            }
            return false;
        }
    }
}