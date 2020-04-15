using System;
using System.Collections.Generic;
using System.IO;
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

namespace BSC.Controllers
{
    public class MediaController : Controller
    {
        //媒体群组Api对应的url
        private string group_url_get = GlobalParameter._AGENTURL+ "/file/MediaGroup/Get";
        private string group_url_add = GlobalParameter._AGENTURL + "/file/MediaGroup/Add";
        private string group_url_edit = GlobalParameter._AGENTURL + "/file/MediaGroup/Edit";
        private string group_url_delete = GlobalParameter._AGENTURL + "/file/MediaGroup/Delete";

        //媒体Api对应的url
        private string media_url_get= GlobalParameter._AGENTURL + "/file/MediaInfo/Get";
        private string media_url_edit = GlobalParameter._AGENTURL + "/file/MediaInfo/Edit";
        private string media_url_delete = GlobalParameter._AGENTURL + "/file/MediaInfo/Delete";

        //文件上传Api对应的url
        private string media_url_upload = GlobalParameter._AGENTURL + "/file/FileUpload/UploadFile";

        private readonly IMediaGroupService _mediaGroupService;
        private readonly IUserGroupService _userGroupService;
        public MediaController(IMediaGroupService mediaGroupService, IUserGroupService userGroupService)
        {
            _mediaGroupService = mediaGroupService;
            _userGroupService = userGroupService;
        }

        /// <summary>
        /// 媒体首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            string jsonStr = HttpHelper.HttpGet(media_url_get);
            List<MediaInfo> mediaInfos = JsonConvert.DeserializeObject<List<MediaInfo>>(jsonStr);
            ViewBag.url_upload = media_url_upload;
            return View(mediaInfos);
        }

        /// <summary>
        /// AJAX获取媒体信息
        /// </summary>
        /// <param name="media_group_id">媒体群组id</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAjaxData(int media_group_id)
        {
            List<MediaInfo> mediaInfos = GetMediaInfos(media_group_id);
            return View(mediaInfos);
        }

        /// <summary>
        /// 删除媒体
        /// </summary>
        /// <param name="id_list">媒体id群组</param>
        /// <param name="media_group_id">媒体群组id</param>
        /// <returns></returns>
        public IActionResult GetAjaxDel(int[] id_list,int media_group_id)
        {
           //删除媒体
           if(id_list.Length>0)
            {
                for (int i = 0; i < id_list.Length; i++)
                {
                    string url = string.Format("{0}?id={1}", media_url_delete, id_list[i]);
                    HttpHelper.HttpDel(url);
                }
            }
            //重新获取当前群组下的媒体
            string url_get = string.Format("{0}?media_group_id={1}", media_url_get, media_group_id);
            string jsonStr = HttpHelper.HttpGet(url_get);
            List<MediaInfo> mediaInfos = JsonConvert.DeserializeObject<List<MediaInfo>>(jsonStr);
            return View(mediaInfos);
        }
        
        //新增媒体群组
        public IActionResult ToAddGroup(string id,string pId)
        {
            MediaGroup obj = new MediaGroup();
            obj.mgroup_name = "new Group";
            if(!Shared.IsNum(id))
            {
                obj.parent_id = 0; 
                obj.group_id= Shared.ExtractNum(id);
            }
            else
            {
                obj.parent_id = Shared.ExtractNum(id);
                if(!Shared.IsNum(pId))
                {
                    obj.group_id= Shared.ExtractNum(pId);
                }
                else
                {
                    string url = string.Format("{0}?id={1}", group_url_get, Shared.ExtractNum(pId));
                    string jsonStr = HttpHelper.HttpGet(url);
                    List<MediaGroup> _mediaGroup = JsonConvert.DeserializeObject<List<MediaGroup>>(jsonStr);
                    if (_mediaGroup.Count > 0)
                    {
                        obj.group_id = _mediaGroup[0].group_id;
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
        public IActionResult ToPutGroup(int id, string mgroup_name)
        {
            string url = string.Format("{0}?id={1}", group_url_get, id);
            string jsonStr = HttpHelper.HttpGet(url);
            List<MediaGroup> _mediaGroup = JsonConvert.DeserializeObject<List<MediaGroup>>(jsonStr);
            if(_mediaGroup.Count>0)
            {
                MediaGroup obj = _mediaGroup[0];
                obj.mgroup_name = mgroup_name;
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
            List<MediaInfo> mediaInfos = GetMediaInfos(id);
            if (mediaInfos.Count <= 0)
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
            if(objUserGroupList.Count>0)
            {
                //读取媒体群组
                string url = string.Format("{0}?id=-1", group_url_get);
                string jsonStr =  HttpHelper.HttpGet(url);
                List <MediaGroup> objMediaGroupList= JsonConvert.DeserializeObject<List<MediaGroup>>(jsonStr);

                List<MediaGroup> mediaGroupList = new List<MediaGroup>();
                foreach (var itme in objUserGroupList)
                {
                    ZTree obj = new ZTree();
                    obj.id = GlobalParameter._PREFIX + itme.id;
                    obj.name = itme.group_name;
                    obj.pId = "0";
                    obj.open = true;
                    objList.Add(obj);
                    if(objMediaGroupList.Where(f=>f.group_id== itme.id).ToList().Count<=0)//判断媒体群组是否需要建立默认的
                    {
                        MediaGroup mediaGroup = new MediaGroup();
                        mediaGroup.parent_id = 0;
                        mediaGroup.mgroup_name = "new group";
                        mediaGroup.group_id = itme.id;
                        mediaGroupList.Add(mediaGroup);
                    }
                }
                //新增默认的媒体群组
                if(mediaGroupList.Count>0)
                {
                    foreach(var items in mediaGroupList)
                    {
                        string data = JsonConvert.SerializeObject(items);
                        HttpHelper.HttpPost(group_url_add, data);
                    }
                }
                //重新获取媒体群组
                if(objMediaGroupList.Count<=0)
                {
                    objMediaGroupList= JsonConvert.DeserializeObject<List<MediaGroup>>(HttpHelper.HttpGet(url));
                }
                foreach (var itme in objMediaGroupList)
                {
                    ZTree obj = new ZTree();

                    obj.id = itme.id.ToString();
                    obj.name = itme.mgroup_name;
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

        [HttpPost]
        public IActionResult Verification(int media_group_id, string[] fileNameList)
        {
            //ajax状态
            bool result = true;
            //是否存在重复的
            bool isRepeat = false;
            //不重复的媒体名称
            List<string> temList = new List<string>();

            Dictionary<string, object> list = new Dictionary<string, object>();
            try
            {
                string url = string.Format("{0}?media_group_id={1}", media_url_get, media_group_id);
                List<MediaInfo> objList = JsonConvert.DeserializeObject<List<MediaInfo>>(HttpHelper.HttpGet(url));
                if(objList.Count<=0)
                {
                    list.Add("result", result);
                    list.Add("isRepeat", isRepeat);
                    list.Add("messageInfo", fileNameList);
                }
                else
                {
                    list.Add("result", result);
                    for (int i=0;i< fileNameList.Length;i++)
                    {
                        if (objList.Exists(f => f.media_name == fileNameList[i]))
                        {
                            isRepeat = true;
                        }
                        else
                        {
                            temList.Add(fileNameList[i]);
                        }
                    }
                    list.Add("isRepeat", isRepeat);
                    list.Add("messageInfo", temList.ToArray());
                }
            }
            catch
            {
                temList = new List<string>();
                list.Add("result", false);
                list.Add("isRepeat", isRepeat);
                list.Add("messageInfo", temList.ToArray());
            }
            return Json(list);

        }

        [HttpGet]
        public IActionResult GetGuid()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string guid = Guid.NewGuid().ToString();
            result.Add("guid", guid);
            return Json(result);

        }

        public List<MediaInfo> GetMediaInfos(int media_group_id)
        {
            string url = string.Format("{0}?media_group_id={1}", media_url_get, media_group_id);
            string jsonStr = HttpHelper.HttpGet(url);
            List<MediaInfo> mediaInfos = JsonConvert.DeserializeObject<List<MediaInfo>>(jsonStr);
            return mediaInfos;
        }

    }
}