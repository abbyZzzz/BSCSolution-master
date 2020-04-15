using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Advantech.Entity;
using Advantech.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Advantech.ProgramScheduleService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProgramGroupController : ControllerBase
    {
        private readonly IProgramGroupService _programGroupService;

        public ProgramGroupController(IProgramGroupService programGroupService)
        {
            _programGroupService = programGroupService;
        }
        /// <summary>
        /// 查询档案目录
        /// </summary>
        /// <param name="id">-1==查询所有</param>
        /// <param name="parent_id">父节点</param>
        /// <returns></returns>
        [HttpGet]
        public List<ProgramGroup> Get(int id, int parent_id)
        {
            List<ProgramGroup> objList = new List<ProgramGroup>();
            Expression<Func<ProgramGroup, bool>> _expression = null;
            if (parent_id <= 0)
            {
                switch (id)
                {
                    case -1:
                        _expression = f => 0 == 0;
                        objList = _programGroupService.QueryableToList(_expression);
                        break;
                    default:
                        _expression = f => f.id == id;
                        objList = _programGroupService.QueryableToList(_expression);
                        break;
                }
            }
            else
            {
                _expression = f => f.parent_id == parent_id;
                objList = _programGroupService.QueryableToList(_expression);
            }
            return objList;
        }

        /// <summary>
        /// 新增档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPost]
        public int Add(ProgramGroup programGroup)
        {
            int bigId = (int)_programGroupService.InsertBigIdentity(programGroup);
            return bigId;
        }

        /// <summary>
        /// 更新档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPut]
        public bool Edit(ProgramGroup programGroup)
        {
            if (_programGroupService.UpdateEntity(programGroup))
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
            Expression<Func<ProgramGroup, bool>> _expression = _expression = f => f.parent_id == id;
            List<ProgramGroup> objList = _programGroupService.QueryableToList(_expression);
            if (objList.Count == 0)
            {
                Expression<Func<ProgramGroup, bool>> _expression_del = _expression_del = f => f.id == id;
                if (_programGroupService.Delete(_expression_del))
                {
                    return true;
                }
            }
            return false;
        }
    }
}