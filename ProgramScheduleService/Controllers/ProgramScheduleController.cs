using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Advantech.Entity;
using Advantech.Entity.Schedule;
using Advantech.Entity.UserAndGroup;
using Advantech.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;

namespace Advantech.ProgramScheduleService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProgramScheduleController : ControllerBase
    {
        private readonly IProgramScheduleService _programScheduleService;

        public ProgramScheduleController( IProgramScheduleService programScheduleService)
        {
            _programScheduleService = programScheduleService;
        }

        /// <summary>
        /// 获取原始数据库里数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public string Get()
        {
            string result = _programScheduleService.GetAll();
            return result;
        }

        /// <summary>
        /// 加入关联id对应的name
        /// </summary>
        /// <param name="details">传递的参数</param>
        /// <returns></returns>
        [HttpGet("all/{details}")]
        public string Get(string details)
        {
            string result = "";
            if (details == "details")
            {
                result = _programScheduleService.GetAllDetails();
            }
            else if (details=="union_details") {
                result = _programScheduleService.GetAllUnionQuery();
            }
            return result;
        }

        /// <summary>
        /// 单笔查询原始数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            string result = _programScheduleService.GetById(id);
            return result;
        }

        /// <summary>
        /// 加入关联的id对应的name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{details}/{id}")]
        public string Get(string details, int id)
        {
            string result = "";
            if (details == "details")
            {
                result = _programScheduleService.GetDetailsById(id);
            }
            else if (details == "union_details") {
                result = JsonConvert.SerializeObject(_programScheduleService.GetCompositeById(id));
            }
            return result;
        }


        /// <summary>
        /// 新增档案目录
        /// </summary>
        /// <param name="scheduleDay">T</param>
        /// <returns></returns>
        [HttpPost]
        public int Insert(ProgramSchedule scheduleDay)
        {
            int bigId = (int)_programScheduleService.InsertBigIdentity(scheduleDay);
            return bigId;
        }
        /// <summary>
        /// 更新档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPut]
        public bool Update(ProgramSchedule obj)
        {
            if (_programScheduleService.UpdateEntity(obj))
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
            Expression<Func<ProgramSchedule, bool>> _expression = _expression = f => f.id == id;
            List<ProgramSchedule> objList = _programScheduleService.QueryableToList(_expression);
            if (objList.Count > 0)
            {
                Expression<Func<ProgramSchedule, bool>> _expression_del = _expression_del = f => f.id == id;
                if (_programScheduleService.Delete(_expression_del))
                {
                    return true;
                }
            }
            return false;
        }
    }
}