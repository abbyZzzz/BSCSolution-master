using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using PdfiumViewer;
using Advantech.Entity;
using System.Text.RegularExpressions;
using Spire.Presentation;
using Advantech.CommonDefine;

namespace Currency
{
    public class FileHelper
    {
        static MediaInfo objMediaInfo = null;

        /// <summary>
        /// 媒体产生缩略图
        /// </summary>
        /// <param name="filePath">原文件地址</param>
        /// <param name="previewPath">缩略图地址</param>
        /// <param name="ffmpegPath">ffmpegPath地址</param>
        /// <returns></returns>
        public static bool GenerateThumbnails(string filePath, string previewPath, string ffmpegPath=null)
        {
            bool result = true;
            if (!File.Exists(previewPath))
            {
                FileType fileType = CheckFileType(filePath);
                switch(fileType.ToString())
                {
                    case "image":
                        result = GetThumbnailImages(filePath, previewPath);
                        break;
                    case "ppt":
                        result = GetThumbnailPpt(filePath, previewPath);
                        break;
                    case "pdf":
                        result = GetThumbnailPdf(filePath, previewPath);
                        break;
                    case "video":
                        if(string.IsNullOrEmpty(ffmpegPath)||!File.Exists(ffmpegPath))
                        {
                            result = false;
                        }
                        else
                        {
                            result = GetThumbnailVideo(filePath, previewPath, ffmpegPath);
                        }
                        break;
                    case "audio":
                    case "html":
                        result = GetThumbnailWritten(fileType.ToString(), previewPath);
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 获取文件的页面（播放时长）,文件大小，宽，高
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="ffmpegPath">ffmpeg地址</param>
        /// <returns></returns>
        public static MediaInfo GetMediaAttribute(string filePath, string ffmpegPath = null)
        {
            objMediaInfo = new MediaInfo();
            if (File.Exists(filePath))
            {
                FileType fileType = CheckFileType(filePath);
                switch (fileType.ToString())
                {
                    case "image":
                        objMediaInfo = GetMediaAttributeImage(filePath);
                        break;
                    case "ppt":
                        objMediaInfo = GetMediaAttributePpt(filePath);
                        break;
                    case "pdf":
                        objMediaInfo = GetMediaAttributePdf(filePath);
                        break;
                    case "video":
                        if (!File.Exists(ffmpegPath))
                        {
                            objMediaInfo = null;
                        }
                        else
                        {
                            objMediaInfo = GetMediaAttributeVideo(filePath, ffmpegPath);
                        }
                        break;
                    case "audio":
                    case "html":
                        objMediaInfo.media_size = CheckFileSize(filePath);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                objMediaInfo = null;
            }
            return objMediaInfo;
        }

        /// <summary>
        /// 根据文件判断文件大类
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static FileType CheckFileType(string path)
        {
            FileType fileType = new FileType();
            fileType = FileType.nosupport;//设置默认值
            string suffixName = Path.GetExtension(path);
            if (CheckSuffix(suffixName))
            {
                int bx = GetHeaderFileInfo(path);
                fileType = CheckHeaderFile(bx);
            }
            return fileType;
        }

        /// <summary>
        /// 获取文件的大小（kb）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static int CheckFileSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            decimal size = fileInfo.Length / 1024;
            return (int)Math.Round(size);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool DelFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return true;
        }

        /// <summary>
        /// 组合图片并产生预览图
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="filePath">文件地址</param>
        /// <param name="previewPath">预览图地址</param>
        /// <param name="imgs">文件地址</param>
        /// <param name="programRegions">区块参数</param>
        /// <returns></returns>
        public static bool CompositePreview(int width, int height, string filePath, string previewPath, List<string> imgs, List<ProgramRegion> programRegions)
        {
            bool result = false;
            // 初始化画布(最终的拼图画布)并设置宽高
            Bitmap bitMap = new Bitmap(width, height);
            // 初始化画板
            Graphics g1 = Graphics.FromImage(bitMap);
            // 将画布涂为白色(底部颜色可自行设置)
            g1.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
            for (int i = 0; i < imgs.Count; i++)
            {
                Image img = Image.FromFile(imgs[i]);
                Bitmap map = new Bitmap(img);
                g1.DrawImage(map, programRegions[i].x, programRegions[i].y, programRegions[i].w, programRegions[i].h);
                map.Dispose();
                img.Dispose();
            }
            Image images = bitMap;
            //保存
            images.Save(filePath);
            if (File.Exists(filePath))
            {
                if (GetThumbnailImages(filePath, previewPath))
                {
                    result = true;
                }
            }
            return result;
        }

        #region private

        /// <summary>
        /// 获取Pdf文件的页数，文件大小
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        private static MediaInfo GetMediaAttributePdf(string filePath)
        {
            objMediaInfo.media_size = CheckFileSize(filePath);
            var pdf = PdfDocument.Load(filePath);
            objMediaInfo.media_len= pdf.PageCount;
            pdf.Dispose();
            return objMediaInfo;
        }

        /// <summary>
        /// 获取PPT类文件的页数，文件大小
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        private static MediaInfo GetMediaAttributePpt(string filePath)
        {

            objMediaInfo.media_size = CheckFileSize(filePath);
            Presentation ppt = new Presentation();

            ppt.LoadFromFile(filePath);
            objMediaInfo.media_len = ppt.Slides.Count;
            ppt.Dispose();

            return objMediaInfo;
        }

        /// <summary>
        /// 获取图片类文件大小，宽，高
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        private static MediaInfo GetMediaAttributeImage(string filePath)
        {
            objMediaInfo.media_size = CheckFileSize(filePath);
            Bitmap pic = new Bitmap(filePath);
            objMediaInfo.media_width = pic.Size.Width;
            objMediaInfo.media_height = pic.Size.Height;
            pic.Dispose();
            return objMediaInfo;

        }

        /// <summary>
        /// 获取视频类文件的播放时长，大小，宽，高
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="ffmpegPath">ffmpeg地址</param>
        /// <returns></returns>
        private static MediaInfo GetMediaAttributeVideo(string filePath, string ffmpegPath)
        {
            objMediaInfo.media_size = CheckFileSize(filePath);

            Process p = new Process();//建立外部调用线程
            p.StartInfo.FileName = ffmpegPath;//要调用外部程序的绝对路径
            p.StartInfo.Arguments = "-i " + filePath;//参数(这里就是FFMPEG的参数了)
            p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;//不创建进程窗口
            p.ErrorDataReceived += new DataReceivedEventHandler(FfmpegOutput);//外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
            p.Start();//启动线程
            p.BeginErrorReadLine();//开始异步读取
            p.WaitForExit();//阻塞等待进程结束
            p.Close();//关闭进程
            p.Dispose();//释放资源
            return objMediaInfo;
        }
        private static void FfmpegOutput(object sendProcess, DataReceivedEventArgs output)
        {
            string data = output.Data;
            if (!string.IsNullOrEmpty(data))
            {
                if (data.Contains("Duration"))
                {
                    objMediaInfo.media_len = GetFfmpegTime(data);
                }
                if (data.Contains("Stream"))
                {
                    if (data.Contains("Video"))
                    {
                        int media_height = 0;
                        objMediaInfo.media_width = GetFfmpegWH(data, ref media_height);
                        objMediaInfo.media_height = media_height;
                    }
                }
            }
        }
        /// <summary>
        /// 截取ffmpeg输出的视频播放时长
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int GetFfmpegTime(string str)
        {
            int mlen = 0;
            try
            {
                string time = string.Empty;
                Regex reg = new Regex(@"\d{2}:\d{2}:\d{2}");
                Match m = reg.Match(str);
                while (m.Success)
                {
                    time = m.Result("$0");
                    break;
                }
                if (!string.IsNullOrEmpty(time))
                {
                    mlen = (int)(Convert.ToDateTime(time) - Convert.ToDateTime("00:00:00")).TotalSeconds;
                }
            }
            catch
            {
                mlen = 0;
            }

            return mlen;
        }
        /// <summary>
        /// 截取ffmpeg输出的视频宽度,高度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int GetFfmpegWH(string str, ref int mh)
        {
            int mw = 0;
            mh = 0;
            string widthHeight = string.Empty;
            try
            {
                Regex reg = new Regex(@"\d{1,4}x\d{1,4}");
                Match m = reg.Match(str);
                while (m.Success)
                {
                    widthHeight = m.Result("$0");
                    break;
                }

                if (!string.IsNullOrEmpty(widthHeight))
                {
                    List<string> objList = widthHeight.Split('x').ToList();
                    if (objList.Count == 2)
                    {
                        mw = Convert.ToInt32(objList[0]);
                        mh = Convert.ToInt32(objList[1]);
                    }
                }
            }
            catch
            {
                mw = 0;
                mh = 0;
            }
            return mw;

        }

        /// <summary>
        /// 视频通过ffmpeg产生预览图
        /// </summary>
        /// <param name="filePath">原视频地址</param>
        /// <param name="previewPath">缩略图地址</param>
        /// <param name="ffmpegPath">ffmpeg路径</param>
        /// <returns></returns>
        private static bool GetThumbnailVideo(string filePath, string previewPath,string ffmpegPath)
        {
            bool result = true;
            try
            {
                if ((!File.Exists(ffmpegPath)) || (!File.Exists(filePath)))
                {
                    result = false;
                }
                else
                {
                    //调用ffmpeg产生缩略图
                    string flvImgSize = "256x144";
                    Process p = new Process();//建立外部调用线程
                    p.StartInfo.FileName = ffmpegPath;//要调用外部程序的绝对路径
                    p.StartInfo.Arguments = " -i " + filePath + "  -y -f image2 -t 0.1 -s " + flvImgSize + " " + previewPath;
                    p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = false;//不创建进程窗口
                    p.Start();//启动线程
                    p.WaitForExit();//阻塞等待进程结束
                    p.Close();//关闭进程
                    p.Dispose();//释放资源
                    if (!File.Exists(previewPath))
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }


        private static bool GetThumbnailWritten(string written,string previewPath)
        {
            Bitmap bmp = new Bitmap(255, 144);      //定义画布大小
            Graphics g = Graphics.FromImage(bmp);      //封装一个GDI+绘图图面
            Random r = new Random();
            g.Clear(ColorTranslator.FromHtml("#000000"));  //背景色为白色

            g.DrawString(written, new System.Drawing.Font("宋体", 20, FontStyle.Bold), Brushes.White, 100, 70);
            bmp.Save(previewPath, ImageFormat.Jpeg);    //保存文件
            bmp.Dispose();
            g.Dispose();
            if (File.Exists(previewPath))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 图片产生缩略图
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="previewPath"></param>
        /// <returns></returns>
        private static bool GetThumbnailImages(string filePath, string previewPath)
        {
            try
            {
                Image image = Image.FromFile(filePath);
                Image thumb = image.GetThumbnailImage(256, 144, () => false, IntPtr.Zero);
                thumb.Save(previewPath);
                thumb.Dispose();
                image.Dispose();
                if (File.Exists(previewPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// PPT产生缩略图
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="previewPath"></param>
        /// <returns></returns>
        private static bool GetThumbnailPpt(string filePath, string previewPath)
        {
            bool result = false;
            Presentation ppt = new Presentation();
            try
            {
                ppt.LoadFromFile(filePath);
                if (ppt.Slides.Count > 0)
                {

                    //生成图片
                    foreach (IShape s in ppt.Slides[0].Shapes)
                    {
                        if (s is SlidePicture)
                        {
                            SlidePicture ps = s as SlidePicture;
                            ps.PictureFill.Picture.EmbedImage.Image.Save(previewPath);
                            if (File.Exists(previewPath))
                            {
                                result = true;
                            }
                            ps.Dispose();
                        }
                        if (s is PictureShape)
                        {
                            PictureShape ps = s as PictureShape;
                            ps.EmbedImage.Image.Save(previewPath);
                            if (File.Exists(previewPath))
                            {
                                result = true;
                            }
                        }
                    }

                    if(!result)
                    {
                        string temImage = Path.GetDirectoryName(previewPath)+"\\"+ Guid.NewGuid().ToString() + ".emf";
                        ppt.Slides[0].SaveAsEMF(temImage);
                        Image image = Bitmap.FromFile(temImage);
                        image.Save(previewPath);
                        image.Dispose();
                        File.Delete(temImage);
                        if (File.Exists(previewPath))
                        {
                            result = true;
                        }
                    }

                    if(!result)
                    {
                        result= GetThumbnailWritten("PPT", previewPath);
                    }
                }
                ppt.Dispose();
            }
            catch
            {
                ppt.Dispose();
                result = GetThumbnailWritten("PPT", previewPath);
            }


            return result;
        }

        private static bool GetThumbnailPdf(string filePath, string previewPath)
        {
            bool result = true;
            try
            {
                var pdf = PdfDocument.Load(filePath);
                var pdfpage = pdf.PageCount;
                if (pdfpage > 0)
                {
                    Size size = new Size();
                    size.Width = 255;
                    size.Height = 144;
                    RenderPage(filePath, 1, size, previewPath);
                    if (!File.Exists(previewPath))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
                pdf.Dispose();
            }
            catch
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 将PDF转换为图片
        /// </summary>
        /// <param name="pdfPath">pdf文件位置</param>
        /// <param name="pageNumber">pdf文件张数</param>
        /// <param name="size">pdf文件尺寸</param>
        /// <param name="outputPath">输出图片位置与名称</param>
        private static void RenderPage(string pdfPath, int pageNumber,Size size, string outputPath, int dpi = 300)
        {
            using (var document = PdfDocument.Load(pdfPath))
            using (var stream = new FileStream(outputPath, FileMode.Create))
            using (var image = GetPageImage(pageNumber, size, document, dpi))
            {
                image.Save(stream, ImageFormat.Jpeg);
                document.Dispose();
                stream.Close();
                stream.Dispose();
                image.Dispose();
            }
        }
        private static Image GetPageImage(int pageNumber, Size size, PdfDocument document, int dpi)
        {
            return document.Render(pageNumber - 1, size.Width, size.Height, dpi, dpi, PdfRenderFlags.Annotations);
        }
        /// <summary>
        /// 根据头文件信息，返回文件大类
        /// </summary>
        /// <param name="bx">头文件信息</param>
        /// <returns></returns>
        public static FileType CheckHeaderFile(int bx)
        {
            FileType fileType = FileType.nosupport;
            switch (bx)
            {
                case (int)FileExtension.JPG:
                case (int)FileExtension.GIF:
                case (int)FileExtension.PNG:
                case (int)FileExtension.BMP:
                    fileType = FileType.image;
                    break;
                case (int)FileExtension.VIDEO:
                case (int)FileExtension.WMV:
                    fileType = FileType.video;
                    break;
                case (int)FileExtension.PPT:
                case (int)FileExtension.PPTX:
                    fileType = FileType.ppt;
                    break;
                case (int)FileExtension.PDF:
                    fileType = FileType.pdf;
                    break;
                case (int)FileExtension.MP3:
                    fileType = FileType.audio;
                    break;
                case (int)FileExtension.HTML:
                    fileType = FileType.html;
                    break;
                default:
                    fileType = FileType.nosupport;
                    break;
            }
            return fileType;
        }

        /// <summary>
        /// 验证后缀名是否在支持的返回之内
        /// </summary>
        /// <param name="suffixName"></param>
        /// <returns></returns>
        private static bool CheckSuffix(string suffixName)
        {
            string[] fileSuffix = new string[] { ".jpg", ".jpeg", ".bmp",".png",".gif",".mp4",".avi",".mov",".wmv",".fly",".ppt",".pptx",".pdf",".mp3",".html",".htm" };
            return fileSuffix.ToList().Contains(suffixName.ToLower());
        }

        /// <summary>
        /// 获取头文件信息
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>头文件信息</returns>
        private static int GetHeaderFileInfo(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);
            string bx = string.Empty;
            byte buffer;
            try
            {
                buffer = r.ReadByte();
                bx = buffer.ToString();
                buffer = r.ReadByte();
                bx += buffer.ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            r.Close();
            fs.Close();

            return int.Parse(bx);
        }

        

        #endregion private
    }
}
