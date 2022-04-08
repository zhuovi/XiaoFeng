using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using XiaoFeng.IO;
using XiaoFeng.Config;
using System.IO;

#if NETFRAMEWORK
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif
/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021-02-02 下午 08:30:56          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Web
{
    /// <summary>
    /// 上传一个文件事件
    /// </summary>
    /// <param name="message">文件信息</param>
    /// <param name="args">消息</param>
    public delegate void UploadMessageEventHandler(FileMessage message, EventArgs args);
    /// <summary>
    /// 上传完成委托
    /// </summary>
    /// <param name="list">列表</param>
    /// <param name="args">消息</param>
    public delegate void UploadCompleteEventHandler(ConcurrentBag<FileMessage> list, EventArgs args);
    /// <summary>
    /// 类说明
    /// Version : 1.0.0
    /// CrateTime : 2021-02-02 下午 08:30:56
    /// Author : Jacky
    /// 更新说明
    /// </summary>
    public class UploadFiles : EntityBase, IUploadFiles
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public UploadFiles()
        {
#if NETFRAMEWORK
            this.HttpFiles = HttpContext.Current.Request.Files;
#else
            this.HttpFiles = HttpContext.Current.Request.Form.Files;
#endif
            this.Config = XiaoFeng.Config.Upload.Current;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 上传配置
        /// </summary>
        public Upload Config { get; set; }
        /// <summary>
        /// 成功列表
        /// </summary>
        public IEnumerable<FileMessage> SuccessList { get => this.Result.Where(a => a.State); }
        /// <summary>
        /// 失败列表
        /// </summary>
        public IEnumerable<FileMessage> ErrorList { get => this.Result.Where(a => !a.State); }
        /// <summary>
        /// 结果
        /// </summary>
        public ConcurrentBag<FileMessage> Result { get; set; } = new ConcurrentBag<FileMessage>();
        /// <summary>
        /// 是否验证文件类型
        /// </summary>
        public Boolean? IsValidateMime { get; set; }
        /// <summary>
        /// 上传文件集合
        /// </summary>
#if NETFRAMEWORK
        public HttpFileCollection HttpFiles { get; set; }
#else
        public IFormFileCollection HttpFiles { get; set; }
#endif
        /// <summary>
        /// 取消标识
        /// </summary>
        public CancellationTokenSource Token { get; set; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 允许上传文件类型 多个用,隔开
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 路径格式 日期格式 yyyy MM dd HH mm ss ffff
        /// </summary>
        public string PathFormat { get; set; } = "yyyyMMdd";
        /// <summary>
        /// 是否是日期路径
        /// </summary>
        public Boolean IsDatePath { get; set; } = true;
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 上传完一个文件事件
        /// </summary>
        public UploadMessageEventHandler OnMessage { get; set; }
        /// <summary>
        /// 上传完所有文件事件
        /// </summary>
        public UploadCompleteEventHandler OnComplete { get; set; }
        /// <summary>
        /// 锁
        /// </summary>
        public static readonly Object StreamLock = new Object();
        #endregion

        #region 方法

        #region 上传
        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        public void Upload()
        {
            if (this.HttpFiles.Count == 0)
            {
                var msg = new FileMessage { Message = "请选择要上传的文件.", State = false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
                return;
            }
            if (!this.Config.IsOpen)
            {
                var msg = new FileMessage { Message = "暂未开放上传附件功能.", State = false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
                return;
            }
            List<Task> tasks = new List<Task>();
#if NETFRAMEWORK
            this.HttpFiles.Each<HttpPostedFile>(f =>
#else
            this.HttpFiles.Each<IFormFile>(f =>
#endif
            {
                //tasks.Add(Task.Factory.StartNew(fe =>
                {
                    var msg = new FileMessage();
#if NETFRAMEWORK
                    var file = (HttpPostedFile)f;
                    msg.Length = file.ContentLength;
                    msg.Name = file.FileName.GetFileName();
#else
                    var file = (IFormFile)f;
                    msg.Length = file.Length;
                    msg.Name = file.FileName;
#endif
                    msg.LocalPath = file.FileName;
                    msg.Extension = msg.Name.GetExtension();
                    /*判断文件大小*/
                    if (msg.Length > this.Config.MaxLength)
                    {
                        msg.State = false;
                        msg.Message = "上传文件超过系统限定大小.<br/>[" + msg.LocalPath + "]<br/>允许最大上传为:" + FileHelper.ConvertByte(this.Config.MaxLength);
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    /*判断文件后缀名*/
                    var uData = new UploadData();
                    if (this.Format.IsNullOrEmpty()) { uData = Config.Default; }
                    else
                        if (!Config.Data.TryGetValue(this.Format, out uData))
                        uData = Config.Default;
                    if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
                    uData.Ext = uData.Ext.ReplacePattern(",", "|");
                    if (msg.Extension.IsNotNullOrEmpty() && !msg.Extension.Trim('.').IsMatch(@"^(" + uData.Ext + ")$"))
                    {
                        msg.State = false;
                        msg.Message = "上传的文件扩展名暂不允许上传.<br/>[" + msg.LocalPath + "]<br/>允许上传文件扩展名:" + uData.Ext.Replace("|", ",");
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    if (msg.Extension.IsNullOrEmpty()) msg.Extension = ".png";
                    /*验证文件Mime信息*/
                    if (this.IsValidateMime.HasValue && this.IsValidateMime.Value && !this.IsMime(msg.Extension, file.ContentType))
                    {
                        msg.State = false;
                        msg.Message = "上传的文件头类型不正确.<br/>当前文件[" + msg.LocalPath + "]头为:" + file.ContentType;
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    /*判断是否有木马特征*/
                    MemoryStream stream = new MemoryStream();
#if NETFRAMEWORK
                    file.InputStream.CopyTo(stream);
#else
                        lock (StreamLock)
                        {
                            var _stream = file.OpenReadStream();
                            _stream.CopyTo(stream);
                            _stream.Close();
                        }
#endif

                    if (this.Config.IsCheckTrojan && msg.Length < 10 * 1024 * 1024 && IsMuMa(stream))
                    {
                        msg.State = false;
                        msg.Message = "上传的文件检测出疑似含有有木马.";
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    /*构建文件路径*/
                    var RemotePath = FileHelper.AltDirectorySeparatorChar + this.Config.UploadPath + FileHelper.AltDirectorySeparatorChar + uData.Path;
                    var SavePath = FileHelper.GetRootStaticPath() + FileHelper.DirectorySeparatorChar + this.Config.UploadPath + FileHelper.DirectorySeparatorChar + uData.Path;
                    if (this.PathFormat.IsNotNullOrEmpty())
                    {
                        var pathFormat = this.IsDatePath ? DateTime.Now.ToString(this.PathFormat) : this.PathFormat;
                        SavePath += FileHelper.DirectorySeparatorChar + pathFormat;
                        RemotePath += FileHelper.AltDirectorySeparatorChar + pathFormat;
                    }
                    SavePath = SavePath.GetBasePath();
                    FileHelper.Create(SavePath, FileAttribute.Directory);
                    if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd:4}.{ext}";
                    var FileName = this.FileName;
                    if (FileName.IsNullOrEmpty())
                    {
                        FileName = Config.FileNameFormat.RemovePattern(@"\.{ext}");
                        if (FileName.IsMatch(@"\{rnd\:?(\d+)\}"))
                        {
                            var num = FileName.GetMatch(@"\{rnd\:?(\d+)\}").ToCast<int>();
                            var rnd = RandomHelper.GetRandomString(num, RandomType.Number);
                            FileName = FileName.ReplacePattern(@"\{rnd\:?(\d+)\}", rnd);
                        }
                        FileName = DateTime.Now.ToString(FileName).ReplacePattern(@"\{(\d+)\}", "$1") + msg.Extension;
                    }
                    else FileName += "." + msg.Extension.Trim('.');
                    SavePath += FileHelper.DirectorySeparatorChar + FileName;
                    RemotePath += FileHelper.AltDirectorySeparatorChar + FileName;
                    msg.RemotePath = RemotePath;
                    msg.Name = FileName;
                    try
                    {
#if NETFRAMEWORK
                        file.SaveAs(SavePath);
#else
                        using (var fStream = new FileStream(SavePath, FileMode.CreateNew))
                        {
                            stream.Position = 0;
                            var bytes = stream.ToArray();
                            fStream.Write(bytes, 0, bytes.Length);
                            fStream.Flush();
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        msg.State = false;
                        msg.Message = ex.Message;
                    }
                    this.Result.Add(msg);
                    OnMessage?.Invoke(msg, EventArgs.Empty);
                }
                // , f));
            });
            //Task.WaitAll(tasks.ToArray());
            this.OnComplete?.Invoke(this.Result, EventArgs.Empty);
        }
        #endregion

        #region 异步上传
        /// <summary>
        /// 异步上传
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task UploadAsync(CancellationToken token = default(CancellationToken))
        {
            var _Token = CancellationTokenSource.CreateLinkedTokenSource(this.Token.Token, token).Token;
            if (this.HttpFiles.Count == 0)
            {
                var msg = new FileMessage { Message = "请选择要上传的文件.", State = false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
                return;
            }
            if (!this.Config.IsOpen)
            {
                var msg = new FileMessage { Message = "暂未开放上传附件功能.", State = false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
                return;
            }
            List<Task> tasks = new List<Task>();
#if NETFRAMEWORK
            this.HttpFiles.Each<HttpPostedFile>(f =>
#else
            this.HttpFiles.Each<IFormFile>(f =>
#endif
            {
                tasks.Add(Task.Factory.StartNew(async fe =>
                {
                    var msg = new FileMessage();
#if NETFRAMEWORK
                    var file = (HttpPostedFile)fe;
                    msg.Length = file.ContentLength;
                    msg.Name = file.FileName.GetFileName();
#else
                    var file = (IFormFile)fe;
                    msg.Length = file.Length;
                    msg.Name = file.FileName;
#endif
                    msg.LocalPath = file.FileName;
                    msg.Extension = msg.Name.GetExtension();
                    /*判断文件大小*/
                    if (msg.Length > this.Config.MaxLength)
                    {
                        msg.State = false;
                        msg.Message = "上传文件超过系统限定大小.<br/>[" + msg.LocalPath + "]<br/>允许最大上传为:" + FileHelper.ConvertByte(this.Config.MaxLength);
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    /*判断文件后缀名*/
                    var uData = new UploadData();
                    if (this.Format.IsNullOrEmpty()) { uData = Config.Default;  }
                    else
                        if (!Config.Data.TryGetValue(this.Format, out uData))
                        uData = Config.Default;
                    if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
                    uData.Ext = uData.Ext.ReplacePattern(",", "|");
                    if (msg.Extension.IsNotNullOrEmpty() && !msg.Extension.Trim('.').IsMatch(@"^(" + uData.Ext + ")$"))
                    {
                        msg.State = false;
                        msg.Message = "上传的文件扩展名暂不允许上传.<br/>[" + msg.LocalPath + "]<br/>允许上传文件扩展名:" + uData.Ext.Replace("|", ",");
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    if (msg.Extension.IsNullOrEmpty()) msg.Extension = ".png";
                    /*验证文件Mime信息*/
                    if (this.IsValidateMime.HasValue && this.IsValidateMime.Value && !this.IsMime(msg.Extension, file.ContentType))
                    {
                        msg.State = false;
                        msg.Message = "上传的文件头类型不正确.<br/>当前文件[" + msg.LocalPath + "]头为:" + file.ContentType;
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    /*判断是否有木马特征*/
                    MemoryStream stream = new MemoryStream();
#if NETFRAMEWORK
                    file.InputStream.CopyTo(stream);
#else
                        lock (StreamLock)
                        {
                            var _stream = file.OpenReadStream();
                            _stream.CopyTo(stream);
                            _stream.Close();
                        }
#endif

                    if (this.Config.IsCheckTrojan && msg.Length < 10 * 1024 * 1024 && IsMuMa(stream))
                    {
                        msg.State = false;
                        msg.Message = "上传的文件检测出疑似含有有木马.";
                        this.Result.Add(msg);
                        OnMessage?.Invoke(msg, EventArgs.Empty);
                        return;
                    }
                    /*构建文件路径*/
                    var RemotePath = FileHelper.AltDirectorySeparatorChar + this.Config.UploadPath + FileHelper.AltDirectorySeparatorChar + uData.Path;
                    var SavePath = FileHelper.GetRootStaticPath() + FileHelper.DirectorySeparatorChar + this.Config.UploadPath + FileHelper.DirectorySeparatorChar + uData.Path;
                    if (this.PathFormat.IsNotNullOrEmpty())
                    {
                        var pathFormat = this.IsDatePath ? DateTime.Now.ToString(this.PathFormat) : this.PathFormat;
                        SavePath += FileHelper.DirectorySeparatorChar + pathFormat;
                        RemotePath += FileHelper.AltDirectorySeparatorChar + pathFormat;
                    }
                    SavePath = SavePath.GetBasePath();
                    FileHelper.Create(SavePath, FileAttribute.Directory);
                    if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd:4}.{ext}";
                    var FileName = this.FileName;
                    if (FileName.IsNullOrEmpty())
                    {
                        FileName = Config.FileNameFormat.RemovePattern(@"\.{ext}");
                        if (FileName.IsMatch(@"\{rnd\:?(\d+)\}"))
                        {
                            var num = FileName.GetMatch(@"\{rnd\:?(\d+)\}").ToCast<int>();
                            var rnd = RandomHelper.GetRandomString(num, RandomType.Number);
                            FileName = FileName.ReplacePattern(@"\{rnd\:?(\d+)\}", rnd);
                        }
                        FileName = DateTime.Now.ToString(FileName).ReplacePattern(@"\{(\d+)\}", "$1") + msg.Extension;
                    }
                    else FileName += "." + msg.Extension.Trim('.');
                    SavePath += FileHelper.DirectorySeparatorChar + FileName;
                    RemotePath += FileHelper.AltDirectorySeparatorChar + FileName;
                    msg.RemotePath = RemotePath;
                    msg.Name = FileName;
                    try
                    {
#if NETFRAMEWORK
                        file.SaveAs(SavePath);
#else
                        using (var fStream = new FileStream(SavePath, FileMode.CreateNew))
                        {
                            stream.Position = 0;
                            var bytes = stream.ToArray();
                            fStream.Write(bytes, 0, bytes.Length);
                            fStream.Flush();
                        }
#endif               
                    }
                    catch (Exception ex)
                    {
                        msg.State = false;
                        msg.Message = ex.Message;
                    }
                    this.Result.Add(msg);
                    OnMessage?.Invoke(msg, EventArgs.Empty);
                    await Task.Delay(1, _Token);
                }, f, _Token));
            });
            Task.WaitAll(tasks.ToArray(),10000, _Token);
            this.OnComplete?.Invoke(this.Result, EventArgs.Empty);
            await Task.Delay(1, _Token).ConfigureAwait(false);
        }
        #endregion

        #region 上传base64
        /// <summary>
        /// 上传base64
        /// </summary>
        /// <param name="base64String">base64字符串</param>
        /// <returns></returns>
        public FileMessage UploadBase64(string base64String)
        {
            FileMessage msg;
            if (base64String.IsNullOrEmpty() || base64String.RemovePattern(@"data:[^;]+;base64,").IsNullOrEmpty())
            {
                msg = new FileMessage { Message = "请选择要上传的文件.", State = false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
                return msg;
            }
            if (!this.Config.IsOpen)
            {
                msg = new FileMessage { Message = "暂未开放上传附件功能.", State = false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
                return msg;
            }

            var baseString = base64String;
            msg = new FileMessage();
            var ImageType = baseString.GetMatch(@"data:([^;]+);base64,");
            baseString = baseString.RemovePattern(@"data:[^;]+;base64,");

            if (ImageType.IsNullOrEmpty())
            {
                msg.State = false;
                msg.Message = "文件类型不能为空";
                this.Result.Add(msg);
                OnMessage?.Invoke(msg, EventArgs.Empty);
                return msg;
            }
            else
            {
                var keys = ContentTypes.Data.Where(a => a.Value.IsMatch(ImageType));
                if (keys != null && keys.Any())
                {
                    msg.Extension = keys.FirstOrDefault().Key;
                }
            }
            var file = baseString.FromBase64StringToBytes();
            msg.Length = file.Length;
            /*判断文件大小*/
            if (msg.Length > this.Config.MaxLength)
            {
                msg.State = false;
                msg.Message = "上传文件超过系统限定大小.<br/>[" + msg.LocalPath + "]<br/>允许最大上传为:" + FileHelper.ConvertByte(this.Config.MaxLength);
                this.Result.Add(msg);
                OnMessage?.Invoke(msg, EventArgs.Empty);
                return msg;
            }
            /*判断文件后缀名*/
            var uData = new UploadData();
            if (this.Format.IsNullOrEmpty()) { uData = Config.Default; uData.Path = this.Format; }
            else
                if (!Config.Data.TryGetValue(this.Format, out uData))
                uData = Config.Default;
            if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
            uData.Ext = uData.Ext.ReplacePattern(",", "|");
            if (msg.Extension.IsNullOrEmpty() || !msg.Extension.Trim('.').IsMatch(@"^(" + uData.Ext + ")$"))
            {
                msg.State = false;
                msg.Message = "上传的文件扩展名暂不允许上传.<br/>[" + msg.LocalPath + "]<br/>允许上传文件扩展名:" + uData.Ext.Replace("|", ",");
                this.Result.Add(msg);
                OnMessage?.Invoke(msg, EventArgs.Empty);
                return msg;
            }
            if (this.Config.IsCheckTrojan && msg.Length < 10 * 1024 * 1024 && IsMuMa(file.GetString()))
            {
                msg.State = false;
                msg.Message = "上传的文件检测出疑似含有有木马.";
                this.Result.Add(msg);
                OnMessage?.Invoke(msg, EventArgs.Empty);
                return msg;
            }
            /*构建文件路径*/
            var RemotePath = FileHelper.AltDirectorySeparatorChar + this.Config.UploadPath + FileHelper.AltDirectorySeparatorChar + uData.Path;
            var SavePath = FileHelper.GetRootStaticPath() + FileHelper.DirectorySeparatorChar + this.Config.UploadPath + FileHelper.DirectorySeparatorChar + uData.Path;
            if (this.PathFormat.IsNotNullOrEmpty())
            {
                var pathFormat = this.IsDatePath ? DateTime.Now.ToString(this.PathFormat) : this.PathFormat;
                SavePath += FileHelper.DirectorySeparatorChar + pathFormat;
                RemotePath += FileHelper.AltDirectorySeparatorChar + pathFormat;
            }
            SavePath = SavePath.GetBasePath();
            FileHelper.Create(SavePath, FileAttribute.Directory);
            if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd:4}.{ext}";
            var FileName = this.FileName;
            if (FileName.IsNullOrEmpty())
            {
                FileName = Config.FileNameFormat.RemovePattern(@"\.{ext}");
                if (FileName.IsMatch(@"\{rnd\:?(\d+)\}"))
                {
                    var num = FileName.GetMatch(@"\{rnd\:?(\d+)\}").ToCast<int>();
                    var rnd = RandomHelper.GetRandomString(num, RandomType.Number);
                    FileName = FileName.ReplacePattern(@"\{rnd\:?(\d+)\}", rnd);
                }
                FileName = DateTime.Now.ToString(FileName).ReplacePattern(@"\{(\d+)\}", "$1") +"."+ msg.Extension;
            }
            else FileName +="."+ msg.Extension;
            SavePath += FileHelper.DirectorySeparatorChar + FileName;
            RemotePath += FileHelper.AltDirectorySeparatorChar + FileName;
            msg.RemotePath = SavePath;
            msg.Name = FileName;

            try
            {
                using (var fStream = new FileStream(SavePath, FileMode.CreateNew))
                {
                    using (MemoryStream ms = new MemoryStream(file))
                    {
                        ms.CopyTo(fStream);
                        fStream.Flush();
                    }
                }
                this.Result.Add(msg);
            }
            catch (Exception ex)
            {
                msg.State = false;
                msg.Message = ex.Message;
                this.Result.Add(msg);
            }
            OnMessage?.Invoke(msg, EventArgs.Empty);
            this.OnComplete?.Invoke(this.Result, EventArgs.Empty);
            return msg;
        }
        /// <summary>
        /// 上传base64
        /// </summary>
        /// <param name="base64String">base64字符串</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task UploadBase64Async(string base64String, CancellationToken token = default(CancellationToken))
        {
            var _Token = CancellationTokenSource.CreateLinkedTokenSource(this.Token.Token, token).Token;
            if (base64String.IsNullOrEmpty() || base64String.RemovePattern(@"data:[^;]+;base64,").IsNullOrEmpty())
            {
                var msg = new FileMessage { Message = "请选择要上传的文件.", State=false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
            }
            if (!this.Config.IsOpen)
            {
                var msg = new FileMessage { Message = "暂未开放上传附件功能.",State=false };
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.Result.Add(msg);
                OnComplete?.Invoke(this.Result, EventArgs.Empty);
            }
            await new Task(async fe =>
            {
                var baseString = (string)fe;
                var msg = new FileMessage();
                var ImageType = baseString.GetMatch(@"data:([^;]+);base64,");
                baseString = baseString.RemovePattern(@"data:[^;]+;base64,");

                if (ImageType.IsNullOrEmpty())
                {
                    msg.State = false;
                    msg.Message = "文件类型不能为空";
                    this.Result.Add(msg);
                    OnMessage?.Invoke(msg, EventArgs.Empty);
                    return;
                }
                else
                {
                    var keys = ContentTypes.Data.Where(a => a.Value.IsMatch(ImageType));
                    if (keys != null && keys.Any())
                    {
                        msg.Extension = keys.FirstOrDefault().Key;
                    }
                }
                var file = baseString.FromBase64StringToBytes();
                msg.Length = file.Length;
                /*判断文件大小*/
                if (msg.Length > this.Config.MaxLength)
                {
                    msg.State = false;
                    msg.Message = "上传文件超过系统限定大小.<br/>[" + msg.LocalPath + "]<br/>允许最大上传为:" + FileHelper.ConvertByte(this.Config.MaxLength);
                    this.Result.Add(msg);
                    OnMessage?.Invoke(msg, EventArgs.Empty);
                    return;
                }
                /*判断文件后缀名*/
                var uData = new UploadData();
                if (this.Format.IsNullOrEmpty()) { uData = Config.Default; uData.Path = this.Format; }
                else
                    if (!Config.Data.TryGetValue(this.Format, out uData))
                    uData = Config.Default;
                if (this.FileType.IsNotNullOrEmpty()) uData.Ext = this.FileType;
                uData.Ext = uData.Ext.ReplacePattern(",", "|");
                if (msg.Extension.IsNullOrEmpty() || !msg.Extension.Trim('.').IsMatch(@"^(" + uData.Ext + ")$"))
                {
                    msg.State = false;
                    msg.Message = "上传的文件扩展名暂不允许上传.<br/>[" + msg.LocalPath + "]<br/>允许上传文件扩展名:" + uData.Ext.Replace("|", ",");
                    this.Result.Add(msg);
                    OnMessage?.Invoke(msg, EventArgs.Empty);
                    return;
                }
                if (this.Config.IsCheckTrojan && msg.Length < 10 * 1024 * 1024 && IsMuMa(file.GetString()))
                {
                    msg.State = false;
                    msg.Message = "上传的文件检测出疑似含有有木马.";
                    this.Result.Add(msg);
                    OnMessage?.Invoke(msg, EventArgs.Empty);
                    return;
                }
                /*构建文件路径*/
                var RemotePath = FileHelper.AltDirectorySeparatorChar + this.Config.UploadPath + FileHelper.AltDirectorySeparatorChar + uData.Path;
                var SavePath = FileHelper.GetRootStaticPath() + FileHelper.DirectorySeparatorChar + this.Config.UploadPath + FileHelper.DirectorySeparatorChar + uData.Path;
                if (this.PathFormat.IsNotNullOrEmpty())
                {
                    var pathFormat = this.IsDatePath ? DateTime.Now.ToString(this.PathFormat) : this.PathFormat;
                    SavePath += FileHelper.DirectorySeparatorChar + pathFormat;
                    RemotePath += FileHelper.AltDirectorySeparatorChar + pathFormat;
                }
                SavePath = SavePath.GetBasePath();
                FileHelper.Create(SavePath, FileAttribute.Directory);
                if (Config.FileNameFormat.IsNullOrEmpty()) Config.FileNameFormat = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd:4}.{ext}";
                var FileName = this.FileName;
                if (FileName.IsNullOrEmpty())
                {
                    FileName = Config.FileNameFormat.RemovePattern(@"\.{ext}");
                    if (FileName.IsMatch(@"\{rnd\:?(\d+)\}"))
                    {
                        var num = FileName.GetMatch(@"\{rnd\:?(\d+)\}").ToCast<int>();
                        var rnd = RandomHelper.GetRandomString(num, RandomType.Number);
                        FileName = FileName.ReplacePattern(@"\{rnd\:?(\d+)\}", rnd);
                    }
                    FileName = DateTime.Now.ToString(FileName).ReplacePattern(@"\{(\d+)\}", "$1") + msg.Extension;
                }
                else FileName += msg.Extension;
                SavePath += FileHelper.DirectorySeparatorChar + FileName;
                RemotePath += FileHelper.AltDirectorySeparatorChar + FileName;
                msg.RemotePath = SavePath;
                msg.Name = FileName;

                try
                {
                    using (var fStream = new FileStream(SavePath, FileMode.CreateNew))
                    {
                        using (MemoryStream ms = new MemoryStream(file))
                        {
                            ms.CopyTo(fStream);
                            fStream.Flush();
                        }
                    }
                    this.Result.Add(msg);
                }
                catch (Exception ex)
                {
                    msg.State = false;
                    msg.Message = ex.Message;
                    this.Result.Add(msg);
                }                
                OnMessage?.Invoke(msg, EventArgs.Empty);
                this.OnComplete?.Invoke(this.Result, EventArgs.Empty);
                await Task.Delay(1, _Token).ConfigureAwait(false);
            }, base64String, _Token);
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
            //return false;
            byte[] fileByte = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(fileByte, 0, int.Parse(stream.Length.ToString()));
            string fileContent = fileByte.GetString();
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
            foreach (var m in MuMa.RemovePattern(@"\\").Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (m.IsNullOrEmpty()) continue;
                if (_content.IndexOfX(m.ToLower()) > -1) return true;
            }
            return false;
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

    #region 上传文件模型
    /// <summary>
    /// 上传文件模型
    /// </summary>
    public class FileMessage : EntityBase
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 客户端地址
        /// </summary>
        public string LocalPath { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string RemotePath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 状态 true是成功 false 失败
        /// </summary>
        public Boolean State { get; set; } = true;
    }
    #endregion
}