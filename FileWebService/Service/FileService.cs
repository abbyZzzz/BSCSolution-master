using Advantech.CommonDefine;
using Advantech.Entity;
using Advantech.Service;
using Currency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Advantech.FileWebService.Service
{
    public class FileService
    {
        private readonly IMediaInfoService _mediaInfoService;
        private readonly IMediaGroupService _mediaGroupService;
        public FileService(IMediaInfoService mediaInfoService, IMediaGroupService mediaGroupService)
        {
            _mediaInfoService = mediaInfoService;
            _mediaGroupService = mediaGroupService;
        }

        /// <summary>
        /// 根据节目信息生成节目预览图
        /// </summary>
        /// <param name="programInfo">节目信息</param>
        /// <returns></returns>
        public bool ProgramProImage(ProgramInfo programInfo)
        {
            bool result = true;
            int id = programInfo.id;
            string previewPath = GlobalParameter._PROGRAMPREVIEW + "\\" + id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
            string filePath= GlobalParameter._PROGRAM+"\\"+ GetFolder(id)+"\\"+ id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
            List<ProgramRegion> programRegions = programInfo.RegionList;
            List<string> imgs = new List<string>();
            try
            {
                foreach (var itme in programRegions)
                {
                    int media_id = itme.MediaList.OrderBy(o => o.play_order).Take(1).ToList()[0].media_id;
                    Expression<Func<MediaInfo, bool>> _expression = f => f.id == media_id;
                    MediaInfo mediaInfo = _mediaInfoService.QueryableToEntity(_expression);
                    if (mediaInfo != null)
                    {
                        if (mediaInfo.media_type == "image")
                        {
                            imgs.Add(mediaInfo.media_address);
                        }
                        else
                        {
                            imgs.Add(mediaInfo.preview_address);
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    result = FileHelper.CompositePreview(programInfo.w, programInfo.h, filePath, previewPath, imgs, programRegions);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public List<string> LoadFiles(List<FileUpload> list)
        {
            List<string> retContent = new List<string>();
            string fullPath;//完整路径
            foreach (var item in list)
            {
                string feedbackInfo = string.Empty;
                if (string.IsNullOrEmpty(item.file_name) || string.IsNullOrEmpty(item.status) || string.IsNullOrEmpty(item.file_path) || item.media_group_id == 0)
                {
                    retContent.Add(item.file_name + ":参数不能为空");
                    continue;
                }

                
                Expression<Func<MediaGroup, bool>> _expression = _expression = f => f.id == item.media_group_id;
                MediaGroup mediaGroup= _mediaGroupService.QueryableToEntity(_expression);
                if(mediaGroup==null)
                {
                    retContent.Add(item.file_name + ":媒体群组Id错误");
                    continue;
                }
                FileType fileType = new FileType();
                if (item.status.ToLower().Trim() != "delete")
                {
                    fullPath = item.file_path + "\\" + item.file_name;//组合路径
                    if (!System.IO.File.Exists(fullPath))
                    {
                        retContent.Add(item.file_name + ":文件路径错误");
                        continue;
                    }
                    fileType = FileHelper.CheckFileType(fullPath);
                    if (fileType.ToString() == "nosupport")
                    {
                        retContent.Add(item.file_name + ":文件格式不支持");
                        continue;
                    }
                }

                switch (item.status.ToLower().Trim())
                {
                    case "add"://新增
                        if (!Add(item, fileType, ref feedbackInfo))
                        {
                            retContent.Add(feedbackInfo);
                        }
                        break;
                    case "update"://更新
                        if(!Update(item, fileType, ref feedbackInfo))
                        {
                            retContent.Add(feedbackInfo);
                        }
                        break;
                    case "delete"://删除
                        if(!Del(item, ref feedbackInfo))
                        {
                            retContent.Add(feedbackInfo);
                        }
                        break;
                    default:
                        feedbackInfo = item.file_name + ":未识别的status";
                        retContent.Add(feedbackInfo);
                        break;
                }

            }
            return retContent;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileUpload">T</param>
        /// <param name="retContent">异常反馈信息</param>
        /// <returns></returns>
        private bool Del(FileUpload fileUpload,ref string retContent)
        {
            bool result= false;
            try
            {
                //查询数据是否存在
                MediaInfo MediaInfo = SelectSingle(fileUpload.media_group_id, fileUpload.file_name);
                if (MediaInfo != null)
                {
                    //根据id删除数据库数据
                    Expression<Func<MediaInfo, bool>> _expression = _expression = f => f.id == MediaInfo.id;
                    if (_mediaInfoService.Delete(_expression))
                    {
                        FileHelper.DelFile(MediaInfo.media_address);
                        FileHelper.DelFile(MediaInfo.preview_address);
                        result = true;
                    }
                    else
                    {
                        retContent = fileUpload.file_name + ":删除数据失败";
                    }
                }
                else
                {
                    retContent = fileUpload.file_name + ":参数不能为空";
                }
            }
            catch(Exception ex)
            {
                retContent = fileUpload.file_name + ":"+ex.Message;
            }
            return result;


        }

        private bool Add(FileUpload fileUpload, FileType fileType , ref string retContent)
        {
            bool result = false;
            try
            {
                string previewPath = string.Empty;
                string mediaPath = string.Empty;
                //查询数据是否存在
                MediaInfo MediaInfo = SelectSingle(fileUpload.media_group_id, fileUpload.file_name);
                if(MediaInfo==null)
                {
                    //ffmpeg地址
                    string ffmpegPath = GlobalParameter._FFMPEG;
                    //获取文件路径
                    string filePath = fileUpload.file_path + "\\" + fileUpload.file_name;
                    //获取文件信息
                    MediaInfo _MediaInfo = FileHelper.GetMediaAttribute(filePath, ffmpegPath);
                    _MediaInfo.media_type = fileType.ToString();
                    _MediaInfo.media_name = fileUpload.file_name;
                    _MediaInfo.media_group_id = fileUpload.media_group_id;
                    //插入数据并获取自增长id
                    int id = (int)_mediaInfoService.InsertBigIdentity(_MediaInfo);
                    if(id>0)
                    {
                        if (FileProcess(filePath, id, ref previewPath, ref mediaPath))
                        {
                            _MediaInfo.serial_number = Path.GetFileName(mediaPath);
                            _MediaInfo.media_address = mediaPath;
                            _MediaInfo.preview_address = previewPath;
                            _MediaInfo.id = id;
                            result= _mediaInfoService.UpdateEntity(_MediaInfo);
                        }
                    }
                }
                else//转为更新处理
                {
                    fileUpload.before_name = MediaInfo.media_name;
                    result = Update(fileUpload, fileType, ref retContent);
                }
            }
            catch(Exception ex)
            {
                retContent = fileUpload.file_name + ":" + ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 更新媒体
        /// </summary>
        /// <param name="fileUpload">T</param>
        /// <param name="retContent">异常反馈信息</param>
        /// <returns></returns>
        private bool Update(FileUpload fileUpload, FileType fileType, ref string retContent)
        {

            bool result = false;
            try
            {
                //查询数据是否存在
                MediaInfo MediaInfo = SelectSingle(fileUpload.media_group_id, fileUpload.before_name);
                if (MediaInfo != null)
                {
                    if (MediaInfo.media_type == fileType.ToString())
                    {
                        string previewPath = string.Empty;
                        string mediaPath = string.Empty;
                        //ffmpeg地址
                        string ffmpegPath = GlobalParameter._FFMPEG;
                        //获取文件路径
                        string filePath = fileUpload.file_path + "\\" + fileUpload.file_name;
                        //获取文件信息
                        MediaInfo _MediaInfo = FileHelper.GetMediaAttribute(filePath, ffmpegPath);
                        if (MediaInfo.media_width == _MediaInfo.media_width && MediaInfo.media_height == _MediaInfo.media_height && MediaInfo.media_size == _MediaInfo.media_size && MediaInfo.media_len == _MediaInfo.media_len)
                        {
                            retContent = fileUpload.file_name + ":文件信息一致";
                            //文件一模一样则无需替换
                        }
                        else
                        {
                            //将信息赋值给原先T
                            MediaInfo.media_width = _MediaInfo.media_width;
                            MediaInfo.media_height = _MediaInfo.media_height;
                            MediaInfo.media_size = _MediaInfo.media_size;
                            MediaInfo.media_len = _MediaInfo.media_len;
                            MediaInfo.media_name = fileUpload.file_name;
                            MediaInfo.media_type = fileType.ToString();
                            if (_mediaInfoService.UpdateEntity(MediaInfo))
                            {
                                //删除文件之前的文件
                                FileHelper.DelFile(MediaInfo.media_address);
                                FileHelper.DelFile(MediaInfo.preview_address);

                                result = FileProcess(filePath, MediaInfo.id, ref previewPath, ref mediaPath);
                                if (MediaInfo.media_address != mediaPath)
                                {
                                    MediaInfo.serial_number= Path.GetFileName(mediaPath);
                                    MediaInfo.preview_address = previewPath;
                                    MediaInfo.media_address = mediaPath;
                                    _mediaInfoService.UpdateEntity(MediaInfo);
                                }
                                result = true;
                            }
                            else
                            {
                                retContent = fileUpload.file_name + ":更新数据失败";
                            }
                        }
                    }
                    else
                    {
                        retContent = fileUpload.file_name + ":只能更新同类的文件";
                    }
                }
                else
                {
                    retContent = fileUpload.file_name + ":未找到需要更新的媒体";
                }
            }
            catch(Exception ex)
            {
                retContent = fileUpload.file_name + ":" + ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 文件处理(生成缩略图，拷贝文件)
        /// </summary>
        /// <param name="filePath">原文件地址</param>
        /// <param name="id">媒体id</param>
        /// <param name="isDeleteFile">是否需要删除原文件</param>
        private bool FileProcess(string filePath,int id,ref string previewPath,ref string mediaPath)
        {
            bool result = false;
            //ffmpeg地址
            string ffmpegPath = GlobalParameter._FFMPEG;
            //判断文件类型
            FileType fileType = FileHelper.CheckFileType(filePath);
            //缩略图地址
            previewPath = GlobalParameter._PREVIEW + "\\" + id.ToString(GlobalParameter._FORMATVALUE) + ".jpg";
            //获取文件后缀
            string extension = Path.GetExtension(filePath);
            //媒体地址
            mediaPath = GlobalParameter._MEDIA + "\\" + GetFolder(id) + "\\" + id.ToString(GlobalParameter._FORMATVALUE) + extension;
            //获取目录地址
            string strPath = Path.GetDirectoryName(mediaPath);
            //获取预览目录地址
            string strPreviePath = Path.GetDirectoryName(previewPath);
            //判断文件夹是否存在，如果不存在则创建
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            if (!Directory.Exists(strPreviePath))
            {
                Directory.CreateDirectory(strPreviePath);
            }
            //产生缩略图
            if(FileHelper.GenerateThumbnails(filePath, previewPath, ffmpegPath))
            {
                File.Copy(filePath, mediaPath);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 根据media_group_id，media_name查询数据
        /// </summary>
        /// <param name="media_group_id"></param>
        /// <param name="media_name"></param>
        /// <returns></returns>
        private MediaInfo SelectSingle(int media_group_id,string media_name)
        {
            MediaInfo mediaInfo = new MediaInfo();
            try
            {
                Expression<Func<MediaInfo, bool>> _expression = _expression = f => f.media_group_id == media_group_id && f.media_name == media_name;
                mediaInfo = _mediaInfoService.QueryableToEntity(_expression);
            }
            catch
            {
                mediaInfo = null;
            }
            return mediaInfo;
        }

        public string GetFolder(int id)
        {
          return  Math.Ceiling((double)id / 100).ToString("00000000");
        }
    }
}
