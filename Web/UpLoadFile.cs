﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System.Drawing;
#if NETCORE
using Microsoft.AspNetCore.Http;
#endif
using XiaoFeng.Config;
using System.Linq;
using XiaoFeng.IO;

namespace XiaoFeng.Web
{
    /// <summary>
    /// 上传文件操作类
    /// </summary>
    public class UpLoadFile : Disposable
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public UpLoadFile() { }

        #region 属性
        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; } = "";
        /// <summary>
        /// 上传配置
        /// </summary>
        private Upload Config { get { return Upload.Current; } }
        /// <summary>
        /// 最大上传文件大小 单位为B
        /// </summary>
        private int _MaxSize = 0;
        /// <summary>
        /// 最大上传文件大小 单位为B
        /// </summary>
        public int MaxSize { get { return _MaxSize; } set { this.Config.MaxLength = _MaxSize = value; } }
        /// <summary>
        /// 允许上传文件类型
        /// </summary>
        private string _FileType = "";
        /// <summary> 
        /// 允许上传文件类型
        /// </summary>
        public string FileType { get { return _FileType; } set { _FileType = value == null ? "" : value.Replace(",", "|"); } }
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        private Boolean _IsDelTempFile = true;
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        public Boolean IsDelTempFile { get { return _IsDelTempFile; } set { _IsDelTempFile = value; } }
        /// <summary>
        /// 是否打水印
        /// </summary>
        private Boolean _IsWater = false;
        /// <summary>
        /// 是否打水印
        /// </summary>
        public Boolean IsWater { get { return _IsWater; } set { _IsWater = value; } }
        /// <summary>
        /// 文字水印信息
        /// </summary>
        private string _WaterText = "";
        /// <summary>
        /// 文字水印信息
        /// </summary>
        public string WaterText { get { return _WaterText; } set { _WaterText = value; } }
        /// <summary>
        /// 图片水印路径
        /// </summary>
        private string _WaterImage = "";
        /// <summary>
        /// 图片水印路径
        /// </summary>
        public string WaterImage { get { return _WaterImage; } set { _WaterImage = value; } }
        /// <summary>
        /// 文字大小
        /// </summary>
        private int _FontWeight = 32;
        /// <summary>
        /// 文字大小
        /// </summary>
        public int FontWeigth { get { return _FontWeight; } set { _FontWeight = value; } }
        /// <summary>
        /// 文字颜色
        /// </summary>
        private string _FontColor = "#000000";
        /// <summary>
        /// 文字颜色
        /// </summary>
        public string FontColor { get { return _FontColor; } set { _FontColor = value; } }
        /// <summary>
        /// 文字字体
        /// </summary>
        private string _FontFamily = "Arial";
        /// <summary>
        /// 文字字体
        /// </summary>
        public string FontFamily { get { return _FontFamily; } set { _FontFamily = value; } }
        /// <summary>
        /// 是文字水印还是图片水平 true是文字水印 false是图片文印
        /// </summary>
        private Boolean _IsWaterTextOrImage = true;
        /// <summary>
        /// 是文字水印还是图片水平 true是文字水印 false是图片文印
        /// </summary>
        public Boolean IsWaterTextOrImage { get { return _IsWaterTextOrImage; } set { _IsWaterTextOrImage = value; } }
        /// <summary>
        /// 打水印位置 0:正中间 1:左上角 2:右上角 3:左下角 4:右下角 
        /// </summary>
        private int _TextAlign = 4;
        /// <summary>
        /// 打水印位置 0:正中间 1:左上角 2:右上角 3:左下角 4:右下角 
        /// </summary>
        public int TextAlign { get { return _TextAlign; } set { _TextAlign = value; } }
        /// <summary>
        /// 客户端上传文件集
        /// </summary>
#if NETFRAMEWORK
        private HttpFileCollection _HttpFiles = null;
#else
        private IFormFileCollection _HttpFiles = null;
#endif
        /// <summary>
        /// 客户端上传文件集
        /// </summary>
#if NETFRAMEWORK
        public HttpFileCollection HttpFiles { get { return _HttpFiles; } set { _HttpFiles = value; } }
#else
        public IFormFileCollection HttpFiles { get { return _HttpFiles; } set { _HttpFiles = value; } }
#endif
        /// <summary>
        /// 文件数量
        /// </summary>
        private int _FileCount = 0;
        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get { return _FileCount; } set { _FileCount = value; } }
        /// <summary>
        /// 上传信息
        /// </summary>
        private string[] _Message;
        /// <summary>
        /// 上传信息
        /// </summary>
        public string[] Message { get { return _Message; } set { _Message = value; } }
        /// <summary>
        /// 文件大小
        /// </summary>
        private long[] _FileSize;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long[] FileSize { get { return _FileSize; } set { _FileSize = value; } }
        /// <summary>
        /// 上传到服务器端的保存文件路径
        /// </summary>
        private string[] _SourcePath;
        /// <summary>
        /// 上传到服务器端的保存文件路径
        /// </summary>
        public string[] SourcePath { get { return _SourcePath; } set { _SourcePath = value; } }
        /// <summary>
        /// 上传到服务器端的相对文件路径
        /// </summary>
        private string[] _BasePath;
        /// <summary>
        /// 上传到服务器端的相对文件路径
        /// </summary>
        public string[] BasePath { get { return _BasePath; } set { _BasePath = value; } }
        /// <summary>
        /// 上传到服务器端打水印后的文件路径
        /// </summary>
        private string[] _WaterPath;
        /// <summary>
        /// 上传到服务器端打水印后的文件路径
        /// </summary>
        public string[] WaterPath { get { return this._WaterPath; } set { this._WaterPath = value; } }
        /// <summary>
        /// 上传到服务器端缩略图后的文件路径
        /// </summary>
        private string[] _ThumbnailPath;
        /// <summary>
        /// 上传到服务器端缩略图后的文件路径
        /// </summary>
        public string[] ThumbnailPath { get { return this._ThumbnailPath; } set { this._ThumbnailPath = value; } }
        /// <summary>
        /// 文件保存路径
        /// </summary>
        private string _SavePath = "";
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string SavePath { get { return _SavePath; } set { _SavePath = value; } }
        /// <summary>
        /// 路径格式
        /// </summary>
        private string _PathFormat = "yyyyMMdd";
        /// <summary>
        /// 路径格式
        /// </summary>
        public string PathFormat { get { return _PathFormat; } set { _PathFormat = value; } }
        /// <summary>
        /// 上传成功文件个数
        /// </summary>
        private int _SuccessFileCount = 0;
        /// <summary>
        /// 上传成功文件个数
        /// </summary>
        public int SuccessFileCount { get { return _SuccessFileCount; } set { _SuccessFileCount = value; } }
        /// <summary>
        /// 是否生成缩略图
        /// </summary>
        private Boolean _IsThumbnail = false;
        /// <summary>
        /// 是否生成缩略图
        /// </summary>
        public Boolean IsThumbnail { get { return _IsThumbnail; } set { _IsThumbnail = value; } }
        /// <summary>
        /// 缩略图宽度
        /// </summary>
        private int _TWidth = 100;
        /// <summary>
        /// 缩略图宽度
        /// </summary>
        public int TWidth { get { return _TWidth; } set { _TWidth = value; } }
        /// <summary>
        /// 缩略图高度
        /// </summary>
        private int _THeight = 100;
        /// <summary>
        /// 缩略图高度
        /// </summary>
        public int THeight { get { return _THeight; } set { _THeight = value; } }
        /// <summary>
        /// X坐标（zoom为1时）
        /// </summary>
        private int _TPosX = 0;
        /// <summary>
        /// X坐标（zoom为1时）
        /// </summary>
        public int TPosX { get { return _TPosX; } set { _TPosX = value; } }
        /// <summary>
        /// Y坐标（zoom为1时）
        /// </summary>
        private int _TPosY = 0;
        /// <summary>
        /// Y坐标（zoom为1时）
        /// </summary>
        public int TPosY { get { return _TPosY; } set { _TPosY = value; } }
        /// <summary>
        /// 生成缩略图模式 指定字符串"WH"指定宽高缩放（可能变形）,"W"指定宽缩放,"H"指定高缩放,"Cut"指定高宽裁减（不变形）,"EQ" 指定宽高等比例缩放(不变形)
        /// </summary>
        private string _TModeType = "WH";
        /// <summary>
        /// 生成缩略图模式 指定字符串"WH"指定宽高缩放（可能变形）,"W"指定宽缩放,"H"指定高缩放,"Cut"指定高宽裁减（不变形）,"EQ" 指定宽高等比例缩放(不变形)
        /// </summary>
        public string TModeType { get { return _TModeType; } set { _TModeType = value; } }
        /// <summary>
        /// 生成缩略图方式 0为直接生成 1为切割后生成
        /// </summary>
        private int _TMode = 0;
        /// <summary>
        /// 生成缩略图方式 0为直接生成 1为切割后生成
        /// </summary>
        public int TMode { get { return _TMode; } set { _TMode = value; } }
        /// <summary>
        /// 是否验证文件头类型
        /// </summary>
        private Boolean _IsValidateMIME = true;
        /// <summary>
        /// 是否验证文件头类型
        /// </summary>
        public Boolean IsValidateMIME { get { return this._IsValidateMIME; } set { this._IsValidateMIME = value; } }
        /// <summary>
        /// 文件名前缀
        /// </summary>
        private string _Prefix = "ZW_";
        /// <summary>
        /// 文件名前缀
        /// </summary>
        public string Prefix { get { return this._Prefix; } set { this._Prefix = value; } }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        #endregion

        #region 方法

        #region 上传入口
        /// <summary>
        /// 上传入口
        /// </summary>
        public void UpLoad()
        {
            this.HttpFiles = this.HttpFiles ?? HttpContext.Current.Request.
#if NETFRAMEWORK
             Files
#else
            Form.Files
#endif
             ;
            this.FileCount = this.HttpFiles.Count;
            this.Message = new string[this.FileCount];
            this.FileSize = new long[this.FileCount];
            this.SourcePath = new string[this.FileCount];
            this.BasePath = new string[this.FileCount];
            this.WaterPath = new string[this.FileCount];
            this.ThumbnailPath = new string[this.FileCount];
            if (!Config.IsOpen) { this.Message[0] = "暂未开放上传附件功能."; return; }
            if (this.FileCount == 0) { this.Message[0] = "请选择要上传的文件."; return; }
            UploadData uData = new UploadData();
            if (this.FileFormat.IsNullOrEmpty()) uData = Config.Default;
            else
                if (!Config.Data.TryGetValue(this.FileFormat, out uData))
                uData = Config.Default;
            if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
            uData.Ext = uData.Ext.ReplacePattern(",", "|");
            if (this.MaxSize > 0) Config.MaxLength = this.MaxSize;
            /*处理保存路径*/
            string RootPath = "";
            if (!this.SavePath.IsBasePath()) { this.SavePath = "/" + Config.UploadPath + "/" + this.SavePath + "/" + uData.Path; this.SavePath = this.SavePath.ReplacePattern("//", "/"); }
            if (!this.SavePath.IsNullOrEmpty() && !this.SavePath.IsBasePath())
            {
                RootPath = (FileHelper.GetRootStaticPath() + this.SavePath.Replace("\\", "/")).GetBasePath();
            }
            if (!this.PathFormat.IsNullOrEmpty())
            {
                this.SavePath = this.SavePath.TrimEnd('/') + @"/" + DateTime.Now.ToString(this.PathFormat) + @"/";
                RootPath += "/" + DateTime.Now.ToString(this.PathFormat).Replace("/", "\\") + @"/";
                RootPath = RootPath.GetBasePath();
            }
            FileHelper.Create(RootPath, FileAttribute.Directory);
            DateTime now = DateTime.Now;
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"yyyy",now.ToString("yyyy") },
                {"yy",now.Year.ToString() },
                {"MM",now.ToString("MM") },
                {"M",(now.Month+1).ToString() },
                {"dd",now.ToString("dd") },
                {"d",now.Day.ToString() },
                {"HH",now.ToString("HH") },
                {"H",now.Hour.ToString() },
                {"mm",now.ToString("mm") },
                {"m",now.Minute.ToString() },
                {"ss",now.ToString("ss") },
                {"s",now.Second.ToString() },
                {"ff",now.ToString("ff") },
                {"fff",now.ToString("fff") },
                {"ffff",now.ToString("ffff") },
                {"fffff",now.ToString("fffff") },
                {"ffffff",now.ToString("ffffff") }
            };
            for (int i = 0; i < this.HttpFiles.Count; i++)
            {
#if NETFRAMEWORK
                HttpPostedFile HttpFile = this.HttpFiles[i];
                this.FileSize[i] = HttpFile.ContentLength;
#else
                IFormFile HttpFile = this.HttpFiles[i];
                this.FileSize[i] = HttpFile.Length;
#endif

                if (this.FileSize[i] == 0 || HttpFile.FileName.IsNullOrEmpty()) continue;
                string FullFileName = Path.GetFileName(HttpFile.FileName);
                string fileType = Path.GetExtension(HttpFile.FileName).TrimStart('.');
                if (!fileType.IsMatch(@"^(" + uData.Ext + ")$")) { this.Message[i] = "UpLoadFailure:上传的文件扩展名暂不允许上传.<br/>[" + FullFileName + "]<br/>允许上传文件扩展名:" + uData.Ext.Replace("|", ",") + ""; continue; }
                if (this.IsValidateMIME && !IsMime(fileType, HttpFile.ContentType)) { this.Message[i] = "UpLoadFailure:上传的文件头类型不正确.<br/>[" + FullFileName + "],当前文件头为：" + HttpFile.ContentType; continue; }
                if (this.FileSize[i] > Config.MaxLength) { this.Message[i] = "UpLoadFailure:上传文件超过系统限定大小.<br/>[" + FullFileName + "]<br/>允许最大上传为：" + (Config.MaxLength / 1024).ToString() + "K"; continue; }
                string newFileName = "";
                if (!data.ContainsKey("ext"))
                    data.Add("ext", fileType);
                else data["ext"] = fileType;
                for (int r = 1; r < 10; r++)
                {
                    string _randNum = RandomHelper.GetRandomString(r, RandomType.Number);
                    if (data.ContainsKey("rnd" + r))
                        data["rnd" + r] = _randNum;
                    else
                        data.Add("rnd" + r, _randNum);
                }
                if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd4}.{ext}";
                if (this.FileCount == 1)
                {
                    newFileName = Config.FileNameFormat.format(data);
                    if (this.FileName.IsNotNullOrEmpty()) newFileName = this.FileName + "." + fileType;
                }
                else
                {
                    newFileName = Config.FileNameFormat.Replace(@"\{rnd\d+\}", (i + 1).ToString("D3")).format(data);
                }
                try
                {
                    /*判断有没有木马*/
                    if (Config.IsCheckTrojan && (fileType.ToLower() == "jpg" || fileType.ToLower() == "jpeg" || fileType.ToLower() == "bmp" || fileType.ToLower() == "gif" || fileType.ToLower() == "png"))
                    {

                        if (!this.IsMuMa(HttpFile.
#if NETFRAMEWORK
                            InputStream
#else
                            OpenReadStream()
#endif
                            ))
                        { this.Message[i] = "UpLoadFailure:上传的文件检测出疑似含有有木马."; continue; }
                    }
#if NETFRAMEWORK
                    HttpFile.SaveAs((RootPath + "/" + newFileName).GetBasePath());
#else
                    using (var fStream = new FileStream((RootPath + "/" + newFileName).GetBasePath(), FileMode.CreateNew))
                    {
                        HttpFile.CopyTo(fStream);
                        fStream.Flush();
                    }
#endif
                    this.Message[i] = "UpLoadSuccess.[" + FullFileName + "]";
                    this.SuccessFileCount++;
                    this.SourcePath[i] = RootPath + newFileName;
                    this.BasePath[i] = this.Config.GetDomainPath(this.SavePath + newFileName);
                    this.WaterPath[i] = this.SavePath + newFileName;
                    this.ThumbnailPath[i] = this.SavePath + newFileName;
                    if (this.IsWater)
                    {
                        Image.ImageWater water = new Image.ImageWater
                        {
                            WaterText = this.WaterText,
                            WaterImagePath = this.WaterImage.GetBasePath(),
                            FontSize = this.FontWeigth,
                            FontColor = ColorTranslator.FromHtml(this.FontColor),
                            FontFamily = this.FontFamily,
                            ImagePath = this.SourcePath[i],
                            NewImagePath = RootPath + "s" + newFileName,
                            IsDelTempFile = this.IsDelTempFile,
                            TextAlign = this.TextAlign
                        };
                        this.WaterPath[i] = this.SavePath + "s" + newFileName;
                        if (this.IsWaterTextOrImage)
                        {
                            if (!water.AddText())
                            {
                                this.Message[i] = "WaterFailure:" + water.ErrMessage + "<br/>[" + FullFileName + "]";
                            }
                        }
                        else
                        {
                            if (!water.AddImage())
                            {
                                this.Message[i] = "WaterFailure:" + water.ErrMessage + "<br/>[" + FullFileName + "]";
                            }
                        }
                    }
                    if (this.IsThumbnail)
                    {
                        this.ThumbnailPath[i] = this.SavePath + newFileName.Replace(".", "_s.");
                        Image.ImageThumbnail img = new Image.ImageThumbnail
                        {
                            SourcePath = this.SourcePath[i],
                            TargetPath = RootPath + newFileName.Replace(".", "_s."),
                            PosX = this.TPosX,
                            PosY = this.TPosY,
                            ModeType = this.TModeType,
                            Width = this.TWidth,
                            Height = this.THeight
                        };
                        if (this.TMode == 0)
                            img.MakeThumbnail();
                        else if (this.TMode == 1)
                            img.MakeMyThumbPhoto();
                    }
                    if ((this.IsWater || this.IsThumbnail) && this.IsDelTempFile && File.Exists(this.SourcePath[i])) File.Delete(this.SourcePath[i]);
                }
                catch (IOException ex)
                {
                    this.Message[i] = "UpLoadFailure:" + ex.Message + "<br/>[" + FullFileName + "]";
                }
            }
        }
        #endregion

        #region 上传入口
        /// <summary>
        /// 上传入口
        /// </summary>
        /// <param name="postedFile">输入文件</param>
        /// <param name="fileName">文件名</param>
        public void UpLoad(
#if NETFRAMEWORK
        HttpPostedFile
#else   
        IFormFile
#endif
           postedFile , string fileName = "")
        {
            if (!Config.IsOpen) { this.Message[0] = "暂未开放上传附件功能."; return; }
            int i = 0;
            if (postedFile == null) this.FileCount = 0;
            else this.FileCount = 1;
#if NETFRAMEWORK
            HttpPostedFile
#else
            IFormFile
#endif
            HttpFile = postedFile;
            UploadData uData = new UploadData();
            if (this.FileFormat.IsNullOrEmpty()) uData = Config.Default;
            else
                if (!Config.Data.TryGetValue(this.FileFormat, out uData))
                uData = Config.Default;
            if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
            uData.Ext = uData.Ext.ReplacePattern(",", "|");
            if (this.MaxSize > 0) Config.MaxLength = this.MaxSize;
            DateTime now = DateTime.Now;
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"yyyy",now.ToString("yyyy") },
                {"yy",now.Year.ToString() },
                {"MM",now.ToString("MM") },
                {"M",(now.Month+1).ToString() },
                {"dd",now.ToString("dd") },
                {"d",now.Day.ToString() },
                {"HH",now.ToString("HH") },
                {"H",now.Hour.ToString() },
                {"mm",now.ToString("mm") },
                {"m",now.Minute.ToString() },
                {"ss",now.ToString("ss") },
                {"s",now.Second.ToString() },
                {"ff",now.ToString("ff") },
                {"fff",now.ToString("fff") },
                {"ffff",now.ToString("ffff") },
                {"fffff",now.ToString("fffff") },
                {"ffffff",now.ToString("ffffff") }
            };
            this.Message = new string[this.FileCount];
            this.FileSize = new long[this.FileCount];
            this.SourcePath = new string[this.FileCount];
            this.BasePath = new string[this.FileCount];
            this.WaterPath = new string[this.FileCount];
            this.ThumbnailPath = new string[this.FileCount];
            this.FileSize[i] =
#if NETFRAMEWORK
            HttpFile.ContentLength;
#else
            HttpFile.Length;
#endif

            if (this.FileSize[i] == 0 || HttpFile.FileName.IsNullOrEmpty()) { this.Message[0] = "请选择要上传的文件."; return; }
            string FullFileName = Path.GetFileName(HttpFile.FileName);
            string fileType = Path.GetExtension(HttpFile.FileName).TrimStart('.');

            if (!data.ContainsKey("ext"))
                data.Add("ext", fileType);
            else data["ext"] = fileType;
            for (int r = 1; r < 10; r++)
            {
                string _randNum = RandomHelper.GetRandomString(r, RandomType.Number);
                if (data.ContainsKey("rnd" + r))
                    data["rnd" + r] = _randNum;
                else
                    data.Add("rnd" + r, _randNum);
            }
            if (!fileType.IsMatch(@"^(" + uData.Ext + ")$")) { this.Message[i] = "UpLoadFailure:上传的文件扩展名暂不允许上传.<br/>[" + FullFileName + "]<br/>允许上传文件扩展名:" + uData.Ext.Replace("|", ",") + ""; return; }
            if (this.IsValidateMIME && !IsMime(fileType, HttpFile.ContentType)) { this.Message[i] = "UpLoadFailure:上传的文件头类型不正确.<br/>[" + FullFileName + "],当前文件头为：" + HttpFile.ContentType; return; }
            if (this.FileSize[i] > Config.MaxLength) { this.Message[i] = "UpLoadFailure:上传文件超过系统限定大小.<br/>[" + FullFileName + "]<br/>允许最大上传为：" + (Config.MaxLength / 1024).ToString() + "K"; return; }
            /*处理保存路径*/
            string RootPath = "", newFileName = "", _SavePath = "";
            if (fileName.IsNotNullOrEmpty())
            {
                _SavePath = fileName.GetBasePath();
                var _path = Path.GetDirectoryName(fileName);
            }
            else
            {
                if (!this.SavePath.IsBasePath()) { this.SavePath = "/" + Config.UploadPath + "/" + this.SavePath + "/" + uData.Path; this.SavePath = this.SavePath.Replace("\\", "/"); }
                RootPath = (FileHelper.GetRootStaticPath() + this.SavePath.Replace(@"\", @"/")).GetBasePath();
                if (!this.PathFormat.IsNullOrEmpty())
                {
                    this.SavePath = this.SavePath.TrimEnd('/') + @"/" + DateTime.Now.ToString(this.PathFormat) + @"/";
                    RootPath += "/" + DateTime.Now.ToString(this.PathFormat).Replace("\\", "/") + @"\";
                    RootPath = RootPath.GetBasePath();
                }
                if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd4}.{ext}";
                if (this.FileCount == 1)
                {
                    newFileName = Config.FileNameFormat.format(data);
                    if (this.FileName.IsNotNullOrEmpty()) newFileName = this.FileName + "." + fileType;
                }
                else
                {
                    newFileName = Config.FileNameFormat.Replace(@"\{rnd\d+\}", (i + 1).ToString("D3")).format(data);
                }
                _SavePath = (RootPath + "/" + newFileName).GetBasePath();
            }
            try
            {
                /*判断有没有木马*/
                if (Config.IsCheckTrojan && (fileType.ToLower() == "jpg" || fileType.ToLower() == "jpeg" || fileType.ToLower() == "bmp" || fileType.ToLower() == "gif" || fileType.ToLower() == "png"))
                {
                    if (!this.IsMuMa(
#if NETFRAMEWORK
                    HttpFile.InputStream
#else
                    HttpFile.OpenReadStream()
#endif
                    ))
                    { this.Message[i] = "UpLoadFailure:上传的文件检测出疑似有木马."; return; }
                }
                FileHelper.Create(_SavePath.GetDirectoryName(), FileAttribute.Directory);
                FileHelper.Delete(_SavePath, FileAttribute.File);
#if NETFRAMEWORK
                HttpFile.SaveAs(_SavePath);
#else
                using (var fStream = new FileStream(_SavePath, FileMode.CreateNew))
                {
                    HttpFile.CopyTo(fStream);
                    fStream.Flush();
                }
#endif
                this.Message[i] = "UpLoadSuccess.[" + FullFileName + "]";
                this.SuccessFileCount++;
                this.SourcePath[i] = _SavePath;
                this.BasePath[i] = this.Config.GetDomainPath(this.SavePath + newFileName);

                this.WaterPath[i] = this.SavePath + newFileName;
                this.ThumbnailPath[i] = this.SavePath + newFileName;
                if (this.IsWater)
                {
                    Image.ImageWater water = new Image.ImageWater
                    {
                        WaterText = this.WaterText,
                        WaterImagePath = this.WaterImage.GetBasePath(),
                        FontSize = this.FontWeigth,
                        FontColor = ColorTranslator.FromHtml(this.FontColor),
                        FontFamily = this.FontFamily,
                        ImagePath = this.SourcePath[i],
                        NewImagePath = RootPath + "s" + newFileName,
                        IsDelTempFile = this.IsDelTempFile,
                        TextAlign = this.TextAlign
                    };
                    this.WaterPath[i] = this.SavePath + "s" + newFileName;
                    if (this.IsWaterTextOrImage)
                    {
                        if (!water.AddText())
                        {
                            this.Message[i] = "WaterFailure:" + water.ErrMessage + "<br/>[" + FullFileName + "]";
                        }
                    }
                    else
                    {
                        if (!water.AddImage())
                        {
                            this.Message[i] = "WaterFailure:" + water.ErrMessage + "<br/>[" + FullFileName + "]";
                        }
                    }
                }
                if (this.IsThumbnail)
                {
                    this.ThumbnailPath[i] = this.SavePath + newFileName.Replace(".", "_s.");
                    Image.ImageThumbnail img = new Image.ImageThumbnail
                    {
                        SourcePath = this.SourcePath[i],
                        TargetPath = RootPath + newFileName.Replace(".", "_s."),
                        PosX = this.TPosX,
                        PosY = this.TPosY,
                        ModeType = this.TModeType,
                        Width = this.TWidth,
                        Height = this.THeight
                    };
                    if (this.TMode == 0)
                        img.MakeThumbnail();
                    else if (this.TMode == 1)
                        img.MakeMyThumbPhoto();
                }
                if (this.IsDelTempFile && (this.IsWater || this.IsThumbnail) && File.Exists(this.SourcePath[i])) File.Delete(this.SourcePath[i]);
            }
            catch (Exception ex)
            {
                this.Message[i] = "UpLoadFailure:" + ex.Message + "<br/>[" + FullFileName + "]";
            }
        }
#endregion

#region Base64上传
        /// <summary>
        /// Base64上传
        /// </summary>
        /// <param name="base64File">附件base64</param>
        /// <param name="fileName">文件名</param>
        public void UpLoad(string base64File, string fileName = "")
        {
            if (!Config.IsOpen) { this.Message[0] = "暂未开放上传附件功能."; return; }
            if (base64File.IsNullOrEmpty() || base64File.RemovePattern(@"data:[^;]+;base64,").IsNullOrEmpty()) { this.Message[0] = "请选择要上传的文件."; return; }
            this.FileCount = 1;
            UploadData uData = new UploadData();
            if (this.FileFormat.IsNullOrEmpty()) uData = Config.Default;
            else
                if (!Config.Data.TryGetValue(this.FileFormat, out uData))
                uData = Config.Default;
            if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
            uData.Ext = uData.Ext.ReplacePattern(",", "|");
            if (this.MaxSize > 0) Config.MaxLength = this.MaxSize;

            var ImageType = base64File.GetMatch(@"data:([^;]+);base64,");
            base64File = base64File.RemovePattern(@"data:[^;]+;base64,");
            var fileType = "";
            if (ImageType.IsNullOrEmpty()) fileType = "png";
            else
            {
                var keys = ContentTypes.Data.Where(a => a.Value.IsMatch(ImageType));
                if (keys != null && keys.Any())
                {
                    fileType = keys.FirstOrDefault().Key;
                }
            }
            int i = 0;
            DateTime now = DateTime.Now;
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"yyyy",now.ToString("yyyy") },
                {"yy",now.Year.ToString() },
                {"MM",now.ToString("MM") },
                {"M",(now.Month+1).ToString() },
                {"dd",now.ToString("dd") },
                {"d",now.Day.ToString() },
                {"HH",now.ToString("HH") },
                {"H",now.Hour.ToString() },
                {"mm",now.ToString("mm") },
                {"m",now.Minute.ToString() },
                {"ss",now.ToString("ss") },
                {"s",now.Second.ToString() },
                {"ff",now.ToString("ff") },
                {"fff",now.ToString("fff") },
                {"ffff",now.ToString("ffff") },
                {"fffff",now.ToString("fffff") },
                {"ffffff",now.ToString("ffffff") }
            };
            this.Message = new string[this.FileCount];
            this.FileSize = new long[this.FileCount];
            this.SourcePath = new string[this.FileCount];
            this.BasePath = new string[this.FileCount];
            this.WaterPath = new string[this.FileCount];
            this.ThumbnailPath = new string[this.FileCount];

            if (!fileType.IsMatch(@"^(" + uData.Ext + ")$")) { this.Message[i] = "UpLoadFailure:上传的文件扩展名暂不允许上传.<br/>允许上传文件扩展名：" + uData.Ext.ReplacePattern("|", ",") + ""; return; }

            if (!data.ContainsKey("ext"))
                data.Add("ext", fileType);
            else data["ext"] = fileType;
            for (int r = 1; r < 10; r++)
            {
                string _randNum = RandomHelper.GetRandomString(r, RandomType.Number);
                if (data.ContainsKey("rnd" + r))
                    data["rnd" + r] = _randNum;
                else
                    data.Add("rnd" + r, _randNum);
            }
            var file = base64File.FromBase64StringToBytes();
            this.FileSize[i] = file.Length;
            if (this.FileSize[i] > Config.MaxLength) { this.Message[i] = "UpLoadFailure:上传文件超过系统限定大小.<br/>允许最大上传为：" + (Config.MaxLength / 1024).ToString() + "K"; return; }
            /*处理保存路径*/
            string RootPath = "", newFileName = "", _SavePath = "";
            if (fileName.IsNotNullOrEmpty())
            {
                _SavePath = fileName.GetBasePath();
                var _path = Path.GetDirectoryName(fileName);
            }
            else
            {
                if (!this.SavePath.IsBasePath()) { this.SavePath = "/" + Config.UploadPath + "/" + (this.SavePath + (this.SavePath.IsNullOrEmpty() ? "" : "/")) + uData.Path;  }
                RootPath = (FileHelper.GetRootStaticPath() + this.SavePath).GetBasePath();
                if (!this.PathFormat.IsNullOrEmpty())
                {
                    this.SavePath = this.SavePath.TrimEnd('/') + @"/" + DateTime.Now.ToString(this.PathFormat) + @"/";
                    RootPath += "/" + DateTime.Now.ToString(this.PathFormat);
                    RootPath = RootPath.GetBasePath();
                }
                if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd4}.{ext}";
                if (this.FileCount == 1)
                {
                    newFileName = Config.FileNameFormat.format(data);
                    if (this.FileName.IsNotNullOrEmpty()) newFileName = this.FileName + "." + fileType;
                }
                else
                {
                    newFileName = Config.FileNameFormat.Replace(@"\{rnd\d+\}", (i + 1).ToString("D3")).format(data);
                }
                _SavePath = (RootPath + "/" + newFileName).GetBasePath();
            }
            try
            {
                /*判断有没有木马*/
                if (Config.IsCheckTrojan && (fileType.ToLower() == "jpg" || fileType.ToLower() == "jpeg" || fileType.ToLower() == "bmp" || fileType.ToLower() == "gif" || fileType.ToLower() == "png") && !this.IsMuMa(file.GetString()))
                {
                    this.Message[i] = "UpLoadFailure:上传的文件检测出疑似有木马."; return;
                }
                FileHelper.Create(_SavePath.GetDirectoryName(), FileAttribute.Directory);
                FileHelper.Delete(_SavePath, FileAttribute.File);
                using (var fStream = new FileStream(_SavePath, FileMode.CreateNew))
                {
                    using (MemoryStream ms = new MemoryStream(file))
                    {
                        ms.CopyTo(fStream);
                        fStream.Flush();
                    }
                }
                
                this.Message[i] = "UpLoadSuccess.";
                this.SuccessFileCount++;
                this.SourcePath[i] = _SavePath;
                this.BasePath[i] = this.Config.GetDomainPath(this.SavePath + newFileName);

                this.WaterPath[i] = this.SavePath + newFileName;
                this.ThumbnailPath[i] = this.SavePath + newFileName;
                if (this.IsWater)
                {
                    Image.ImageWater water = new Image.ImageWater
                    {
                        WaterText = this.WaterText,
                        WaterImagePath = this.WaterImage.GetBasePath(),
                        FontSize = this.FontWeigth,
                        FontColor = ColorTranslator.FromHtml(this.FontColor),
                        FontFamily = this.FontFamily,
                        ImagePath = this.SourcePath[i],
                        NewImagePath = RootPath + "s" + newFileName,
                        IsDelTempFile = this.IsDelTempFile,
                        TextAlign = this.TextAlign
                    };
                    this.WaterPath[i] = this.SavePath + "s" + newFileName;
                    if (this.IsWaterTextOrImage)
                    {
                        if (!water.AddText())
                        {
                            this.Message[i] = "WaterFailure:" + water.ErrMessage;
                        }
                    }
                    else
                    {
                        if (!water.AddImage())
                        {
                            this.Message[i] = "WaterFailure:" + water.ErrMessage;
                        }
                    }
                }
                if (this.IsThumbnail)
                {
                    this.ThumbnailPath[i] = this.SavePath + newFileName.Replace(".", "_s.");
                    Image.ImageThumbnail img = new Image.ImageThumbnail
                    {
                        SourcePath = this.SourcePath[i],
                        TargetPath = RootPath + newFileName.Replace(".", "_s."),
                        PosX = this.TPosX,
                        PosY = this.TPosY,
                        ModeType = this.TModeType,
                        Width = this.TWidth,
                        Height = this.THeight
                    };
                    if (this.TMode == 0)
                        img.MakeThumbnail();
                    else if (this.TMode == 1)
                        img.MakeMyThumbPhoto();
                }
                if (this.IsDelTempFile && (this.IsWater || this.IsThumbnail) && File.Exists(this.SourcePath[i])) File.Delete(this.SourcePath[i]);
            }
            catch (Exception ex)
            {
                this.Message[i] = "UpLoadFailure:" + ex.Message;
            }
        }
#endregion

#region 是否含有木马
        /// <summary>
        /// 是否含有木马
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns></returns>
        public Boolean IsMuMa(Stream stream)
        {
            byte[] fileByte = new byte[stream.Length];
            stream.Read(fileByte, 0, int.Parse(stream.Length.ToString()));
            string fileContent = Encoding.Default.GetString(fileByte);
            return this.IsMuMa(fileContent);
        }
        /// <summary>
        /// 是否有木马
        /// </summary>
        /// <param name="fileContent">文件内容</param>
        /// <returns></returns>
        public Boolean IsMuMa(string fileContent)
        {
            if (fileContent.IsNullOrEmpty()) return false;
            string MuMa = Config.TrojanFeature.IsNullOrEmpty() ? @"request[\[\(\s]|eval|alert|document\.|response.|system|excute|redirect|delete |create |exec |select |update |\.asp|\.php|\.aspx|\.ashx|\.jsp|\.asmx|\.js| table | file|for\(|foreach|drop |alter |dbo\.|sys\.|<script|\.getfolder|\.createfolder|\.deletefolder|\.createdirectory|\.deletedirectory|\.saveas|wscript\.shell|script\.encode|server\.|\.createobject|execute|activexobject|language=|echo|<\?php" : Config.TrojanFeature;
            var _content = fileContent.ToLower();
            foreach (var m in MuMa.RemovePattern(@"\\").Split(new char[] { ',','|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (m.IsNullOrEmpty()) continue;
                if (_content.IndexOfX(m.ToLower()) > -1) return false;
            }
            return true;
            //return fileContent.IsMatch(@"[\r\t\n\s]*(" + MuMa + ")");
        }
#endregion

#region 验证文件MIME
        /// <summary>
        /// 验证文件MIME
        /// </summary>
        /// <param name="fType">当前文件类型</param>
        /// <param name="fMime">当前文件MIME</param>
        /// <returns></returns>
        public bool IsMime(string fType, string fMime)
        {
            if (fType.IsNullOrEmpty() || fMime.IsNullOrEmpty()) return false;
            fMime = "," + fMime.ToLower() + ",";
            string myMIME = "," + ContentTypes.get(fType) + ",";
            return myMIME != ",," && myMIME.IndexOf(fMime, StringComparison.OrdinalIgnoreCase) > -1;
        }
#endregion

#endregion
    }
}