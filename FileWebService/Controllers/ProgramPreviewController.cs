using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advantech.CoreExtention.Http;
using Advantech.Entity;
using Advantech.FileWebService.Service;
using Advantech.Service;
using Currency;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Advantech.FileWebService.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class ProgramPreviewController : ControllerBase
    {
        private readonly IMediaInfoService _mediaInfoService;
        private readonly IMediaGroupService _mediaGroupService;

        [HttpPost]
        public bool Add(ProgramInfo programInfo)
        {
            FileService fileService = new FileService(_mediaInfoService, _mediaGroupService);
            bool result = fileService.ProgramProImage(programInfo);
            return result;
        }

        [HttpGet]
        public string Get(int id)
        {
            FileService fileService = new FileService(_mediaInfoService, _mediaGroupService);

            //获取参数
            string filePath = string.Empty;
            string previewPath = string.Empty;
            Dictionary<string, string> result = new Dictionary<string, string>();
            //获取需要替换的前缀
            string  sub1= GlobalParameter._PROGRAM.Split('\\').ToList()[0];
            string sub2 = GlobalParameter._PROGRAMPREVIEW.Split('\\').ToList()[0];
            //将实际路径替换问http路径
            filePath = GlobalParameter._PROGRAM.Replace(sub1, GlobalParameter._APIURL);
            previewPath = GlobalParameter._PROGRAMPREVIEW.Replace(sub2, GlobalParameter._APIURL);

            if(id>0)
            {
                filePath= filePath+"\\"+ fileService.GetFolder(id)+"\\"+ id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
                previewPath= previewPath+"\\"+ id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
                filePath = filePath.Replace('\\', '/');
                previewPath = previewPath.Replace('\\', '/');
                if(!HttpHelper.RemoteFileExists(filePath)||!HttpHelper.RemoteFileExists(previewPath))
                {
                    result.Add("filePath", null);
                    result.Add("previewPath", null);
                }
                else
                {
                    result.Add("filePath", filePath);
                    result.Add("previewPath", previewPath);
                }
                
            }
            else
            {
                filePath = filePath.Replace('\\', '/');
                previewPath = previewPath.Replace('\\', '/');
                result.Add("filePath", filePath);
                result.Add("previewPath", previewPath);
            }
            
           
            return JsonConvert.SerializeObject(result);
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            FileService fileService = new FileService(_mediaInfoService, _mediaGroupService);

            string filePath = GlobalParameter._PROGRAM + "\\" + fileService.GetFolder(id) + "\\" + id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
            string previewPath = GlobalParameter._PROGRAMPREVIEW + "\\" + id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
            if(FileHelper.DelFile(filePath)&& FileHelper.DelFile(previewPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
