using Microsoft.AspNetCore.Mvc;
using System;
using Advantech.Service;
using Microsoft.AspNetCore.Authorization;
using Advantech.Entity.UserAndGroup;

namespace Advantech.SecurityService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }
        /// <summary>
        /// 获取用户组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            var group = _userGroupService.QueryableToEntity(x => x.id == id);
            return new JsonResult(group);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Add([FromBody] UserGroup userGroup)
        {
            var group = _userGroupService.QueryableToEntity(x => x.group_name == userGroup.group_name);
            if (group == null)
            {
                group.create_time = DateTime.Now;
               
                if (_userGroupService.Insert(group))
                {
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("fail");
                }
            }
            return new JsonResult("分组已经存在");
        }
        [HttpDelete]
        [Authorize]
        public IActionResult Delete([FromBody] int id)
        {
            var groupInfo = _userGroupService.QueryableToEntity(x => x.id == id);
            if (groupInfo != null)
            {
                if (_userGroupService.Delete(groupInfo))
                {
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("fail");
                }
            }
            return new JsonResult("分组不存在");
        }
        [HttpPut]
        [Authorize]
        public IActionResult Update([FromBody] UserGroup userGroup)
        {
            var group = _userGroupService.QueryableToEntity(x => x.id == userGroup.id);
            if (group != null)
            {
                if (_userGroupService.Update(userGroup))
                {
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("fail");
                }
            }
            return new JsonResult("分组不存在");
        }
        
    }
}
