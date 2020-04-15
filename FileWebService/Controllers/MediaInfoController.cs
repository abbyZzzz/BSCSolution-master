using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Advantech.CoreExtention.Attribute;
using Advantech.Entity;
using Advantech.FileWebService.Service;
using Advantech.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileWebService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class MediaInfoController : ControllerBase
    {
        private readonly IMediaInfoService _mediaInfoService;
        private readonly IMediaGroupService _mediaGroupService;

        public MediaInfoController(IMediaInfoService mediaInfoService, IMediaGroupService mediaGroupService)
        {
            _mediaInfoService = mediaInfoService;
            _mediaGroupService = mediaGroupService;
        }
        
        /// <summary>
        /// 查询媒体1(可以通过id,media_group_id,media_group_id+media_name)查询
        /// 2查询所有，如果未传take参数，则安时间查询100条数据，如果已传入，则按照take数量进行查询，如果传入-1则查询所有
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="media_group_id">群组id</param>
        /// <param name="media_name">群组名称</param>
        /// <returns></returns>
        [HttpGet]
        public List<MediaInfo> Get(int id, int media_group_id, string media_name, int take, string orderType)
        {
            //InitApiUrl();
            List<MediaInfo> objList = new List<MediaInfo>();
            Expression<Func<MediaInfo, bool>> _expression = null; //_expression = f => f.media_group_id == media_group_id;
            if (id > 0 || media_group_id > 0 || (media_group_id > 0 && !string.IsNullOrEmpty(media_name)))
            {
                if (id > 0)
                {
                    _expression = f => f.id == id;
                    objList = _mediaInfoService.QueryableToList(_expression);
                }
                else
                {
                    if (media_group_id > 0 && string.IsNullOrEmpty(media_name))
                    {
                        _expression = f => f.media_group_id == media_group_id;
                        objList = _mediaInfoService.QueryableToList(_expression);
                    }
                    else if (media_group_id > 0 && !string.IsNullOrEmpty(media_name))
                    {
                        _expression = f => f.media_group_id == media_group_id && f.media_name == media_name;
                        objList = _mediaInfoService.QueryableToList(_expression);
                    }
                }
            }
            else
            {
                _expression = f => 0 == 0;
                Expression<Func<MediaInfo, object>> _order = f => f.create_time;
                if (string.IsNullOrEmpty(orderType))
                {
                    orderType = "DESC";
                }
                objList = _mediaInfoService.QueryableToListOrder(_expression, _order, orderType, take);
            }
            if (objList.Count > 0)//将实际地址转义为http地址
            {
                //替换\为/

                objList.All(a => { a.preview_address = GlobalParameter._APIURL + "\\" + a.preview_address.Substring(a.preview_address.IndexOf('\\') + 1, a.preview_address.Length - a.preview_address.IndexOf('\\') - 1); return true; });
                objList.All(a => { a.media_address = GlobalParameter._APIURL + "\\" + a.media_address.Substring(a.media_address.IndexOf('\\') + 1, a.media_address.Length - a.media_address.IndexOf('\\') - 1); return true; });

                objList.All(a => { a.preview_address = a.preview_address.Replace("\\", "/"); return true; });
                objList.All(a => { a.media_address = a.media_address.Replace("\\", "/"); return true; });

            }
            return objList;
        }
        /// <summary>
        /// 返回HttpResponseMessage(HttpStatusCode.OK为成功)
        /// </summary>
        /// <param name="list">List<T></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Edit(List<FileUpload> list)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            FileService fileService = new FileService(_mediaInfoService, _mediaGroupService);
            List<string> objList = fileService.LoadFiles(list);
            if (objList.Count <= 0)
            {
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StringContent("Success");
                message.ReasonPhrase = "Success";
            }
            else
            {
                message.StatusCode = HttpStatusCode.BadRequest;
                string result = Newtonsoft.Json.JsonConvert.SerializeObject(objList);
                message.Content = new StringContent(result);
                message.ReasonPhrase = result;
            }
            return message;


        }
        [HttpDelete]
        //[UserAuthorize(false)]//权限检查,失败则跳转到登录
        public bool Delete(int id)
        {
            Expression<Func<MediaInfo, bool>> _expression = _expression = f => f.id == id;
            MediaInfo mediaInfo = _mediaInfoService.QueryableToEntity(_expression);
            if(mediaInfo!=null)
            {
                FileUpload fileUpload = new FileUpload();
                fileUpload.file_name = mediaInfo.media_name;
                fileUpload.media_group_id = mediaInfo.media_group_id;
                fileUpload.status = "delete";
                fileUpload.file_path = Path.GetDirectoryName(mediaInfo.media_address);
                List<FileUpload> list = new List<FileUpload>();
                list.Add(fileUpload);
                FileService fileService = new FileService(_mediaInfoService, _mediaGroupService);
                List<string> objList = fileService.LoadFiles(list);
                if(objList.Count<=0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


    }
}