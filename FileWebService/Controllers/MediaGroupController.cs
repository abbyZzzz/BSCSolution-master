using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advantech.Service;
using Advantech.Repository;
using Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Advantech.Entity;
using System.Linq.Expressions;

namespace FileWebService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class MediaGroupController : ControllerBase
    {
        private readonly IMediaGroupService _mediaGroupService;

        public MediaGroupController(IMediaGroupService mediaGroupService)
        {
            _mediaGroupService = mediaGroupService;
        }

        /// <summary>
        /// 查询档案目录
        /// </summary>
        /// <param name="id">-1==查询所有</param>
        /// <param name="parent_id">父节点</param>
        /// <returns></returns>
        [HttpGet]
        public List<MediaGroup> Get(int id,int parent_id)
        {
            List<MediaGroup> objList = new List<MediaGroup>();
            Expression<Func<MediaGroup, bool>> _expression = null;
            if (parent_id <= 0)
            {
                switch (id)
                {
                    case -1:
                        _expression = f => 0 == 0;
                        objList = _mediaGroupService.QueryableToList(_expression);
                        break;
                    default:
                        _expression = f => f.id == id;
                        objList = _mediaGroupService.QueryableToList(_expression);
                        break;
                }
            }
            else
            {
                _expression = f => f.parent_id == parent_id;
                objList = _mediaGroupService.QueryableToList(_expression);
            }
            return objList;
        }

        /// <summary>
        /// 新增档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPost]
        public int Add(MediaGroup mediaGroup)
        {
           int bigId= (int)_mediaGroupService.InsertBigIdentity(mediaGroup);
           return bigId;
        }

        /// <summary>
        /// 更新档案目录
        /// </summary>
        /// <param name="obj">T</param>
        /// <returns></returns>
        [HttpPut]
        public bool Edit(MediaGroup obj)
        {
            if (_mediaGroupService.UpdateEntity(obj))
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
            Expression<Func<MediaGroup, bool>> _expression= _expression = f => f.parent_id == id;
            List<MediaGroup> objList = _mediaGroupService.QueryableToList(_expression);
            if(objList.Count==0)
            {
                Expression<Func<MediaGroup, bool>> _expression_del = _expression_del = f => f.id == id;
                if(_mediaGroupService.Delete(_expression_del))
                {
                    return true;
                }
            }
            return false;
        }
    }

  
}
