using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Advantech.CoreExtention;
using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Entity.UserAndGroup;
using Advantech.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Advantech.BSCWeb.Controllers
{
    public class ProgramController : Controller
    {
        //媒体群组Api对应的url
        private string group_url_get = GlobalParameter._AGENTURL + "/program/ProgramGroup/Get";
        private string group_url_add = GlobalParameter._AGENTURL + "/program/ProgramGroup/Add";
        private string group_url_edit = GlobalParameter._AGENTURL + "/program/ProgramGroup/Edit";
        private string group_url_delete = GlobalParameter._AGENTURL + "/program/ProgramGroup/Delete";

        //节目Api对应的url
        private string program_url_get = GlobalParameter._AGENTURL + "/program/ProgramInfo/Get";
        private string program_url_add = GlobalParameter._AGENTURL + "/program/ProgramInfo/Add";
        private string program_url_edit = GlobalParameter._AGENTURL + "/program/ProgramInfo/Edit";
        private string program_url_delete = GlobalParameter._AGENTURL + "/program/ProgramInfo/Delete";

        private readonly IUserGroupService _userGroupService;
        public ProgramController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetAjaxData(int program_group_id)
        {
          
            return View();
        }

        public IActionResult GetAjaxDel(int[] id_list, int program_group_id)
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        //新增媒体群组
        public IActionResult ToAddGroup(string id, string pId)
        {
            ProgramGroup obj = new ProgramGroup();
            obj.pgroup_name = "new Group";
            if (!Shared.IsNum(id))
            {
                obj.parent_id = 0;
                obj.group_id = Shared.ExtractNum(id);
            }
            else
            {
                obj.parent_id = Shared.ExtractNum(id);
                if (!Shared.IsNum(pId))
                {
                    obj.group_id = Shared.ExtractNum(pId);
                }
                else
                {
                    string url = string.Format("{0}?id={1}", group_url_get, Shared.ExtractNum(pId));
                    string jsonStr = HttpHelper.HttpGet(url);
                    List<ProgramGroup> _programGroup = JsonConvert.DeserializeObject<List<ProgramGroup>>(jsonStr);
                    if (_programGroup.Count > 0)
                    {
                        obj.group_id = _programGroup[0].group_id;
                    }
                }
            }

            string postData = JsonConvert.SerializeObject(obj);
            int bigId = Convert.ToInt32(HttpHelper.HttpPost(group_url_add, postData));
            if (bigId > 0)
            {
                obj.id = bigId;
                return Json(obj);
            }
            else
            {
                return Json(false);
            }

        }

        /// <summary>
        /// 修改媒体群组信息
        /// </summary>
        /// <param name="id">媒体群组的id</param>
        /// <param name="mgroup_name">媒体群组名称</param>
        /// <returns></returns>
        public IActionResult ToPutGroup(int id, string pgroup_name)
        {
            string url = string.Format("{0}?id={1}", group_url_get, id);
            string jsonStr = HttpHelper.HttpGet(url);
            List<ProgramGroup> _programGroup = JsonConvert.DeserializeObject<List<ProgramGroup>>(jsonStr);
            if (_programGroup.Count > 0)
            {
                ProgramGroup obj = _programGroup[0];
                obj.pgroup_name = pgroup_name;
                obj.create_time = DateTime.Now;
                string postData = JsonConvert.SerializeObject(obj);
                bool result = Convert.ToBoolean(HttpHelper.HttpPut(group_url_edit, postData));
                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }

        /// <summary>
        /// 删除媒体群组
        /// </summary>
        /// <param name="id">群组id</param>
        /// <returns></returns>
        public IActionResult ToDelGroup(int id)
        {

            //判断当前群组下是否有媒体
            List<ProgramInfo> programInfos = GetProgramInfos(id);
            if (programInfos.Count <= 0)
            {
                string url_delete = string.Format("{0}?id={1}", group_url_delete, id);
                bool result = Convert.ToBoolean(HttpHelper.HttpDel(url_delete));
                return Json(result);
            }
            else
            {
                return Json(false);
            }

        }

        /// <summary>
        /// 返回Group群组
        /// </summary>
        /// <returns></returns>
        public IActionResult ToSelectGroup()
        {
            List<ZTree> objList = new List<ZTree>();
            //获取用户群组
            Expression<Func<UserGroup, bool>> _expressionUserGroup = _expressionUserGroup = f => 0 == 0;
            List<UserGroup> objUserGroupList = _userGroupService.QueryableToList(_expressionUserGroup);
            if (objUserGroupList.Count > 0)
            {
                //读取媒体群组
                string url = string.Format("{0}?id=-1", group_url_get);
                string jsonStr = HttpHelper.HttpGet(url);
                List<ProgramGroup> objProgramGroupList = JsonConvert.DeserializeObject<List<ProgramGroup>>(jsonStr);

                List<ProgramGroup> programGroupList = new List<ProgramGroup>();
                foreach (var itme in objUserGroupList)
                {
                    ZTree obj = new ZTree();
                    obj.id = GlobalParameter._PREFIX + itme.id;
                    obj.name = itme.group_name;
                    obj.pId = "0";
                    obj.open = true;
                    objList.Add(obj);
                    if (objProgramGroupList.Where(f => f.group_id == itme.id).ToList().Count <= 0)//判断媒体群组是否需要建立默认的
                    {
                        ProgramGroup programGroup = new ProgramGroup();
                        programGroup.parent_id = 0;
                        programGroup.pgroup_name = "new group";
                        programGroup.group_id = itme.id;
                        programGroupList.Add(programGroup);
                    }
                }
                //新增默认的媒体群组
                if (programGroupList.Count > 0)
                {
                    foreach (var items in programGroupList)
                    {
                        string data = JsonConvert.SerializeObject(items);
                        HttpHelper.HttpPost(group_url_add, data);
                    }
                }
                //重新获取媒体群组
                if (objProgramGroupList.Count <= 0)
                {
                    objProgramGroupList = JsonConvert.DeserializeObject<List<ProgramGroup>>(HttpHelper.HttpGet(url));
                }
                foreach (var itme in objProgramGroupList)
                {
                    ZTree obj = new ZTree();

                    obj.id = itme.id.ToString();
                    obj.name = itme.pgroup_name;
                    if (itme.parent_id == 0)
                    {
                        obj.pId = GlobalParameter._PREFIX + itme.group_id;
                    }
                    else
                    {
                        obj.pId = itme.parent_id.ToString();
                    }
                    objList.Add(obj);
                }
            }
            return Json(objList);

        }

        public List<ProgramInfo> GetProgramInfos(int program_group_id)
        {
            string url = string.Format("{0}?program_group_id={1}", program_url_get, program_group_id);
            string jsonStr = HttpHelper.HttpGet(url);
            List<ProgramInfo> programInfos = JsonConvert.DeserializeObject<List<ProgramInfo>>(jsonStr);
            return programInfos;
        }


    }
}