﻿using System;
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
    public class ScheduleDayController : ControllerBase
    {
        private readonly IScheduleDayService _scheduleDayService;
        public ScheduleDayController(IScheduleDayService scheduleDayService) {
            _scheduleDayService = scheduleDayService;
        }

        /// <summary>
        /// 获取原始数据库里数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public string Get()
        {
            string result = _scheduleDayService.GetAll();
            return result;
        }

        /// <summary>
        /// 加入关联id对应的name
        /// </summary>
        /// <param name="details">传递的参数</param>
        /// <returns></returns>
        [HttpGet("all/details")]
        public string Get(string details)
        {
            string result = _scheduleDayService.GetAllDetails();
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
            string result = _scheduleDayService.GetById(id);
            return result;
        }

        /// <summary>
        /// 加入关联的id对应的name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("details/{id}")]
        public string Get(string details,int id)
        {
            string result = _scheduleDayService.GetDetailsById(id);

            return result;
        }

        /// <summary>
        /// 新增档案目录
        /// </summary>
        /// <param name="scheduleDay">T</param>
        /// <returns></returns>
        [HttpPost]
        public int Insert(ScheduleDay scheduleDay)
        {
            int bigId = (int)_scheduleDayService.InsertBigIdentity(scheduleDay);
            return bigId;
        }

        /// <summary>
        /// 更新档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPut]
        public bool Update(ScheduleDay obj)
        {
            if (_scheduleDayService.UpdateEntity(obj))
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
            Expression<Func<ScheduleDay, bool>> _expression = _expression = f => f.id == id;
            List<ScheduleDay> objList = _scheduleDayService.QueryableToList(_expression);
            if (objList.Count > 0)
            {
                Expression<Func<ScheduleDay, bool>> _expression_del = _expression_del = f => f.id == id;
                if (_scheduleDayService.Delete(_expression_del))
                {
                    return true;
                }
            }
            return false;
        }
    }
}