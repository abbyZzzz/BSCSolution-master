using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Advantech.Entity;
using Advantech.FileWebService.Service;
using Advantech.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FileWebService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IMediaInfoService _mediaInfoService;
        private readonly IMediaGroupService _mediaGroupService;

        public FileUploadController(IMediaInfoService mediaInfoService, IMediaGroupService mediaGroupService)
        {
            _mediaInfoService = mediaInfoService;
            _mediaGroupService = mediaGroupService;
        }
        public string Get()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("number", "1");
            result.Add("mergeOk", true);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 文件拆分上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> UploadFile()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            //所在目录id
            var mediaGroupId = Request.Form["mediaGroupId"];
            //媒体名称
            var mediaName = Request.Form["mediaName"];
            //slice方法用于切出文件的一部分
            var data = Request.Form.Files["data"];
            //文件最后的修改日期
            string guid = Request.Form["guid"].ToString();
            //总的切分数量
            var total = Request.Form["total"];
            //当前是第几块文件
            var index = Request.Form["index"];
            //临时保存分块的目录
            string temporary = Path.Combine(GlobalParameter._TEMPROARY, guid);
            try
            {
                if (!Directory.Exists(temporary))
                {
                    Directory.CreateDirectory(temporary);
                }
                string filePath = Path.Combine(temporary, index.ToString());
                if (!Convert.IsDBNull(data))
                {
                    await Task.Run(() =>
                    {
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        data.CopyTo(fs);
                        fs.Close();
                        fs.Dispose();
                    });
                }
                bool mergeResult = false;
                string errorInfo = string.Empty;
                if (total == index)//全部上传完成进行组合，并返回组合好的文件路径
                {
                    errorInfo = await FileMerge(guid, mediaName, Convert.ToInt32(mediaGroupId));
                    if (string.IsNullOrEmpty(errorInfo))
                    {
                        mergeResult = true;
                    }
                }
                result.Add("number", index);
                result.Add("mergeResult", mergeResult);
                result.Add("errorInfo", errorInfo);

            }
            catch (Exception ex)
            {
                var files = Directory.GetFiles(temporary);//获得下面的所有文件
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                {
                    System.IO.File.Delete(part);//删除分块
                }
                Directory.Delete(temporary);//删除文件夹

                result.Add("number", index);
                result.Add("mergeResult", false);
                result.Add("errorInfo", ex.Message);
            }
            return JsonConvert.SerializeObject(result);
        }

        private async Task<string> FileMerge(string guid,string mediaName,int mediaGroupId)
        {
            string result = string.Empty;
            //媒体临时文件夹
            string guidFolder = Path.Combine(GlobalParameter._TEMPROARY, Guid.NewGuid().ToString());
            //分块临时文件夹
            string temporary = Path.Combine(GlobalParameter._TEMPROARY, guid);//临时文件夹
            try
            {
                if(!Directory.Exists(guidFolder))
                {
                    Directory.CreateDirectory(guidFolder);
                }
                var files = Directory.GetFiles(temporary);//获得下面的所有文件
                var finalPath = Path.Combine(guidFolder, mediaName);
                var fs = new FileStream(finalPath, FileMode.Create);
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                {
                    var bytes = System.IO.File.ReadAllBytes(part);
                    await fs.WriteAsync(bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete(part);//删除分块
                }
                fs.Close();
                fs.Dispose();
                Directory.Delete(temporary);//删除文件夹

                //处理文件
                FileUpload fileUpload = new FileUpload();
                fileUpload.file_name = mediaName;
                fileUpload.status = "add";
                fileUpload.file_path = Path.GetDirectoryName(finalPath);
                fileUpload.media_group_id = mediaGroupId;

                List<FileUpload> fileUploads = new List<FileUpload>();
                fileUploads.Add(fileUpload);

                FileService fileService = new FileService(_mediaInfoService,_mediaGroupService);
                List<string> objreList= fileService.LoadFiles(fileUploads);
                if(objreList.Count>0)
                {
                    result = objreList[0];
                }
                System.IO.File.Delete(finalPath);
                Directory.Delete(Path.GetDirectoryName(finalPath));//删除文件夹
            }
            catch(Exception ex)
            {
                var files = Directory.GetFiles(temporary);//获得下面的所有文件
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                {
                    System.IO.File.Delete(part);//删除分块
                }
                Directory.Delete(temporary);//删除文件夹

                result = ex.Message;
            }
            return result;
        }


        
    }
}