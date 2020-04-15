using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Advantech.ProgramScheduleService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProgramInfoController : Controller
    {
        private string programInfo_url_del = GlobalParameter._AGENTURL + "/program/ProgramInfo/Delete";
        
        private string programPreview_url_get = GlobalParameter._AGENTURL + "/file/ProgramPreview/Get";
        private string programPreview_url_add = GlobalParameter._AGENTURL + "/file/ProgramPreview/Add";
        private string programPreview_url_del = GlobalParameter._AGENTURL + "/file/ProgramPreview/Delete";
        private readonly IProgramInfoService _programInfoService;
        private readonly IProgramRegionService _programRegionService;
        private readonly IProgramRegionMediaService _programRegionMediaService;

        public ProgramInfoController(IProgramInfoService programInfoService, IProgramRegionService programRegionService, IProgramRegionMediaService programRegionMediaService)
        {
            _programInfoService = programInfoService;
            _programRegionService = programRegionService;
            _programRegionMediaService = programRegionMediaService;
        }
        [HttpGet]
        public List<ProgramInfo> Get(int id, int program_group_id, string program_name, int take, string orderType)
        {
            List<ProgramInfo> programInfos = new List<ProgramInfo>();
            Expression<Func<ProgramInfo, bool>> _expression = null;
            if (id > 0 || program_group_id > 0 || (program_group_id > 0 && !string.IsNullOrEmpty(program_name)))
            {
                if (id > 0)
                {
                    _expression = f => f.id == id;
                    programInfos = _programInfoService.QueryableToList(_expression);
                }
                else
                {
                    if (program_group_id > 0 && string.IsNullOrEmpty(program_name))
                    {
                        _expression = f => f.program_group_id == program_group_id;
                        programInfos = _programInfoService.QueryableToList(_expression);
                    }
                    else if (program_group_id > 0 && !string.IsNullOrEmpty(program_name))
                    {
                        _expression = f => f.program_group_id == program_group_id && f.program_name == program_name;
                        programInfos = _programInfoService.QueryableToList(_expression);
                    }
                }
            }
            else
            {
                _expression = f => 0 == 0;
                Expression<Func<ProgramInfo, object>> _order = f => f.create_time;
                if (string.IsNullOrEmpty(orderType))
                {
                    orderType = "DESC";
                }
                programInfos = _programInfoService.QueryableToListOrder(_expression, _order, orderType, take);
            }
            //获取附属信息
            if(programInfos.Count>0)
            {
                //一次性取出附属数据//会占用内存，如数据量大的时候可以更改为分次获取
                List<ProgramRegion> programRegions = new List<ProgramRegion>();
                List<ProgramRegionMedia> programRegionMedias = new List<ProgramRegionMedia>();
                Expression<Func<ProgramRegion, bool>> _expressions1 = f => 0 == 0;
                Expression<Func<ProgramRegionMedia, bool>> _expressions2 = f => 0 == 0;
                programRegions=_programRegionService.QueryableToList(_expressions1);
                programRegionMedias=_programRegionMediaService.QueryableToList(_expressions2);

                foreach(var itme in programInfos)
                {
                    //获取节目对应的图片
                    string url = string.Format("{0}?id={1}", programPreview_url_get, itme.id);
                    string result= HttpHelper.HttpGet(url);
                    if(!string.IsNullOrEmpty(result))
                    {
                        JObject objResp = (JObject)JsonConvert.DeserializeObject(result);
                        itme.program_address = objResp["filePath"].ToString();
                        itme.preview_address = objResp["previewPath"].ToString();
                    }
                    //获取下属信息
                    itme.RegionList = programRegions.FindAll(o => o.program_id == itme.id);
                    foreach (var itmes in itme.RegionList)
                    {
                        itmes.MediaList = programRegionMedias.FindAll(o => o.region_id == itmes.id);
                    }
                }
            }
                return programInfos;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var program = _programInfoService.GetCompositeById(id);
            return new JsonResult(program);
        }
        [HttpPost]
        public int Add(ProgramInfo programInfo)
        {
            int id = 0;
            try
            {
                id = (int)_programInfoService.InsertBigIdentity(programInfo);
                if (id > 0)
                {
                    string postData = JsonConvert.SerializeObject(programInfo);
                    //生成节目预览图
                    if (Convert.ToBoolean(HttpHelper.HttpPost(programPreview_url_add, postData)))
                    {
                        List<ProgramRegion> programRegions = programInfo.RegionList;
                        if (programRegions.Count > 0)
                        {
                            foreach (var itme in programRegions)
                            {
                                itme.program_id = id;
                                int programRegionId = (int)_programRegionService.InsertBigIdentity(itme);
                                if (programRegionId > 0)
                                {
                                    List<ProgramRegionMedia> programRegionMedias = itme.MediaList;
                                    if (programRegionMedias.Count > 0)
                                    {
                                        foreach (var itmes in programRegionMedias)
                                        {
                                            itmes.region_id = programRegionId;
                                            if (_programRegionMediaService.InsertBigIdentity(itmes) <= 0)
                                            {
                                                string url = string.Format("{0}?id={1}", programInfo_url_del, id);
                                                HttpHelper.HttpDel(url);
                                                //删除
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string url = string.Format("{0}?id={1}", programInfo_url_del, id);
                                    HttpHelper.HttpDel(url);
                                    //删除
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        string url = string.Format("{0}?id={1}", programInfo_url_del, id);
                        HttpHelper.HttpDel(url);
                        id = 0;
                    }
                }
                return id;
            }
            catch
            {
                if (id > 0)
                {
                    string url = string.Format("{0}?id={1}", programInfo_url_del, id);
                    HttpHelper.HttpDel(url);
                    //删除
                    id = 0;
                }
                return id;
            }
        }

        [HttpPut]
        public bool Edit(ProgramInfo programInfo)
        {
            bool result = true;
            try
            {
                if (_programInfoService.UpdateEntity(programInfo))
                {
                    string postData = JsonConvert.SerializeObject(programInfo);
                    //生成节目预览图
                    if (Convert.ToBoolean(HttpHelper.HttpPost(programPreview_url_add, postData)))
                    {
                        List<ProgramRegion> programRegions = programInfo.RegionList;
                        if (programRegions.Count > 0)
                        {
                            //获取节目下的所有区块
                            Expression<Func<ProgramRegion, bool>> _expression = f => f.program_id == programInfo.id;
                            List<ProgramRegion> programRegions1 = _programRegionService.QueryableToList(_expression);
                            List<ProgramRegion> programRegions2 = programRegions1;
                            foreach (var itme in programRegions)
                            {
                                //判断当前区块是否在数据库
                                if (programRegions1.Find(o => o.id == itme.id) != null)
                                {
                                    programRegions1.RemoveAll(o => o.id == itme.id);
                                }

                                //判断当前区块是否已经在数据库中存在
                                if (programRegions2.Find(o => o.id == itme.id) != null)
                                {
                                    //更新
                                    if (!_programRegionService.UpdateEntity(itme))
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    //新增
                                    itme.program_id = programInfo.id;
                                    itme.id = (int)_programRegionService.InsertBigIdentity(itme);
                                    if (itme.id <= 0)
                                    {
                                        result = false;
                                    }
                                }
                                if (!result)
                                {
                                    break;
                                }

                                //获取区块下的媒体信息
                                List<ProgramRegionMedia> programRegionMedias = itme.MediaList;

                                //从数据库中获取对应的信息
                                Expression<Func<ProgramRegionMedia, bool>> _expressionRM = f => f.region_id == itme.id;
                                List<ProgramRegionMedia> programRegionMedias1 = _programRegionMediaService.QueryableToList(_expressionRM);
                                List<ProgramRegionMedia> programRegionMedias2 = programRegionMedias1;

                                foreach (var itmes in programRegionMedias)
                                {
                                    if (programRegionMedias1.Find(o => o.id == itmes.id) != null)
                                    {
                                        programRegionMedias1.RemoveAll(o => o.id == itmes.id);
                                    }

                                    if (programRegionMedias2.Find(o => o.id == itmes.id) != null)
                                    {
                                        //更新
                                        if (!_programRegionMediaService.UpdateEntity(itmes))
                                        {
                                            result = false;
                                        }
                                    }
                                    else
                                    {
                                        itmes.region_id = itme.id;
                                        //新增
                                        if (_programRegionMediaService.InsertBigIdentity(itmes) <= 0)
                                        {
                                            result = false;
                                        }
                                    }
                                    if (!result)
                                    {
                                        break;
                                    }
                                }
                                if (result)
                                {
                                    if (programRegionMedias1.Count > 0)
                                    {
                                        foreach (var itmes in programRegionMedias1)
                                        {
                                            Expression<Func<ProgramRegionMedia, bool>> _expressionDel = f => f.id == itme.id;
                                            _programRegionMediaService.Delete(_expressionDel);
                                        }
                                    }
                                }
                            }
                            if (result)
                            {
                                if (programRegions1.Count > 0)
                                {
                                    //删除
                                    foreach (var itme in programRegions1)
                                    {
                                        Expression<Func<ProgramRegion, bool>> _expressionDel = f => f.id == itme.id;
                                        _programRegionService.Delete(_expressionDel);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result = false;
                    }

                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            bool result = true;
            if (id > 0)
            {
                Expression<Func<ProgramRegion, bool>> _expression = f => f.program_id == id;
                List<ProgramRegion> programRegions = _programRegionService.QueryableToList(_expression);
                foreach(var itme in programRegions)
                {
                    Expression<Func<ProgramRegionMedia, bool>> _expressionDel = f => f.region_id == itme.id;
                    if(! _programRegionMediaService.Delete(_expressionDel))
                    {
                        result = false;
                        break;
                    }
                }
                if(result)
                {
                    Expression<Func<ProgramRegion, bool>> _expressionDel = f => f.program_id == id;
                    _programRegionService.Delete(_expressionDel);

                    Expression<Func<ProgramInfo, bool>> _expressionDelInfo = f => f.id == id;
                    _programInfoService.Delete(_expressionDelInfo);

                    //调用api删除节目图片及预览图
                    string url = string.Format("{0}?id={1}", programPreview_url_del, id);
                    HttpHelper.HttpDel(url);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
